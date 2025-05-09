using Financial.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Financial.Infra.MapEF
{
    [ExcludeFromCodeCoverage]
    public class FinancialConfiguration : IEntityTypeConfiguration<Financiallaunch>
    {
        public void Configure(EntityTypeBuilder<Financiallaunch> builder)
        {
            builder.ToTable("Financiallaunch");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                   .HasColumnName("id")
                   .ValueGeneratedOnAdd();

            builder.Property(a => a.IdempotencyKey)
                    .IsRequired();

            builder.Property(a=> a.LaunchType)
                   .IsRequired();

            builder.Property(a => a.PaymentMethod)
                   .IsRequired();

            builder.Property(a => a.Status)
                   .IsRequired();

            builder.Property(a => a.CoinType)
                   .IsRequired();

            builder.Property(a => a.Value)
                   .IsRequired();

            builder.Property(a => a.Value)
                   .IsRequired();

            builder.Property(a => a.NameCustomerSupplier)
                   .IsRequired();

            builder.Property(a => a.CostCenter)
                   .IsRequired();

            builder.HasIndex(a => a.IdempotencyKey)
                   .IsUnique()
                   .HasDatabaseName("IX_Financiallaunch_IdempotencyKey");

            builder.HasIndex(a => a.Status)
                   .HasDatabaseName("IX_Financiallaunch_Status");
        }
    }
}
