using Financial.Common;
using Xunit;

namespace Financial.Common.Tests
{
    public class ConnectionQueueMenssageTests
    {
        [Fact(DisplayName = "Deve atribuir e recuperar valores corretamente das propriedades")]
        public void Should_SetAndGetPropertiesCorrectly()
        {
            // Arrange
            var config = new ConnectionQueueMenssage();
            string expectedHostName = "localhost";
            int expectedPort = 5672;
            string expectedUserName = "guest";
            string expectedPassword = "password123";
            string expectedVirtualHost = "/";
            string expectedQueueName = "test-queue";
            string expectedExchangeName = "test-exchange";
            string expectedRoutingKey = "test.route";

            // Act
            config.HostName = expectedHostName;
            config.Port = expectedPort;
            config.UserName = expectedUserName;
            config.Password = expectedPassword;
            config.VirtualHost = expectedVirtualHost;
            config.QueueName = expectedQueueName;
            config.ExchangeName = expectedExchangeName;
            config.RoutingKey = expectedRoutingKey;

            // Assert
            Assert.Equal(expectedHostName, config.HostName);
            Assert.Equal(expectedPort, config.Port);
            Assert.Equal(expectedUserName, config.UserName);
            Assert.Equal(expectedPassword, config.Password);
            Assert.Equal(expectedVirtualHost, config.VirtualHost);
            Assert.Equal(expectedQueueName, config.QueueName);
            Assert.Equal(expectedExchangeName, config.ExchangeName);
            Assert.Equal(expectedRoutingKey, config.RoutingKey);
        }
    }
}