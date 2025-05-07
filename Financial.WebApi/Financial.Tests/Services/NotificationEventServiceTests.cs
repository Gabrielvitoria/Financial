using Financial.Common;
using Financial.Domain.Dtos;
using Financial.Service;
using Microsoft.EntityFrameworkCore.Metadata;
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

            _connectionMock.Setup(connection => connection.CreateChannelAsync(null,default))
                .ReturnsAsync(_channelMock.Object);

            _connectionFactoryMock.Setup(factory => factory.CreateConnectionAsync(It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(_connectionMock.Object);

            _notificationEventService = new NotificationEventService(_connectionFactoryWrapperMock.Object,_configurationMock.Object, _loggerMock.Object);
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

    }
}