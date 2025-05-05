
# 🛫 Arquitetura da Solução de Controle de Fluxo de Caixa

## 1\. Visão Geral da Arquitetura

Este projeto tem como seu foco a resolução do desafio proposto de desenvolver uma arquitetura de software escalável e resiliente, garantindo alta disponibilidade, segurança e desempenho.

Essa arquitetura para o controle de fluxo de caixa diário utiliza um padrão de microsserviços, com componentes desacoplados para garantir escalabilidade e resiliência. A comunicação entre os serviços é principalmente assíncrona através de filas de mensagens.

## 2\. Diagrama de Arquitetura




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

## 4\. Escalabilidade

\[Seção detalhando as estratégias de escalabilidade para cada componente.\]

## 5\. Resiliência

\[Seção detalhando as estratégias de tratamento de falhas e garantia de disponibilidade para cada componente.\]

## 6\. Segurança

\[Seção detalhando as medidas de segurança implementadas em cada camada, incluindo autenticação, autorização e proteção de dados.\]

## 7\. Monitoramento e Observabilidade

\[Seção detalhando as métricas, ferramentas e alertas para monitorar o sistema.\]

## 8\. Testes de Carga

\[Seção detalhando a estratégia e as ferramentas para testes de carga.\]

## 9\. Evoluções Futuras

\[Seção com possíveis melhorias e expansões do sistema.\]