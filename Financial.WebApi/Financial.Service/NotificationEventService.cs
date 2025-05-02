using Financial.Common;
using Financial.Domain.Dtos;
using Financial.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Financial.Service
{
    public class NotificationEventService : INotificationEvent
    {
        private readonly IConfiguration _configuration;
        public NotificationEventService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendAsync(FinanciallaunchEvent financiallaunchEvent)
        {
            try
            {
                var config = GetConfig();
                var factory = GetConnectionFactory(config);
                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(queue: config.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                var jsonMessage = JsonSerializer.Serialize(financiallaunchEvent);

                var body = Encoding.UTF8.GetBytes(jsonMessage);

                await channel.BasicPublishAsync(exchange: string.Empty, routingKey: config.RoutingKey, body: body);

            }
            catch (BrokerUnreachableException ex)
            {
                Console.WriteLine($"Error connecting to RabbitMQ: {ex.Message}");
                // Lógica para lidar com a falha de conexão (tentar novamente, logar o erro, etc.)
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                // Lógica para lidar com outros tipos de erros
            }

        }

        private ConnectionFactory GetConnectionFactory(ConnectionQueueMenssage config)
        {
      
            var factory = new ConnectionFactory
            {
                HostName = config.HostName,
                Port = config.Port,
                UserName = config.UserName,
                Password = config.Password
            };

            return factory;
        }

        private ConnectionQueueMenssage GetConfig()
        {
            var config = new ConnectionQueueMenssage
            {
                HostName = _configuration["ConnectionQueueMenssage:HostName"],
                Port = int.Parse(_configuration["ConnectionQueueMenssage:Port"]),
                UserName = _configuration["ConnectionQueueMenssage:UserName"],
                Password = _configuration["ConnectionQueueMenssage:Password"],
                VirtualHost = _configuration["ConnectionQueueMenssage:VirtualHost"],
                QueueName = _configuration["ConnectionQueueMenssage:QueueName"],
                ExchangeName = _configuration["ConnectionQueueMenssage:ExchangeName"],
                RoutingKey = _configuration["ConnectionQueueMenssage:RoutingKey"]
            };

            return config;
        }
    }
}
