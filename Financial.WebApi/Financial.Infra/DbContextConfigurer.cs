using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Financial.Infra
{
    [ExcludeFromCodeCoverage]
    public class DbContextConfigurer : IDesignTimeDbContextFactory<DefaultContext>
    {
        private readonly ILogger<DbContextConfigurer> _logger;

        public DefaultContext CreateDbContext(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var currentDirectory = Directory.GetCurrentDirectory();
            Console.WriteLine($"[DesignTime] Current Directory: {currentDirectory}"); // Adicione esta linha

            var basePath = Path.Combine(currentDirectory, "..", "Financial.WebApi");

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true);

            var configuration = builder.Build();
            var connectionString = configuration.GetConnectionString("Default");

            var optionsBuilder = new DbContextOptionsBuilder<DefaultContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new DefaultContext(optionsBuilder.Options);

        }

        
    }
}
