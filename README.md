# Sistema de lançamentos financeiros e saldo diario Financial Web Api

* Este projeto tem como seu foco a resolução do desafio proposto de desenvolver uma arquitetura de software escalável e resiliente, garantindo alta disponibilidade, segurança e desempenho.

##
### Desenho da solução
#### Fluxo principal:
<img src="https://github.com/Gabrielvitoria/Financial/blob/master/Documentacao/DIagrama_servico_financeiro-FLUXO_PRINCIPAL.drawio.svg">

#### Fluxo para processar lançamentos e atualizar saldo diario:
<img src="https://github.com/Gabrielvitoria/Financial/blob/master/Documentacao/DIagrama_servico_financeiro-FLUXO_RELATORIOS_NOVO_LAN%C3%87AMENTO.drawio.svg">

#### Fluxo para obter o saldo diario:
<img src="https://github.com/Gabrielvitoria/Financial/blob/master/Documentacao/DIagrama_servico_financeiro-FLUXO_RELATORIOS_OBTER_CONSOLIDADO.drawio.svg">

##
### Como usar esse projeto
É bem simples de já começar rodando
```bash
docker-compose up -d --build
```



##
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
