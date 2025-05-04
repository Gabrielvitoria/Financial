# Financial.WebApi

Construir imagem:
## docker build -t financial-app -f Dockerfile .

Subir ambiente
## docker-compose up -d

Acessar api
## http://localhost:44367/scalar/v1


## Migrations
- set FInancial.Infra no Manager Console.
- Execute:
dotnet ef migrations add "Alter_DescricaoDesejada" --project Financial.Infra