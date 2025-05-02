using Financial.Domain;
using Financial.Infra.MapEF;
using Microsoft.EntityFrameworkCore;

namespace Financial.Infra
{
    public class DefaultContext : DbContext
    {
        public DefaultContext(DbContextOptions options) : base(options)
        {
                
        }

        public virtual DbSet<Financiallaunch> Financiallaunch { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new FinancialConfiguration());
            
        }
    }
}
