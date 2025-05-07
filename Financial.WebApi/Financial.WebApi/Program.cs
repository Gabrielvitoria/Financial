using Financial.Common;
using Financial.Infra;
using Financial.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Diagnostics.CodeAnalysis;
using System.Text;

public partial class Program
{
    [ExcludeFromCodeCoverage]
    public static async Task Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("Default");

        // Add services to the container.
        builder.Services.AddControllers();

        //-> Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddTransient<DbContextConfigurer>();


        //-> Services and Reposytory Dependencies Config
        builder.Services.AddRespositoriDependecie();
        builder.Services.AddServicesDependecie();

        //-> Auto execução das Migrations

        builder.Services.AddDbContext<DefaultContext>(options =>
             options.UseNpgsql(connectionString, sqlOptions => { sqlOptions.MigrationsAssembly("Financial.Infra"); })
             );



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

        //-> Seguranca
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("gerente", policy => policy.RequireClaim("Project", "gerente"));

        });


        var app = builder.Build();

        //-> Auto execução das Migrations
        using (var scope = app.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DefaultContext>();
            await dbContext.Database.EnsureCreatedAsync();
        }



        app.MapOpenApi();
        app.MapScalarApiReference(o =>
                                  o.WithTitle("Financial API")
                                   .WithTheme(ScalarTheme.BluePlanet)

        );


        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();


    }
}