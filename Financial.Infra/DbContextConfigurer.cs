using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Financial.Infra
{
    public class DbContextConfigurer : IDesignTimeDbContextFactory<DefaultContext>
    {
        private readonly ILogger<DbContextConfigurer> _logger;

        public DbContextConfigurer(ILogger<DbContextConfigurer> logger) =>
            _logger = logger;



        public DefaultContext CreateDbContext(string[] args)
        {

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
           
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true);

            var configuration = builder.Build();
            var connectionString = configuration.GetConnectionString("Default");

            _logger.LogInformation("CONNECTION STRING:", connectionString);

            var optionsBuilder = new DbContextOptionsBuilder<DefaultContext>();
            
            optionsBuilder.UseNpgsql(connectionString);
          
            return new DefaultContext(optionsBuilder.Options);
        }

        
    }
}
