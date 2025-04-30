using Financial.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Financial.Infra.MapEF
{
    public class FinancialConfiguration : IEntityTypeConfiguration<Financiallaunch>
    {
        public void Configure(EntityTypeBuilder<Financiallaunch> builder)
        {
            builder.ToTable("Financiallaunch");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();


        }
    }
}
