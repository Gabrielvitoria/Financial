using Financial.Common;
using Financial.Domain.Dtos;
using Financial.Service;
using Financial.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace Financial.Tests.Services
{
    public class NotificationEventServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<NotificationEventService>> _loggerMock;
        private readonly Mock<IConnectionFactory> _connectionFactoryMock;
        private readonly Mock<IConnection> _connectionMock;
        private readonly Mock<IChannel> _channelMock;
        private readonly NotificationEventService _notificationEventService;
        private readonly Mock<IConnectionFactoryWrapper> _connectionFactoryWrapperMock;

        public NotificationEventServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<NotificationEventService>>();
            _connectionFactoryMock = new Mock<IConnectionFactory>();
            _connectionMock = new Mock<IConnection>();
            _channelMock = new Mock<IChannel>();
            _connectionFactoryWrapperMock = new Mock<IConnectionFactoryWrapper>();

            // Configurar o comportamento padrão para simular uma conexão bem-sucedida
            _configurationMock.Setup(config => config["ConnectionQueueMenssage:HostName"]).Returns("localhost");
            _configurationMock.Setup(config => config["ConnectionQueueMenssage:Port"]).Returns("5672");
            _configurationMock.Setup(config => config["ConnectionQueueMenssage:UserName"]).Returns("guest");
            _configurationMock.Setup(config => config["ConnectionQueueMenssage:Password"]).Returns("guest");
            _configurationMock.Setup(config => config["ConnectionQueueMenssage:QueueName"]).Returns("financial_launches");
            _configurationMock.Setup(config => config["ConnectionQueueMenssage:QueueCancel"]).Returns("financial_launches_cancel");
            _configurationMock.Setup(config => config["ConnectionQueueMenssage:QueuePaid"]).Returns("financial_launches_paid");
            _configurationMock.Setup(config => config["ConnectionQueueMenssage:RoutingKey"]).Returns("");


            // Configurar o comportamento do mock da IConnectionFactoryWrapper
            _connectionFactoryWrapperMock.Setup(wrapper => wrapper.CreateConnectionAsync(It.IsAny<ConnectionQueueMenssage>()))
                .ReturnsAsync(_connectionMock.Object);

            _connectionMock.Setup(connection => connection.CreateChannelAsync(null, default))
                .ReturnsAsync(_channelMock.Object);

            _connectionFactoryMock.Setup(factory => factory.CreateConnectionAsync(It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(_connectionMock.Object);

            _notificationEventService = new NotificationEventService(_connectionFactoryWrapperMock.Object, _configurationMock.Object, _loggerMock.Object);
        }

        [Fact]
        [Description("Deve enviar uma mensagem para a fila padrão com sucesso")]
        public async Task SendAsync_Success()
        {
            // Arrange
            var financiallaunchEvent = new FinanciallaunchEvent(new Financial.Domain.Financiallaunch());
            var expectedQueueName = "financial_launches";
            byte[] expectedBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(financiallaunchEvent));


            // Act
            Assert.True(await _notificationEventService.SendAsync(financiallaunchEvent));

        }


        [Fact]
        [Description("Deve enviar uma mensagem para a fila de pagamento")]
        public async Task SendPaidAsync_Success()
        {
            // Arrange
            var financiallaunchEvent = new FinanciallaunchEvent(new Financial.Domain.Financiallaunch());
            var expectedQueueName = "financial_launches";
            byte[] expectedBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(financiallaunchEvent));


            // Act
            Assert.True(await _notificationEventService.SendPaidAsync(financiallaunchEvent));

        }


        [Fact]
        [Description("Deve enviar uma mensagem para a fila de cancelar pagamento")]
        public async Task SendCancelAsync_Success()
        {
            // Arrange
            var financiallaunchEvent = new FinanciallaunchEvent(new Financial.Domain.Financiallaunch());
            var expectedQueueName = "financial_launches";
            byte[] expectedBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(financiallaunchEvent));


            // Act
            Assert.True(await _notificationEventService.SendCancelAsync(financiallaunchEvent));

        }

        [Fact]
        [Description("Deve lançar BrokerUnreachableException ao falhar a criação da conexão para fila de pagamento")]
        public async Task SendPaidAsync_BrokerUnreachableException()
        {
            // Arrange
            var financiallaunchEvent = new FinanciallaunchEvent(new Financial.Domain.Financiallaunch());
            var expectedQueueName = "financial_launches_paid"; // Supondo que GetConfig retorne isso para "QueuePaid"

            // Configurar o mock para lançar BrokerUnreachableException ao tentar criar a conexão
            _connectionFactoryWrapperMock.Setup(cfw => cfw.CreateConnectionAsync(It.IsAny<ConnectionQueueMenssage>()))
                                         .ThrowsAsync(new BrokerUnreachableException(new Exception("Could not connect to RabbitMQ")));

          
            // Act & Assert
            await Assert.ThrowsAsync<BrokerUnreachableException>(() => _notificationEventService.SendPaidAsync(financiallaunchEvent));

            // Assert (Verificar se outras interações não ocorreram, se relevante)
            _connectionFactoryWrapperMock.Verify(cfw => cfw.CreateConnectionAsync(It.IsAny<ConnectionQueueMenssage>()), Times.Once);
            _loggerMock.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().StartsWith("Error connecting to RabbitMQ:", StringComparison.OrdinalIgnoreCase)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        [Description("Deve lançar BrokerUnreachableException ao falhar a criação da conexão para fila de lançamento")]
        public async Task TaskSendAsync_BrokerUnreachableExceptionAsync()
        {
            // Arrange
            var financiallaunchEvent = new FinanciallaunchEvent(new Financial.Domain.Financiallaunch());
            var expectedQueueName = "financial_launches_paid"; // Supondo que GetConfig retorne isso para "QueuePaid"

            // Configurar o mock para lançar BrokerUnreachableException ao tentar criar a conexão
            _connectionFactoryWrapperMock.Setup(cfw => cfw.CreateConnectionAsync(It.IsAny<ConnectionQueueMenssage>()))
                                         .ThrowsAsync(new BrokerUnreachableException(new Exception("Could not connect to RabbitMQ")));


            // Act & Assert
            await Assert.ThrowsAsync<BrokerUnreachableException>(() => _notificationEventService.SendAsync(financiallaunchEvent));

            // Assert (Verificar se outras interações não ocorreram, se relevante)
            _connectionFactoryWrapperMock.Verify(cfw => cfw.CreateConnectionAsync(It.IsAny<ConnectionQueueMenssage>()), Times.Once);
            _loggerMock.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().StartsWith("Error connecting to RabbitMQ:", StringComparison.OrdinalIgnoreCase)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        [Description("Deve lançar BrokerUnreachableException ao falhar a criação da conexão para fila de cancelar lançamento")]
        public async Task SendCancelAsync_BrokerUnreachableExceptionAsync()
        {
            // Arrange
            var financiallaunchEvent = new FinanciallaunchEvent(new Financial.Domain.Financiallaunch());
            var expectedQueueName = "financial_launches_paid"; // Supondo que GetConfig retorne isso para "QueuePaid"

            // Configurar o mock para lançar BrokerUnreachableException ao tentar criar a conexão
            _connectionFactoryWrapperMock.Setup(cfw => cfw.CreateConnectionAsync(It.IsAny<ConnectionQueueMenssage>()))
                                         .ThrowsAsync(new BrokerUnreachableException(new Exception("Could not connect to RabbitMQ")));


            // Act & Assert
            await Assert.ThrowsAsync<BrokerUnreachableException>(() => _notificationEventService.SendCancelAsync(financiallaunchEvent));

            // Assert (Verificar se outras interações não ocorreram, se relevante)
            _connectionFactoryWrapperMock.Verify(cfw => cfw.CreateConnectionAsync(It.IsAny<ConnectionQueueMenssage>()), Times.Once);
            _loggerMock.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().StartsWith("Error connecting to RabbitMQ:", StringComparison.OrdinalIgnoreCase)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

    }


}