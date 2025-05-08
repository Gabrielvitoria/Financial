    # See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

    # This stage is used when running from VS in fast mode (Default for Debug configuration)
    FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
    USER app
    WORKDIR /app
    EXPOSE 8080
    EXPOSE 44399
    ENV ASPNETCORE_URLS=http://*:8080
    ENV ASPNETCORE_ENVIRONMENT="Docker"

    # This stage is used to build the service project
    FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
    COPY . ./src
    WORKDIR /src
    COPY ["Financial.WebApi/Financial.WebApi.csproj", "Financial.WebApi"]
    COPY ["Financial.Service/Financial.Service.csproj", "Financial.Service"]
    COPY ["Financial.Infra/Financial.Infra.csproj", "Financial.Infra"]
    COPY ["Financial.Domain/Financial.Domain.csproj", "Financial.Domain"]
    COPY ["Financial.Common/Financial.Common.csproj", "Financial.Common"]
    COPY ["Financial.Works/Financial.Works.csproj", "Financial.Common"]

    RUN dotnet restore "Financial.WebApi/Financial.WebApi.csproj"
    COPY . .
    WORKDIR "/src/Financial.WebApi"
    RUN dotnet build "Financial.WebApi.csproj" -c Release -o /app/build

    # This stage is used to publish the service project to be copied to the final stage
    FROM build AS publish
    RUN dotnet publish "Financial.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

    # This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
    FROM base AS final
    WORKDIR /app
    COPY --from=publish /app/publish .
    ENTRYPOINT ["dotnet", "Financial.WebApi.dll"]