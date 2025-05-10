using Financial.Domain;
using Financial.Domain.Dtos;
using Financial.Infra.Interfaces;
using Financial.Service;
using Financial.Service.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.ComponentModel;
using System.Diagnostics;

namespace Financial.Tests.Services
{
    public class FinanciallaunchTest
    {
        private readonly Mock<IProcessLaunchRepository> _processLaunchRepositoryMock;
        private readonly Mock<INotificationEvent> _INotificationEvent;
        private readonly Mock<ILogger<ProcessLaunchservice>> _logger;
        private readonly ProcessLaunchservice _processLaunchservice;

        public FinanciallaunchTest()
        {
            _processLaunchRepositoryMock = new Mock<IProcessLaunchRepository>();
            _INotificationEvent = new Mock<INotificationEvent>();
            _logger = new Mock<ILogger<ProcessLaunchservice>>();
            _processLaunchservice = new ProcessLaunchservice(_processLaunchRepositoryMock.Object, _INotificationEvent.Object, _logger.Object);
        }


        [Fact]
        [Description("Deve Instanciar Financial launch")]
        public void DeveInstanciarFinanciallaunch()
        {
            var idempotencyKey = Guid.NewGuid().ToString();

            var financiallaunchDto = new CreateFinanciallaunchDto
            {
                IdempotencyKey = idempotencyKey,
                LaunchType = launchTypeEnum.Revenue,
                PaymentMethod = launchPaymentMethodEnum.Cash,
                CoinType = "USD",
                Value = 100,
                BankAccount = "453262",
                NameCustomerSupplier = "Nome novo de Customer",
                CostCenter = "10.3.456.0",
                Description = "Venda de Novo produto"

            };

            var entity = new Financiallaunch(financiallaunchDto);

            Assert.True(entity.IdempotencyKey != Guid.Empty);
        }

        [Fact]
        [Description("Deve invalidar Instanciar Financial launch com mesmo IdempotencyKey")]
        public void DeveInvalidarInstanciarFinanciallaunchIdempotencyKey()
        {
            var idempotencyKey1 = Guid.NewGuid().ToString();

            var financiallaunchDto1 = new CreateFinanciallaunchDto
            {
                IdempotencyKey = idempotencyKey1,
                LaunchType = launchTypeEnum.Revenue,
                PaymentMethod = launchPaymentMethodEnum.Cash,
                CoinType = "USD",
                Value = 100,
                BankAccount = "453262",
                NameCustomerSupplier = "Nome novo de Customer",
                CostCenter = "10.3.456.0",
                Description = "Venda de Novo produto"

            };

            var idempotencyKey2 = Guid.NewGuid().ToString();

            var financiallaunchDto2 = new CreateFinanciallaunchDto
            {
                IdempotencyKey = idempotencyKey2,
                LaunchType = launchTypeEnum.Revenue,
                PaymentMethod = launchPaymentMethodEnum.Cash,
                CoinType = "USD",
                Value = 100.01m,
                BankAccount = "453262",
                NameCustomerSupplier = "Nome novo de Customer",
                CostCenter = "10.3.456.0",
                Description = "Venda de Novo produto"

            };


            var entity1 = new Financiallaunch(financiallaunchDto1);
            var entity2 = new Financiallaunch(financiallaunchDto2);


            Assert.True(entity1.IdempotencyKey != entity2.IdempotencyKey);
        }

        [Fact]
        [Description("Deve invalidar Instanciar Financial launch com mesmo IdempotencyKey 2")]
        public void DeveInvalidarInstanciarFinanciallaunchIdempotencyKey2()
        {
            var idempotencyKey1 = $"Financiallaunch_Revenue_Cash_10_453262_Nome novo de Customer_UNIQUESUFFIX";

            var financiallaunchDto1 = new CreateFinanciallaunchDto
            {
                IdempotencyKey = idempotencyKey1,
                LaunchType = launchTypeEnum.Revenue,
                PaymentMethod = launchPaymentMethodEnum.Cash,
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
                LaunchType = launchTypeEnum.Revenue,
                PaymentMethod = launchPaymentMethodEnum.Cash,
                CoinType = "USD",
                Value = 100.01m,
                BankAccount = "453262",
                NameCustomerSupplier = "Nome novo de Customer",
                CostCenter = "10.3.456.0",
                Description = "Venda de Novo produto"

            };


            var entity1 = new Financiallaunch(financiallaunchDto1);
            var entity2 = new Financiallaunch(financiallaunchDto2);


            Assert.False(financiallaunchDto2.IdempotencyKeyValid);
            Assert.False(financiallaunchDto2.IdempotencyKeyValid);
            Assert.True(entity1.IdempotencyKey == Guid.Empty);
            Assert.True(entity2.IdempotencyKey == Guid.Empty);
        }

        [Fact]
        [Description("Deve processar a criação de um novo lançamento")]
        public async Task DeveProcessarAcriacaoDeUmNovolancamento()
        {
            // Arrange
            var idempotencyKey = Guid.NewGuid().ToString();

            var financiallaunchDto = new CreateFinanciallaunchDto
            {
                IdempotencyKey = idempotencyKey,
                LaunchType = launchTypeEnum.Revenue,
                PaymentMethod = launchPaymentMethodEnum.Cash,
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
            Assert.Equal(financialLaunch.IdempotencyKey.ToString(), result.IdempotencyKey);
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


        [Fact]
        [Description("Deve processar o cancelamento de um lançamento existente com status 'Open'")]
        public async Task DeveProcessarOCancelamentoDeUmLancamentoExistenteComStatusOpen()
        {
            // Arrange
            var cancelDescription = "Cancelamento de teste";
            var existingLaunchDto = new CreateFinanciallaunchDto
            {
                IdempotencyKey = Guid.NewGuid().ToString(),
                LaunchType = launchTypeEnum.Revenue,
                PaymentMethod = launchPaymentMethodEnum.Card,
                CoinType = "BRL",
                Value = 150.00m,
                BankAccount = "1234-5",
                NameCustomerSupplier = "Cliente Teste",
                CostCenter = "Vendas",
                Description = "Lançamento inicial"
            };
            var existingLaunch = new Financiallaunch(existingLaunchDto);

            var expectedDto = new FinanciallaunchDto
            {
                Id = existingLaunch.Id,
                Value = 150.00m,
                Description = "Lançamento inicial",
                Status = launchStatusEnum.Canceled,
                IdempotencyKey = existingLaunch.IdempotencyKey.ToString()
            };
            var cancelDto = new CancelFinanciallaunchDto { Id = existingLaunch.Id, Description = cancelDescription };

            _processLaunchRepositoryMock.Setup(repo => repo.GetByIdStatusOpenAsync(existingLaunch.Id)).ReturnsAsync(existingLaunch); // Simula que não existe um lançamento já cancelado com esse ID
           
            _processLaunchRepositoryMock.Setup(repo => repo.GetAsync(existingLaunch.Id)).ReturnsAsync(existingLaunch);
            
            _processLaunchRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Financiallaunch>()))
                                        .ReturnsAsync(existingLaunch);

            // Act
            var result = await _processLaunchservice.ProcessCancelLaunchAsync(cancelDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.Id, result.Id);
            Assert.Equal(expectedDto.Status, result.Status);
            Assert.Contains("Cancel", result.Description);
            Assert.Equal(expectedDto.IdempotencyKey, result.IdempotencyKey);

            _processLaunchRepositoryMock.Verify(repo => repo.GetByIdStatusOpenAsync(existingLaunch.Id), Times.Once);
            _processLaunchRepositoryMock.Verify(repo => repo.GetByIdStatusOpenAsync(existingLaunch.Id), Times.Once);
            _processLaunchRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Financiallaunch>(l =>
                l.Id == existingLaunch.Id &&
                l.Status == launchStatusEnum.Canceled
            )), Times.Once);
        }

        [Fact]
        [Description("Não Deve processar o cancelamento de um lançamento com ID vazio")]
        public async Task NaoDeveProcessarOCancelamentoDeUmLancamentoComIdVazio()
        {
            // Arrange
            var cancelDto = new CancelFinanciallaunchDto { Id = Guid.Empty, Description = "Cancelamento de teste" };

            // Act
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _processLaunchservice.ProcessCancelLaunchAsync(cancelDto));

            // Assert
            Assert.Equal("Error: Check if the data is correct. Some information that makes up the Id is incorrect or does not match the ID", exception.Message);

            _processLaunchRepositoryMock.Verify(repo => repo.GetByIdStatusOpenAsync(It.IsAny<Guid>()), Times.Never);
            _processLaunchRepositoryMock.Verify(repo => repo.GetAsync(It.IsAny<Guid>()), Times.Never);
            _processLaunchRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Financiallaunch>()), Times.Never);
        }

        [Fact]
        [Description("Não Deve processar o cancelamento de um lançamento já cancelado")]
        public async Task NaoDeveProcessarOCancelamentoDeUmLancamentoJaCancelado()
        {
            // Arrange
  
            var cancelDescription = "Tentativa de cancelamento";
            var cancelDto = new CancelFinanciallaunchDto
            {
                Id = Guid.NewGuid(),
                Description = "Lançamento original",
            };

            Financiallaunch existingLaunch; 

            _processLaunchRepositoryMock.Setup(repo => repo.GetByIdStatusOpenAsync(It.IsAny<Guid>())).ReturnsAsync((Financiallaunch)null);

            // Act
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _processLaunchservice.ProcessCancelLaunchAsync(cancelDto));

            // Assert
            Assert.Equal("Info: The release cannot be canceled. Status other than \"Open\"", exception.Message);

            _processLaunchRepositoryMock.Verify(repo => repo.GetByIdStatusOpenAsync(It.IsAny<Guid>()), Times.Once);
            _processLaunchRepositoryMock.Verify(repo => repo.GetAsync(It.IsAny<Guid>()), Times.Never);
            _processLaunchRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Financiallaunch>()), Times.Never);
        }

        [Fact]
        [Description("Não Deve processar o cancelamento de um lançamento inexistente")]
        public async Task NaoDeveProcessarOCancelamentoDeUmLancamentoInexistente()
        {
            // Arrange
            var launchId = Guid.NewGuid();
            var cancelDto = new CancelFinanciallaunchDto { Id = launchId, Description = "Cancelamento de teste" };

            _processLaunchRepositoryMock.Setup(repo => repo.GetByIdStatusOpenAsync(launchId)).ReturnsAsync((Financiallaunch)null);
            _processLaunchRepositoryMock.Setup(repo => repo.GetAsync(launchId)).ReturnsAsync((Financiallaunch)null);

            // Act
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _processLaunchservice.ProcessCancelLaunchAsync(cancelDto));

            // Assert
            _processLaunchRepositoryMock.Verify(repo => repo.GetByIdStatusOpenAsync(launchId), Times.Once);
            _processLaunchRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Financiallaunch>()), Times.Never);
        }


        [Fact]
        [Description("Deve processar o pagamento de um lançamento existente com status 'Open' e enviar notificação")]
        public async Task DeveProcessarOPagamentoDeUmLancamentoExistenteComStatusOpenEEnviarNotificacao()
        {
            // Arrange

            var existingLaunchDto = new CreateFinanciallaunchDto
            {
                IdempotencyKey = Guid.NewGuid().ToString().ToUpper(),
                LaunchType = launchTypeEnum.Revenue,
                PaymentMethod = launchPaymentMethodEnum.Card,
                CoinType = "BRL",
                Value = 150.00m,
                BankAccount = "1234-5",
                NameCustomerSupplier = "Cliente Teste",
                CostCenter = "Vendas",
                Description = "Lançamento inicial"
            };
            var existingLaunch = new Financiallaunch(existingLaunchDto);

            var expectedDto = new FinanciallaunchDto
            {
                Id = existingLaunch.Id,
                Value = 150.00m,
                Description = "Lançamento inicial Paid off.",
                Status = launchStatusEnum.PaidOff,
                IdempotencyKey = existingLaunch.IdempotencyKey.ToString()
            };
            var payDto = new PayFinanciallaunchDto { Id = existingLaunch.Id };

            _processLaunchRepositoryMock.Setup(repo => repo.GetByIdStatusOpenAsync(existingLaunch.Id)).ReturnsAsync(existingLaunch);
            _processLaunchRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Financiallaunch>()))
                                        .ReturnsAsync(existingLaunch);

            // Act
            var result = await _processLaunchservice.ProcessPayLaunchAsync(payDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.Id, result.Id);
            Assert.Equal(expectedDto.Status, result.Status);
            Assert.Contains("Paid", result.Description);
            Assert.Equal(expectedDto.IdempotencyKey, result.IdempotencyKey);

            _processLaunchRepositoryMock.Verify(repo => repo.GetByIdStatusOpenAsync(existingLaunch.Id), Times.Once);
            _processLaunchRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Financiallaunch>(l =>
                l.Id == existingLaunch.Id &&
                l.Status == launchStatusEnum.PaidOff
            )), Times.Once);

        }

        [Fact]
        [Description("Não Deve processar o pagamento de um lançamento com ID vazio e não enviar notificação")]
        public async Task NaoDeveProcessarOPagamentoDeUmLancamentoComIdVazioENaoEnviarNotificacao()
        {
            // Arrange
            var payDto = new PayFinanciallaunchDto { Id = Guid.Empty };

            // Act
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _processLaunchservice.ProcessPayLaunchAsync(payDto));

            // Assert
            Assert.Equal("Error: Check if the data is correct. Some information that makes up the Id is incorrect or does not match the ID", exception.Message);

            _processLaunchRepositoryMock.Verify(repo => repo.GetByIdStatusOpenAsync(It.IsAny<Guid>()), Times.Never);
            _processLaunchRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Financiallaunch>()), Times.Never);
            _INotificationEvent.Verify(notify => notify.SendAsync(It.IsAny<FinanciallaunchEvent>()), Times.Never);
        }

        [Fact]
        [Description("Não Deve processar o pagamento de um lançamento com status diferente de 'Open' e não enviar notificação")]
        public async Task NaoDeveProcessarOPagamentoDeUmLancamentoComStatusDiferenteDeOpenENaoEnviarNotificacao()
        {
            // Arrange
            var launchId = Guid.NewGuid();
            var existingLaunchDto = new CreateFinanciallaunchDto
            {
                IdempotencyKey = Guid.NewGuid().ToString().ToUpper(),
                LaunchType = launchTypeEnum.Revenue,
                PaymentMethod = launchPaymentMethodEnum.Card,
                CoinType = "BRL",
                Value = 150.00m,
                BankAccount = "1234-5",
                NameCustomerSupplier = "Cliente Teste",
                CostCenter = "Vendas",
                Description = "Lançamento inicial",
            };
            var existingLaunch = new Financiallaunch(existingLaunchDto);
            var payDto = new PayFinanciallaunchDto { Id = launchId };

            _processLaunchRepositoryMock.Setup(repo => repo.GetByIdStatusOpenAsync(launchId)).ReturnsAsync((Financiallaunch)null);

            // Act
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _processLaunchservice.ProcessPayLaunchAsync(payDto));

            // Assert
            Assert.Equal("Info: The release cannot be canceled. Status other than \"Open\"", exception.Message);

            _processLaunchRepositoryMock.Verify(repo => repo.GetByIdStatusOpenAsync(launchId), Times.Once);
            _processLaunchRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Financiallaunch>()), Times.Never);
            _INotificationEvent.Verify(notify => notify.SendAsync(It.IsAny<FinanciallaunchEvent>()), Times.Never);
        }

        [Fact]
        [Description("Não Deve processar o pagamento de um lançamento inexistente e não enviar notificação")]
        public async Task NaoDeveProcessarOPagamentoDeUmLancamentoInexistenteENaoEnviarNotificacao()
        {
            // Arrange
            var launchId = Guid.NewGuid();
            var payDto = new PayFinanciallaunchDto { Id = launchId };

            _processLaunchRepositoryMock.Setup(repo => repo.GetByIdStatusOpenAsync(launchId)).ReturnsAsync((Financiallaunch)null);

            // Act
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _processLaunchservice.ProcessPayLaunchAsync(payDto));

            // Assert
            Assert.Equal("Info: The release cannot be canceled. Status other than \"Open\"", exception.Message);

            _processLaunchRepositoryMock.Verify(repo => repo.GetByIdStatusOpenAsync(launchId), Times.Once);
            _processLaunchRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Financiallaunch>()), Times.Never);
            _INotificationEvent.Verify(notify => notify.SendAsync(It.IsAny<FinanciallaunchEvent>()), Times.Never);
        }
    }
}


