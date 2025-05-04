# Sistema de lançamentos financeiros e saldo diario Financial Web Api

* Este projeto tem como seu foco a resolução do desafio proposto de desenvolver uma arquitetura de software escalável e resiliente, garantindo alta disponibilidade, segurança e desempenho.

### Subir ambiente: /Financial
-> docker-compose up -d --build

### Financial App: 
-> http://localhost:44367/scalar/v1

### FInancial App Report
-> http://localhost:44368/scalar/v1

### RabbitMQ Manager
-> http://localhost:15672/#/


### Criar novas Migrations
- set Financial.Infra no Manager Console.
- Execute:
dotnet ef migrations add "Alter_DescricaoDesejada" --project Financial.Infra


### Construir imagem individual:
-> docker build -t financial-app -f Dockerfile .
-> docker build -t financial-report-image-app -f Dockerfile .
