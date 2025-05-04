# Financial.WebApi

## Subir ambiente: /Financial
-> docker-compose up -d --build

## Financial App: 
-> http://localhost:44367/scalar/v1

## FInancial App Report
-> http://localhost:44368/scalar/v1

## RabbitMQ Manager
-> http://localhost:15672/#/





## Criar novas Migrations
- set Financial.Infra no Manager Console.
- Execute:
dotnet ef migrations add "Alter_DescricaoDesejada" --project Financial.Infra

## Construir imagem:
-> docker build -t financial-app -f Dockerfile .