using Financial.Domain;
using Financial.Domain.Dtos;
using Financial.Infra.Interfaces;
using Financial.Service;
using Moq;
using System.ComponentModel;
using System.Diagnostics;

namespace Financial.Tests
{
    public class FinanciallaunchTest
    {
        private readonly Mock<IProcessLaunchRepository> _processLaunchRepositoryMock;
        private readonly ProcessLaunchservice _processLaunchservice;

        public FinanciallaunchTest()
        {
            _processLaunchRepositoryMock = new Mock<IProcessLaunchRepository>();
            _processLaunchservice = new ProcessLaunchservice(_processLaunchRepositoryMock.Object);
        }


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

        [Fact]
        [Description("Deve processar a criação de um novo lançamento")]
        public async Task DeveProcessarAcriacaoDeUmNovolancamento()
        {
            // Arrange
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

            var financialLaunch = new Financiallaunch(financiallaunchDto);

            _processLaunchRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<Financiallaunch>()))
                .ReturnsAsync(financialLaunch);

            // Act
            var result = await _processLaunchservice.ProcessNewLaunchAsync(financiallaunchDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(financialLaunch.Id, result.Id);
            Assert.Equal(financialLaunch.IdempotencyKey, result.IdempotencyKey);
            Assert.Equal(financialLaunch.LaunchType, result.LaunchType);
            Assert.Equal(financialLaunch.PaymentMethod, result.PaymentMethod);
            Assert.Equal(financialLaunch.CoinType, result.CoinType);
            Assert.Equal(financialLaunch.Value, result.Value);
            Assert.Equal(financialLaunch.BankAccount, result.BankAccount);
            Assert.Equal(financialLaunch.NameCustomerSupplier, result.NameCustomerSupplier);
            Assert.Equal(financialLaunch.CostCenter, result.CostCenter);
            Assert.Equal(financialLaunch.Description, result.Description);

            _processLaunchRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Financiallaunch>()), Times.Once);
        }

        [Fact]
        [Description("Não Deve processar a criação de um novo lançamento")]
        public async Task NaoDeveProcessarAcriacaoDeUmNovolancamento()
        {
            // Arrange
            var createDto = new CreateFinanciallaunchDto
            {
                IdempotencyKey = "invalid-key",
                LaunchType = launchTypeEnum.Revenue,
                PaymentMethod = launchPaymentMethodEnum.Cash,
                CoinType = "USD",
                Value = 100.00m,
                BankAccount = "12345",
                NameCustomerSupplier = "John Doe",
                CostCenter = "Sales",
                Description = "Test description"
            };

            var financialLaunch = new Financiallaunch(createDto);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(() =>
                _processLaunchservice.ProcessNewLaunchAsync(createDto));

            Assert.Equal("Error: Check if the data is correct. Some information that makes up the Idempotency is incorrect or does not match the idempotency", exception.Message);

            _processLaunchRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Financiallaunch>()), Times.Never);
        }
    }
}
