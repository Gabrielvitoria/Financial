using Financial.Common;
using Financial.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


//-> Services and Reposytory Dependencies Config
var connectionStringRedis = builder.Configuration.GetConnectionString("ConexaoRedis");

builder.Services.AddRespositoriDependecie();
builder.Services.AddServicesDependecie();
builder.Services.AddBackgroundServiceDependecie();
builder.Services.AddServiceExternal(connectionStringRedis);

//-> autenticacao-autorizacao
var key = Encoding.ASCII.GetBytes(Settings.Secret);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});



// Configuração do Redis para Cache Distribuído (você já tem isso, mas pode estar faltando a conexão base)
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("ConexaoRedis"); // Ou sua string de conexão
    options.InstanceName = "FinancialApp_"; // Opcional: Prefixo para as chaves
});

// Registro do IConnectionMultiplexer
builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    string connectionString = builder.Configuration.GetConnectionString("ConexaoRedis"); // Ou sua string de conexão
    return ConnectionMultiplexer.Connect(connectionString);
});


//-> Seguranca
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("gerente", policy => policy.RequireClaim("Project", "gerente"));

});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapOpenApi();

app.MapScalarApiReference(o =>
                          o.WithTitle("Financial Report API")
                           .WithTheme(ScalarTheme.BluePlanet)
                          
);


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
