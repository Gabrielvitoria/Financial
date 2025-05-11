
# 🛫 Arquitetura da Solução de Controle de Fluxo de Caixa

## 1\. Visão Geral da Arquitetura

Este projeto tem como seu foco a resolução do desafio proposto de desenvolver uma arquitetura de software escalável e resiliente, garantindo alta disponibilidade, segurança e desempenho.

Essa arquitetura para o controle de fluxo de caixa diário utiliza um padrão de microsserviços, com componentes desacoplados para garantir escalabilidade e resiliência. A comunicação entre os serviços é principalmente assíncrona através de filas de mensagens.

## 2\. Diagrama de Arquitetura

<img src="https://github.com/Gabrielvitoria/Financial/blob/master/Documentacao/DIagrama_servico_financeiro-Fluxograma.drawio.svg">


## 3\. Componentes da Arquitetura 🛬

### 3.1. YARP (Yet Another Reverse Proxy)

*   **Responsabilidade:** Ponto de entrada para todas as requisições dos clientes. Realiza o roteamento para as APIs apropriadas e atua como um balanceador de carga.
*   **Escalabilidade:** Escalável horizontalmente com múltiplas instâncias rodando atrás de um balanceador de carga (interno ou externo).
*   **Resiliência:** Configurado com retries e circuit breakers para lidar com falhas temporárias nas APIs backend.
*   **Segurança:** Implementação de HTTPS, possível tratamento de CORS e outras políticas de segurança.
*   **Autenticação:** O YARP pode ser configurado para validar tokens JWT antes de encaminhar as requisições para as APIs backend. Isso garante que apenas requisições autenticadas sejam processadas.
*   **Autorização:** O YARP pode ser configurado para verificar claims específicas no token JWT, permitindo ou negando o acesso a determinados endpoints com base nas permissões do usuário.
*   **Exemplo:** Se o token JWT contiver uma claim "role" com o valor "admin", o YARP pode permitir o acesso a endpoints administrativos, enquanto bloqueia o acesso a usuários sem essa claim.
*   **Configuração:** O YARP pode ser configurado através de um arquivo JSON ou programaticamente, permitindo flexibilidade na definição de rotas e regras de autenticação/autorização.
*   **A configuração do YARP:** O YARP é configurado através de um arquivo JSON, onde são definidas as rotas e os destinos para cada serviço. O arquivo de configuração pode ser facilmente modificado para adicionar ou remover serviços, permitindo uma gestão dinâmica das rotas.
```json
{
    "ReverseProxy": {
    "Routes": {
      // Rotas para o Container Report
      "report-daily-balance": {
        "ClusterId": "cluster-report",
        "Match": {
          "Path": "/dailybalance"
        }
      },
      "report-daily-launch": {
        "ClusterId": "cluster-report",
        "Match": {
          "Path": "/dailylaunch"
        }
      },
      // Rotas para o Container Financeiro
      "financial-launch": {
        "ClusterId": "cluster-financial",
        "Match": {
          "Path": "/launch"
        }
      },
      "financial-pay": {
        "ClusterId": "cluster-financial",
        "Match": {
          "Path": "/pay"
        }
      },
      // Rota para Autenticação
      "financial-auth-login": {
        "ClusterId": "cluster-auth",
        "Match": {
          "Path": "/login"
        }
      }
    },
    "Clusters": {
      "cluster-report": {
        "Destinations": {
          "report-service": {
            "Address": "http://financial-report-app:8081/api/v1/Report"
          }
        }
      },
      "cluster-financial": {
        "Destinations": {
          "financial-service": {
            "Address": "http://financial-app:8080/api/v1/Financial"
          }
        }
      },
      "cluster-auth": {
        "Destinations": {
          "auth-service": {
             "Address": "http://financial-app:8080/api/v1/Authenticate" 
          }
        }
      }
    }
  }
}
```

### 3.2. API de Lançamentos (Recebimento e Autenticação)

*   **Responsabilidades:**
    *   **Recebimento de Lançamentos:** Recebe requisições para registrar novos lançamentos financeiros, realiza validações iniciais e enfileira as mensagens no RabbitMQ.
    *   **Autenticação:** Fornece endpoints para autenticação de usuários (utilizando dois usuários "fake" para demonstração), gerando tokens JWT reais em caso de sucesso.
*   **Escalabilidade:** Escalável horizontalmente com múltiplas instâncias. O YARP distribui a carga entre as instâncias.
*   **Resiliência:** A funcionalidade de recebimento de lançamentos se beneficia do desacoplamento via RabbitMQ. A disponibilidade da autenticação é crucial para o acesso ao sistema; múltiplas instâncias da API ajudam na resiliência.
*   **Segurança:**
    *   **Autenticação:** Implementação de um fluxo de autenticação (e.g., via POST de credenciais para `/login`) que retorna um token JWT em caso de sucesso.
    *   **Autorização:** Utilização do token JWT para autorizar o acesso aos endpoints de recebimento de lançamentos (`/launch`) e (potencialmente) a outros recursos. As claims no token (como roles) podem ser usadas para controle de acesso.
* **Validações:** A APi de Lançamentos realiza validações de dados de entrada, como verificar se o ID do lançamento já existe, se o valor é positivo e se os campos obrigatórios estão preenchidos. Caso as validações falhem, a API retorna um erro apropriado.
* **Exemplo de Payload:** O payload para criar um lançamento inclui campos como `idempotencyKey`, `launchType`, `paymentMethod`, `coinType`, `value`, `bankAccount`, `nameCustomerSupplier`, `costCenter` e `description`. Esses campos são validados antes de serem enfileirados no RabbitMQ.
```json
{
  "idempotencyKey": "33A5A8E9-F0B3-48D3-873C-95A66EF118CC",
  "launchType": 1,
  "paymentMethod": 1,
  "coinType": "USD",
  "value": 100.98,
  "bankAccount": "453262",
  "nameCustomerSupplier": "Nome novo de Customer",
  "costCenter": "666",
  "description": ""
}
```
* **Exemplo de Payload de Autenticação:** O payload para autenticação inclui campos como `userName` e `password`. A API valida as credenciais e retorna um token JWT em caso de sucesso.
```json
{
  "userName": "master",
  "password": "master"
}
```



### 3.3. RabbitMQ (Fila de Mensagens)

*   **Responsabilidade:** Fila de mensagens robusta e confiável para desacoplar a API de recebimento do serviço de saldo. Garante que os lançamentos sejam processados mesmo se o serviço de saldo estiver temporariamente indisponível.
*   **Escalabilidade:** Escalável aumentando o número de filas, exchanges e consumers.
*   **Resiliência:** Configuração de filas duráveis para garantir a persistência das mensagens em caso de falha do broker. Utilização de dead-letter queues (DLQs) para tratamento de mensagens que não puderam ser processadas. Mecanismos de confirmação de entrega para garantir que as mensagens sejam processadas pelo menos uma vez.
*   **Configuração:** O RabbitMQ está configurado com características do Exchange Padrão (Default Exchange):

 **Nome:** O nome do exchange padrão é uma string vazia ("").

 **Tipo:** O tipo do exchange padrão é direct.

 **Binding Implícito:** Binding entre o exchange padrão e a fila. A chave de roteamento (routing key) desse binding é o nome da fila.


### 3.4. Serviço de Saldo

*   **Responsabilidade:** Consumir as mensagens da fila do RabbitMQ, processar os lançamentos (converter valores, etc.) e atualizar o saldo diário no Redis de forma atômica.
*   **Escalabilidade:** Escalável aumentando o número de consumers que processam as mensagens da fila.
*   **Resiliência:** Se o serviço falhar, as mensagens permanecerão na fila do RabbitMQ até que seja reiniciado. Lógica de retry pode ser implementada para tentativas de conexão com o Redis.
*   **Segurança:** A comunicação com o RabbitMQ e o Redis (geralmente em rede interna) deve ser protegida com as configurações de segurança apropriadas.

### 3.5. Redis (Cache/Armazenamento de Estado)

*   **Responsabilidade:** Armazenar o saldo diário consolidado para acesso rápido pela API de saldo. A atomicidade das operações garante a consistência do saldo.
*   **Escalabilidade:** Escalável utilizando clustering e replicação para alta disponibilidade e distribuição de carga.
*   **Resiliência:** Configuração de persistência (RDB e/ou AOF) para garantir a durabilidade dos dados. Replicação para failover em caso de falha do nó primário.
*   **Segurança:** Configuração de senha e acesso restrito pela rede.

### 3.6. API de Saldo

*   **Responsabilidade:** Fornecer uma interface para os clientes consultarem o saldo diário atual (e.g., via GET para `/api/v1/Report/DailyBalance`). Busca o saldo diretamente do Redis.
*   **Escalabilidade:** Escalável horizontalmente com múltiplas instâncias (a leitura do Redis é rápida). O YARP distribui a carga.
*   **Resiliência:** Depende da disponibilidade do Redis. A replicação do Redis melhora a resiliência.
*   **Segurança:** Validação do token JWT enviado no header da requisição (e.g., `Authorization: Bearer <token>`) para garantir que apenas clientes autenticados possam acessar o saldo.
  * **Autorização:** O token JWT pode conter claims que definem o nível de acesso do usuário (No caso apenas usuários com claim 'master' podem realizar operações).

### 3.7. Fluxos de negócios

  * **Fluxo de Lançamentos:** O cliente envia um lançamento para a API de Lançamentos, que valida e enfileira a mensagem no RabbitMQ. O Serviço de Saldo consome a mensagem, processa o lançamento e atualiza o saldo no Redis.
  * **Fluxo de Consulta de Saldo:** O cliente envia uma requisição GET para a API de Saldo, que consulta o saldo diretamente no Redis e retorna a resposta.
  * **Fluxo de Pagamento:** O cliente envia uma requisição POST para a API de Pagar, que consulta o ID informado e existe lançamento com status aberto. Caso tenha, confirma o pagamento e enfileira a mensagem no RabbitMQ com novo status. O Serviço de Saldo consome a mensagem, processa o lançamento e atualiza o saldo e o lançamento no Redis.
    
## 4\. Escalabilidade

*   A API de Lançamentos pode ser escalada horizontalmente adicionando mais instâncias em containers Docker. Um balanceador de carga (como o YARP) distribui o tráfego entre as instâncias.

## 5\. Resiliência

*   O RabbitMQ garante a resiliência usando filas duráveis, que persistem as mensagens em disco. Se o Serviço de Saldo falhar, as mensagens serão entregues quando ele se recuperar.

## 6\. Segurança

A API de Autenticação usa HTTPS para criptografar a comunicação. Os tokens JWT são assinados para garantir sua integridade. Todas as APIs verificam o token JWT no header Authorization para autorizar o acesso.

## 7\. Monitoramento e Observabilidade

1. API de Lançamentos e API de Saldo:

  **Instrumentação:**

Utilizar os pacotes do OpenTelemetry ao projeto ASP.NET Core.
Configure o OpenTelemetry no Program.cs para coletar métricas, traces e logs.
Use as instrumentações fornecidas pelo OpenTelemetry para coletar dados automaticamente (por exemplo, instrumentação do ASP.NET Core, instrumentação do HttpClient).
Exportar os dados para o coletor OTLP.
##
  **Métricas Importantes:**

Tempo de resposta dos endpoints da API.
Taxa de requisições por endpoint.
Taxa de erros (códigos de status HTTP).
Uso de recursos (CPU, memória).
Tamanho das mensagens enviadas/recebidas.

##
  **Traces Importantes:**

Propague o contexto de trace entre os serviços para rastrear as requisições de ponta a ponta.
Capture informações sobre as chamadas de banco de dados, chamadas a outros serviços e outras operações relevantes.

##

 **Logs Importantes:**

Registre eventos importantes na vida útil de uma requisição.
Use um formato de log estruturado para facilitar a análise.

 **Por que instrumentar:**

Para entender o desempenho e o comportamento interno das suas APIs.
Para identificar gargalos e otimizar o código.
Para diagnosticar erros e problemas de desempenho.
Para correlacionar as requisições entre os serviços e entender o fluxo completo.
##

2. Serviço de Saldo:

**Instrumentação:**

Adicione os pacotes do OpenTelemetry ao seu projeto.
Configure o OpenTelemetry para coletar métricas, traces e logs.
Instrumente o código que interage com o RabbitMQ e o Redis.
Exporte os dados para o coletor OTLP.
##

 **Métricas Importantes:**
Número de mensagens consumidas do RabbitMQ.
Tempo de processamento das mensagens.
Latência das operações do Redis.
Uso de recursos (CPU, memória).
Tamanho das mensagens processadas.
##

 **Traces Importantes:**
Capture informações sobre as operações do RabbitMQ (consumo, confirmação).
Capture informações sobre as operações do Redis (leituras, gravações).
Propague o contexto de trace para correlacionar as operações com as requisições da API.

##

**Logs Importantes:**
Registrar eventos importantes no processamento das mensagens.
Registre erros e exceções.

##
**Por que instrumentar:**
Entender o desempenho e o comportamento do serviço de processamento.
Identificar gargalos no processamento das mensagens.
Diagnosticar erros e falhas no processamento.
Entender o impacto do serviço de processamento no desempenho geral da aplicação.

##

3. RabbitMQ e Redis:

Embora não precise instrumentar o RabbitMQ e o Redis diretamente no sentido de adicionar código neles, pode-se coletar métricas e logs deles.

**RabbitMQ:**

O RabbitMQ fornece métricas via sua API HTTP ou Prometheus exporter.
Métricas como:

* Número de mensagens nas filas.
* Taxa de consumo/produção de mensagens.
* Número de conexões.
* O RabbitMQ também gera logs que você pode coletar.
##
**Redis:**
O Redis fornece métricas via o comando INFO.

 Métricas como:
* Uso de memória.
* Número de conexões.
* Número de operações por segundo.
* Latência das operações.

##

**Como coletar:**

* Usar o Prometheus para coletar métricas do RabbitMQ e do Redis.
* Configurar o RabbitMQ para expor métricas via Prometheus exporter.
* Usar ferramentas de coleta de logs (como Fluentd ou Logstash) para coletar os logs.

**Por que coletar:**

* Monitorar a saúde e o desempenho do RabbitMQ e do Redis.
* Identificar problemas de desempenho ou capacidade.
* Correlacionar os problemas do RabbitMQ ou Redis com os problemas da sua aplicação.

* ##

**Em Resumo:**

Para ter uma observabilidade completa:

* Instrumentar todos os seus serviços com o OpenTelemetry.
* Configurar o coletor OTLP para receber e processar os dados.
* Usar o Prometheus para coletar métricas.
* Usar o Grafana para visualizar as métricas e os traces.
* Coletar métricas e logs do RabbitMQ e do Redis para monitorar a infraestrutura.


## 8\. Testes de Carga

Este guia detalha como avaliar os resultados dos testes de carga executados com k6 para as APIs de autenticação, criação de lançamentos e relatório de saldo diário.
```bash
Financial
└── Financial.k6
    ├── 01.dailyBalance.js     (Script k6 para criar lançamentos 1000 de R$1.00)
    ├── 02.dailyBalance.js     (Script k6 para testar relatório de saldo diário) - Após esse teste, o saldo diário deverá ser de R$1000.00. Deverá ser rodado antes dos outros testes para não atrapalhar o saldo diário. Recomento destruir os container e criar novamente.
    ├── Anotações e cenários.txt (Registro de planejamento e contexto dos testes com os resultados)
    ├── auth.js                (Script k6 para testar API de autenticação)
    ├── Dockerfile             (Configuração Docker para ambiente de teste - opcional)
    ├── lauch.js               (Script k6 para testar criação de um lançamento)
    └── pay.js                 (Script k6 para testar fluxo de lançamento e pagamento)
```
Para cada teste, observe as seguintes métricas principais:

**checks_succeeded:** Indica a porcentagem de verificações bem-sucedidas. O valor ideal é 100%, garantindo que as respostas da API atendem aos critérios esperados (status code, formato do corpo, etc.). Qualquer valor abaixo de 100% indica falhas que precisam ser investigadas.

**http_req_duration:** Mede o tempo que as requisições HTTP levam para serem concluídas. Analise:

**avg (média):** O tempo de resposta médio para todas as requisições. Valores altos podem indicar lentidão geral.
p(90) e p(95) (percentis 90 e 95): Indicam o tempo de resposta para 90% e 95% das requisições, respectivamente. Esses valores são importantes para entender a experiência da maioria dos usuários, pois os valores médios podem ser distorcidos por outliers.

**max (máximo):** O tempo de resposta mais longo. Valores muito altos podem indicar problemas intermitentes ou gargalos severos.

**http_req_failed:** Indica a porcentagem de requisições HTTP que falharam completamente (e.g., timeouts, erros de conexão). O valor ideal é 0%. Qualquer valor acima indica instabilidade ou problemas na API sob carga.

**http_reqs:** Mostra o número total de requisições enviadas e a taxa de requisições por segundo (throughput). Um throughput baixo sob alta concorrência pode indicar gargalos.

**iterations:** O número total de vezes que o script principal do k6 foi executado. Compare com o número de VUs e a duração do teste para entender a taxa de execução.

**vus:** O número de usuários virtuais ativos durante o teste. Certifique-se de que corresponde ao configurado.

**dropped_iterations:** Indica o número de iterações que o k6 não conseguiu completar dentro da duração do teste. Um número alto pode sugerir que a carga era muito alta para a duração configurada ou que as iterações estavam demorando muito para serem concluídas.

### Avaliação Específica por Teste:

#### **Autenticação (1000 VUs):**

* Aceitável: checks_succeeded: 100%, http_req_failed: 0%.

* Preocupante: Latência média e percentis altos (p(90), p(95) acima de 1-2 segundos), baixo throughput (< 50 req/s), alto número de dropped_iterations.
* Ação: Investigar a performance do servidor de autenticação sob alta concorrência.
* Novo Lançamento + Autenticação (100 VUs):

* Aceitável: checks_succeeded: 100%, http_req_failed: 0%.
* Atenção: Latência média acima de 500ms, percentis 90/95 acima de 1 segundo, throughput abaixo do esperado.
* Ação: Monitorar a latência em cargas maiores e considerar otimizações se os tempos de resposta forem considerados lentos para a experiência do usuário.
* Saldo Criar Lançamentos (100 VUs, 1000 iterações):

* Aceitável: checks_succeeded: 100%, http_req_failed: 0%, iterations próximo de 1000.
* Atenção: Latência média acima de 1 segundo, percentis altos significativamente maiores que a média, throughput baixo para o número de VUs.
* Ação: Analisar o desempenho da API de criação sob carga para identificar gargalos.
* Saldo Diário - Utilizando 1000 Lançamentos (1000 VUs):

* Excelente: checks_succeeded: 100%, http_req_failed: 0%, latência média abaixo de 200ms, percentis 90/95 abaixo de 500ms, throughput alto (> 5000 req/s).
* Ação: Monitorar os recursos do servidor para garantir que ele não esteja sobrecarregado, mesmo com o bom desempenho.

#### Interpretação Geral:

* A ausência de falhas (http_req_failed: 0%) é um bom sinal de estabilidade básica das APIs.
* A latência é o principal ponto de atenção, especialmente no endpoint de autenticação sob alta carga e na criação de lançamentos.
* O throughput deve ser avaliado em relação ao número de usuários virtuais e à capacidade esperada do sistema.

* Como executar os testes:*
* Certifique-se de ter o k6 instalado ou docker e execute os scripts com o comando:

Estando no diretório do script Financial\Financial.k6, execute:

Exemplo:
Para criar 1000 lançamentos de R$1.00, execute:
```bash
docker run --rm -i --network="host" grafana/k6 run - <01.dailyBalance.js
```
na sequencia, para testar o saldo diário, execute:
```bash
docker run --rm -i --network="host" grafana/k6 run - <02.dailyBalance.js
```

Para o fluxo de pagamento, execute:
```bash
docker run --rm -i --network="host" grafana/k6 run - <pay.js
```

Para o fluxo de criar lançamentos
```bash
docker run --rm -i --network="host" grafana/k6 run - <lauch.js
```

Para o fluxo de autenticação, execute:
```bash
docker run --rm -i --network="host" grafana/k6 run - <auth.js
```

⭐ Caso queira visualizar alguns testes que forame executados de basa para Interpretação Geral,  
https://github.com/Gabrielvitoria/Financial/blob/master/Financial.k6/Anota%C3%A7%C3%B5es%20e%20cen%C3%A1rios.txt


## 8.1\. Testes de unidade
* Projeto Financial - Web principal: 🚀
* Projeto Financial Relatório - Web: 🔄 (Pendente realizar cobertura) ⚠️

⭐ Relatório de cobertura Projeto Financial - Web principal
<img src="https://github.com/Gabrielvitoria/Financial/blob/master/Documentacao/Summary_Coverage_Report_Financial.png">


## 9\. Evoluções Futuras
*   Otimização do fluxo de pagamento, onde o cliente pode enviar um lançamento com status "pago" e o sistema irá verificar se existe um lançamento com status "aberto" para o mesmo ID. Caso exista, o sistema irá confirmar o pagamento e atualizar o saldo no Redis.
*   Implementação de um sistema de notificações para alertar os usuários sobre lançamentos pendentes ou vencidos.
  * Implementação de um sistema de relatórios para gerar relatórios financeiros detalhados com base nos lançamentos registrados.
  * Implementação de um sistema de auditoria para registrar todas as operações realizadas no sistema, incluindo criação, atualização e exclusão de lançamentos.
  * Implementação de um sistema de backup e recuperação para garantir a integridade dos dados em caso de falhas.
  * Implementação de um sistema de autenticação multifator (MFA) para aumentar a segurança do sistema.
  * Implementação de um sistema de controle de acesso baseado em papéis (RBAC) para gerenciar permissões de usuários e grupos. Pois atualmente o sistema só possui dois usuários com permissões diferentes, mas não há controle de acesso granular para diferentes operações ou recursos.
  * Implementação de um sistema de orquestramento de containers (como Kubernetes) para gerenciar a escalabilidade e resiliência da aplicação em produção.
  * Implementação de um sistema de CI/CD (Integração Contínua/Entrega Contínua) para automatizar o processo de build, teste e deploy da aplicação.
 


## 10\. Documentação das API
*  A solução utiliza **SCALAR** que fornece uma documentação das API de maneira padronizada. Scalar - Document, Test & Discover APIs
Derivado de um arquivo Swagger ou OpenAPI Specification, o Scalar é uma solução que constrói documentações de API ricas em detalhes de forma automatizada. Sua proposta é oferecer uma experiência intuitiva e eficaz, capacitando desenvolvedores a gerar documentações abrangentes que explicitam endpoints, parâmetros, respostas e ilustrações de uso em múltiplos contextos de programação.
Exemplo da solução utilizando:
<img src="https://github.com/Gabrielvitoria/Financial/blob/master/Documentacao/print_lancamentos_api_scalar.png?raw=true">
<img src="https://github.com/Gabrielvitoria/Financial/blob/master/Documentacao/print_report_api_scalar.png?raw=true">


## 11\. Como usar esse projeto
É bem simples de já começar rodando
```bash
docker-compose up -d --build
```

*  **Financial API:** http://localhost:44367/scalar/v1
 
*  **Financial API Report:** http://localhost:44368/scalar/v1

*  **YARP API:** http://localhost:44369/scalar/v1

*  **RabbitMQ Manager:** http://localhost:15672/

Tem disponível dois usuários:
```bash
Roles: "gerente"

user: master
password: master
```

```bash
Roles: "usuario"
user: basic
password: basic
```

##
#### ** Caso queira criar criar novas Migrations

Definir o projeto "Financial.Infra" no Manager Console. O projetoe stá configurado para auto executar as atualizações pendentes que exitir e criar a base de dados.

Execute:
```bash
dotnet ef migrations add "Alter_DescricaoDesejada" --project Financial.Infra
```

*  Para construir imagem individual
```bash
docker build -t financial-app -f Dockerfile .
```
```bash
docker build -t financial-report-image-app -f Dockerfile .
```

##
####  Exemplo de payload gerar um token
```json
curl -X POST \
  http://localhost:44369/api/v1/Authenticate/login \
  -H 'Content-Type: application/json' \
  -d '{
    "userName": "master",
    "password": "master"
  }'
```

##
####  Exemplo de payload para criar um lançamento
```json
{
  "idempotencyKey": "33A5A8E9-F0B3-48D3-873C-95A66EF118CC",
  "launchType": 1,
  "paymentMethod": 1,
  "coinType": "USD",
  "value": 100.98,
  "bankAccount": "453262",
  "nameCustomerSupplier": "Nome novo de Customer",
  "costCenter": "666",
  "description": ""
}
```
