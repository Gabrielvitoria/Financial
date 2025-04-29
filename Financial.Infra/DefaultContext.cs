using Financial.Domain;
using Microsoft.EntityFrameworkCore;

namespace Financial.Infra
{
    public class DefaultContext : DbContext
    {
        public DefaultContext(DbContextOptions options) : base(options)
        {
            
        }

        public virtual DbSet<Financiallaunch> Financiallaunch { get; set; }

    }
}
