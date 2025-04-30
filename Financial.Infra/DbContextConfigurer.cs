using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Financial.Infra
{
    public class DbContextConfigurer : IDesignTimeDbContextFactory<DefaultContext>
    {
        public DefaultContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DefaultContext>();
            
            optionsBuilder.UseNpgsql(@"User ID=postgres;Password=123456;Host=localhost;Port=5432;Database=FinanciallaunchDB;Pooling=true;");
          
            return new DefaultContext(optionsBuilder.Options);
        }

        
    }
}
