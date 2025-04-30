using Financial.Common;
using Financial.Infra;
using Financial.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

//-> Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddTransient<DbContextConfigurer>();


//-> Services and Reposytory Dependencies Config
builder.Services.AddRespositoriDependecie();
builder.Services.AddServicesDependecie();

//-> Auto execução das Migrations
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<DefaultContext>(options =>
     options.UseNpgsql(connectionString, sqlOptions => { sqlOptions.MigrationsAssembly("ProjectManagement.Infra");})
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


//-> Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
