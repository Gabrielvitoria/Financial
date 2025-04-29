using Financial.Domain;
using Financial.Domain.Dtos;
using System.ComponentModel;

namespace Financial.Tests
{
    public class FinanciallaunchTest
    {
        [Fact]
        [Description("Deve Instanciar Financial launch")]
        public void DeveInstanciarFinanciallaunch()
        {
            var idempotencyKey = $"Financiallaunch_Revenue_Cash_100_453262_Nome novo de Customer_UNIQUESUFFIX";

            var financiallaunchDto = new CreateFinanciallaunchDto
            {
                IdempotencyKey = idempotencyKey,
                LaunchType = Domain.launchTypeEnum.Revenue,
                PaymentMethod = Domain.launchPaymentMethodEnum.Cash,
                CoinType = "USD",
                Value = 100,
                BankAccount = "453262",
                NameCustomerSupplier = "Nome novo de Customer",
                CostCenter = "10.3.456.0",
                Description = "Venda de Novo produto"

            };


            var entity = new Financiallaunch(financiallaunchDto);

            Assert.True(entity.IdempotencyKeyValid);
        }

        [Fact]
        [Description("Deve invalidar Instanciar Financial launch com mesmo IdempotencyKey")]
        public void DeveInvalidarInstanciarFinanciallaunchIdempotencyKey()
        {
            var idempotencyKey1 = $"Financiallaunch_Revenue_Cash_100_453262_Nome novo de Customer_UNIQUESUFFIX";

            var financiallaunchDto1 = new CreateFinanciallaunchDto
            {
                IdempotencyKey = idempotencyKey1,
                LaunchType = Domain.launchTypeEnum.Revenue,
                PaymentMethod = Domain.launchPaymentMethodEnum.Cash,
                CoinType = "USD",
                Value = 100,
                BankAccount = "453262",
                NameCustomerSupplier = "Nome novo de Customer",
                CostCenter = "10.3.456.0",
                Description = "Venda de Novo produto"

            };

            var idempotencyKey2 = $"Financiallaunch_Revenue_Cash_100_453262_Nome novo de Customer_UNIQUESUFFIX";

            var financiallaunchDto2 = new CreateFinanciallaunchDto
            {
                IdempotencyKey = idempotencyKey2,
                LaunchType = Domain.launchTypeEnum.Revenue,
                PaymentMethod = Domain.launchPaymentMethodEnum.Cash,
                CoinType = "USD",
                Value = 100.01m,
                BankAccount = "453262",
                NameCustomerSupplier = "Nome novo de Customer",
                CostCenter = "10.3.456.0",
                Description = "Venda de Novo produto"

            };


            var entity1 = new Financiallaunch(financiallaunchDto1);
            var entity2 = new Financiallaunch(financiallaunchDto2);


            Assert.True(entity1.GetIdempotencyKey != entity2.GetIdempotencyKey);
        }

        [Fact]
        [Description("Deve invalidar Instanciar Financial launch com mesmo IdempotencyKey 2")]
        public void DeveInvalidarInstanciarFinanciallaunchIdempotencyKey2()
        {
            var idempotencyKey1 = $"Financiallaunch_Revenue_Cash_10_453262_Nome novo de Customer_UNIQUESUFFIX";

            var financiallaunchDto1 = new CreateFinanciallaunchDto
            {
                IdempotencyKey = idempotencyKey1,
                LaunchType = Domain.launchTypeEnum.Revenue,
                PaymentMethod = Domain.launchPaymentMethodEnum.Cash,
                CoinType = "USD",
                Value = 100,
                BankAccount = "453262",
                NameCustomerSupplier = "Nome novo de Customer",
                CostCenter = "10.3.456.0",
                Description = "Venda de Novo produto"

            };

            var idempotencyKey2 = $"Financiallaunch_Revenue_Cash_100_453262_Nome novo de Customer_UNIQUESUFFIX";

            var financiallaunchDto2 = new CreateFinanciallaunchDto
            {
                IdempotencyKey = idempotencyKey2,
                LaunchType = Domain.launchTypeEnum.Revenue,
                PaymentMethod = Domain.launchPaymentMethodEnum.Cash,
                CoinType = "USD",
                Value = 100.01m,
                BankAccount = "453262",
                NameCustomerSupplier = "Nome novo de Customer",
                CostCenter = "10.3.456.0",
                Description = "Venda de Novo produto"

            };


            var entity1 = new Financiallaunch(financiallaunchDto1);
            var entity2 = new Financiallaunch(financiallaunchDto2);


            Assert.False(entity1.IdempotencyKeyValid);
            Assert.False(entity2.IdempotencyKeyValid);
        }
    }
}
