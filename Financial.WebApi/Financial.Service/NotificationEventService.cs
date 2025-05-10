using Financial.Common;
using Financial.Domain.Dtos;
using Financial.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Financial.Service
{
    public class NotificationEventService : INotificationEvent
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationEventService> _logger;
        private readonly IConnectionFactoryWrapper _connectionFactoryWrapper;   
        public NotificationEventService(IConnectionFactoryWrapper connectionFactoryWrapper, IConfiguration configuration, ILogger<NotificationEventService> logger)
        {
            _connectionFactoryWrapper = connectionFactoryWrapper;
            _configuration = configuration;
            _logger = logger;

        }
        public async Task<bool> SendAsync(FinanciallaunchEvent financiallaunchEvent)
        {
            try
            {
                var config = GetConfig("QueueName", "RoutingKey");
                //var factory = GetConnectionFactory(config);
                // Cria uma conexão com o RabbitMQ
                using var connection = await _connectionFactoryWrapper.CreateConnectionAsync(config);   
                using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(queue: config.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                var jsonMessage = JsonSerializer.Serialize(financiallaunchEvent);

                var body = Encoding.UTF8.GetBytes(jsonMessage);

                _logger.LogInformation($"Publis channel: {body}");

                await channel.BasicPublishAsync(exchange: string.Empty, routingKey: config.RoutingKey, body: body);

                _logger.LogInformation($"Publis channel done");

                return true;
            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogError($"Error connecting to RabbitMQ: {ex.Message}");
                // Lógica para lidar com a falha de conexão (tentar novamente, logar o erro, etc.)
                throw ex;
            }    

        }
        public async Task<bool> SendCancelAsync(FinanciallaunchEvent financiallaunchEvent)
        {
            try
            {
                var config = GetConfig("QueueCancel", "RoutingKeyCancel");
                using var connection = await _connectionFactoryWrapper.CreateConnectionAsync(config);
                using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(queue: config.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                var jsonMessage = JsonSerializer.Serialize(financiallaunchEvent);

                var body = Encoding.UTF8.GetBytes(jsonMessage);

                _logger.LogInformation($"Publis channel: {body}");

                await channel.BasicPublishAsync(exchange: string.Empty, routingKey: config.RoutingKey, body: body);

                _logger.LogInformation($"Publis channel done");

                return true;

            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogError($"Error connecting to RabbitMQ: {ex.Message}");
                // Lógica para lidar com a falha de conexão (tentar novamente, logar o erro, etc.)
                throw ex;
            }     
        }

        public async Task<bool> SendPaidAsync(FinanciallaunchEvent financiallaunchEvent)
        {
            try
            {
  
                var config = GetConfig("QueuePaid", "RoutingKeyPaid");
                using var connection = await _connectionFactoryWrapper.CreateConnectionAsync(config);
                using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(queue: config.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                var jsonMessage = JsonSerializer.Serialize(financiallaunchEvent);

                var body = Encoding.UTF8.GetBytes(jsonMessage);

                _logger.LogInformation($"Publis channel: {body}");

                await channel.BasicPublishAsync(exchange: string.Empty, routingKey: config.RoutingKey, body: body);

                _logger.LogInformation($"Publis channel done");

                return true;

            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogError($"Error connecting to RabbitMQ: {ex.Message}");
                // Lógica para lidar com a falha de conexão (tentar novamente, logar o erro, etc.)
                throw ex;
            }         
        }

        [ExcludeFromCodeCoverage]
        private ConnectionQueueMenssage GetConfig(string queueName, string routingKeyPaid)
        {
            var config = new ConnectionQueueMenssage
            {
                HostName = _configuration["ConnectionQueueMenssage:HostName"],
                Port = int.Parse(_configuration["ConnectionQueueMenssage:Port"]),
                UserName = _configuration["ConnectionQueueMenssage:UserName"],
                Password = _configuration["ConnectionQueueMenssage:Password"],
                VirtualHost = _configuration["ConnectionQueueMenssage:VirtualHost"],
                QueueName = _configuration[$"ConnectionQueueMenssage:{queueName}"],
                ExchangeName = _configuration["ConnectionQueueMenssage:ExchangeName"],
                RoutingKey = _configuration[$"ConnectionQueueMenssage:{routingKeyPaid}"]
            };

            return config;
        }
    }

    [ExcludeFromCodeCoverage]
    public class ConnectionFactoryWrapper : IConnectionFactoryWrapper
    {
        public async Task<IConnection> CreateConnectionAsync(ConnectionQueueMenssage config)
        {
            var factory = new ConnectionFactory
            {
                HostName = config.HostName,
                Port = config.Port,
                UserName = config.UserName,
                Password = config.Password
            };
            return await factory.CreateConnectionAsync();
        }
    }

    
    public interface IConnectionFactoryWrapper
    {
        Task<IConnection> CreateConnectionAsync(ConnectionQueueMenssage config);
    }
}
