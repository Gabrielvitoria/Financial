using Financial.Infra;
using Financial.Infra.Interfaces;
using Financial.Infra.Repositories;
using Financial.Service;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddTransient<DbContextConfigurer>();


//Services
builder.Services.AddTransient<IProcessLaunchservice, ProcessLaunchservice>();



//Repos
builder.Services.AddTransient<IProcessLaunchRepository, ProcessLaunchRepository>();


var connectionString = builder.Configuration.GetConnectionString("Default");

// para auto execução das Migrations
builder.Services.AddDbContext<DefaultContext>(options =>
     options.UseNpgsql(connectionString, sqlOptions => { sqlOptions.MigrationsAssembly("ProjectManagement.Infra");})
     );

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
