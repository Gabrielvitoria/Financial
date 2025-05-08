
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

### 3.2. API de Lançamentos (Recebimento e Autenticação)

*   **Responsabilidades:**
    *   **Recebimento de Lançamentos:** Recebe requisições para registrar novos lançamentos financeiros, realiza validações iniciais e enfileira as mensagens no RabbitMQ.
    *   **Autenticação:** Fornece endpoints para autenticação de usuários (utilizando dois usuários "fake" para demonstração), gerando tokens JWT reais em caso de sucesso.
*   **Escalabilidade:** Escalável horizontalmente com múltiplas instâncias. O YARP distribui a carga entre as instâncias.
*   **Resiliência:** A funcionalidade de recebimento de lançamentos se beneficia do desacoplamento via RabbitMQ. A disponibilidade da autenticação é crucial para o acesso ao sistema; múltiplas instâncias da API ajudam na resiliência.
*   **Segurança:**
    *   **Autenticação:** Implementação de um fluxo de autenticação (e.g., via POST de credenciais para `/auth/login`) que retorna um token JWT em caso de sucesso.
    *   **Autorização:** Utilização do token JWT para autorizar o acesso aos endpoints de recebimento de lançamentos (`/api/lancamentos`) e (potencialmente) a outros recursos. As claims no token (como roles) podem ser usadas para controle de acesso.

### 3.3. RabbitMQ (Fila de Mensagens)

*   **Responsabilidade:** Fila de mensagens robusta e confiável para desacoplar a API de recebimento do serviço de saldo. Garante que os lançamentos sejam processados mesmo se o serviço de saldo estiver temporariamente indisponível.
*   **Escalabilidade:** Escalável aumentando o número de filas, exchanges e consumers.
*   **Resiliência:** Configuração de filas duráveis para garantir a persistência das mensagens em caso de falha do broker. Utilização de dead-letter queues (DLQs) para tratamento de mensagens que não puderam ser processadas. Mecanismos de confirmação de entrega para garantir que as mensagens sejam processadas pelo menos uma vez.

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

\[Seção detalhando as métricas, ferramentas e alertas para monitorar o sistema.\]

## 8\. Testes de Carga

\[Seção detalhando a estratégia e as ferramentas para testes de carga.\]

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

* Financial API: http://localhost:44367/scalar/v1
 
* Financial API Report: http://localhost:44368/scalar/v1
* 
* YARP API: http://localhost:44369/scalar/v1

* RabbitMQ Manager: http://localhost:15672/

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
