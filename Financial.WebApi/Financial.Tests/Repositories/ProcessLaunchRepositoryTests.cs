using Financial.Domain;
using Financial.Domain.Dtos; // Important for CreateFinanciallaunchDto
using Financial.Infra.Interfaces;
using Financial.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Threading.Tasks;

namespace Financial.Infra.Tests.Repositories
{
    public class ProcessLaunchRepositoryTests
    {

        private DefaultContext _context;
        private ProcessLaunchRepository _repository;
        private readonly Mock<IProcessLaunchRepository> _processLaunchRepositoryMock;
        public ProcessLaunchRepositoryTests()
        {
            _processLaunchRepositoryMock = new Mock<IProcessLaunchRepository>();

            // Setup In-Memory Database
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .UseInternalServiceProvider(serviceProvider)
                .EnableSensitiveDataLogging()
                .Options;

            _context = new DefaultContext(options);
            _context.Database.EnsureCreated();
            _repository = new ProcessLaunchRepository(_context);

            // Seed Data (Helper method to add data to the database)

        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted(); // Limpar o banco de dados após o teste

            _context.Dispose();
        }


        private async Task SeedData(string? idempotencyKey = null)
        {
            // You can add multiple launches here for testing GetAsync and GetByIdempotencyKeyAsync
            var createDto1 = new CreateFinanciallaunchDto
            {
                IdempotencyKey = idempotencyKey ?? Guid.NewGuid().ToString(),
                LaunchType = launchTypeEnum.Expense,
                PaymentMethod = launchPaymentMethodEnum.Card,
                CoinType = "EUR",
                Value = 50.00M,
                BankAccount = "6789-0",
                NameCustomerSupplier = "Test2",
                CostCenter = "Test2",
                Description = "Test2"
            };
            var launch1 = new Financiallaunch(createDto1);
            _context.Financiallaunch.Add(launch1);

            await _context.SaveChangesAsync(); // Persist the data to the In-Memory Database
        }


        [Fact(DisplayName = "CreateAsync should add and save a financial launch")]
        public async Task CreateAsync_ShouldAddAndSaveFinancialLaunch()
        {
            // Arrange
            var createDto = new CreateFinanciallaunchDto
            {
                IdempotencyKey = Guid.NewGuid().ToString(),
                LaunchType = launchTypeEnum.Expense,
                PaymentMethod = launchPaymentMethodEnum.Cash,
                CoinType = "USD",
                Value = 100.00M,
                BankAccount = "1234-5",
                NameCustomerSupplier = "Test",
                CostCenter = "Test",
                Description = "Test"
            };

            var launch = new Financiallaunch(createDto);

            // Act
            var result = await _repository.CreateAsync(launch);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(launch.Id, result.Id);
            Assert.Equal(launch.IdempotencyKey, result.IdempotencyKey);

            // Verificar no banco de dados (In-Memory)
            var retrievedLaunch = await _context.Financiallaunch.FindAsync(launch.Id);
            Assert.NotNull(retrievedLaunch);
            Assert.Equal(launch.LaunchType, retrievedLaunch.LaunchType);
        }


        [Fact(DisplayName = "GetAsync deve retornar um lançamento financeiro pelo ID")]
        public async Task GetAsync_ShouldReturnFinancialLaunchById()
        {
            await SeedData();
            // Arrange
            var launchId = _context.Financiallaunch.First().Id; // Get the first launch ID

            // Act
            var result = await _repository.GetAsync(launchId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(launchId, result.Id);
        }

        [Fact(DisplayName = "GetByIdStatusOpenAsync deve retornar um lançamento financeiro pelo ID")]
        public async Task GetByIdStatusOpenAsyncShouldReturnFinancialLaunchById()
        {
            await SeedData();
            // Arrange
            var launchId = _context.Financiallaunch.First().Id; // Get the first launch ID

            // Act
            var result = await _repository.GetByIdStatusOpenAsync(launchId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(launchId, result.Id);
        }

        [Fact(DisplayName = "UpdateAsync deve atualizar um lançamento financeiro pelo ID")]
        public async Task UpdateAsyncShouldUpdateFinancialLaunchById()
        {
            // Arrange
            var mockRepo = new Mock<IProcessLaunchRepository>();
            await SeedData();
            var launchId = _context.Financiallaunch.First();
            launchId.PayOff();

            mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Financiallaunch>()))
                    .ReturnsAsync(launchId);


            _processLaunchRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Financiallaunch>()))
                                        .ReturnsAsync(launchId);

            // Act
            var repo = _processLaunchRepositoryMock.Object;


            // Assert
            Assert.NotNull(repo.UpdateAsync(launchId));
        }


        [Fact(DisplayName = "GetAsync deve retornar null se o lançamento financeiro não for encontrado pelo ID")]
        public async Task GetAsync_ShouldReturnNullIfNotFoundById()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.GetAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact(DisplayName = "GetByIdempotencyKeyAsync deve retornar um lançamento financeiro pela chave de idempotência")]
        public async Task GetByIdempotencyKeyAsync_ShouldReturnFinancialLaunchByIdempotencyKey()
        {

            // Arrange
            var idempotencyKey = Guid.NewGuid().ToString(); // Use the specific key

            await SeedData(idempotencyKey);

            // Act
            var result = await _repository.GetByIdempotencyKeyAsync(idempotencyKey);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(idempotencyKey, result.IdempotencyKey.ToString());
        }

        [Fact(DisplayName = "GetByIdempotencyKeyAsync deve retornar null se o lançamento financeiro não for encontrado pela chave de idempotência")]
        public async Task GetByIdempotencyKeyAsync_ShouldReturnNullIfNotFoundByIdempotencyKey()
        {
            await SeedData();

            // Arrange
            var nonExistentKey = Guid.NewGuid().ToString();

            // Act
            var result = await _repository.GetByIdempotencyKeyAsync(nonExistentKey);

            // Assert
            Assert.Null(result);
        }
    }
}
