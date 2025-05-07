using Financial.Common;
using Financial.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Financial.Service.Works
{
    public class FinancialPayLaunchBackgroundService : BackgroundService
    {
        private readonly IFinanciallaunchService _financiallaunchService;
        private readonly ILogger<FinancialPayLaunchBackgroundService> _logger;
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IChannel _channel;

        public FinancialPayLaunchBackgroundService(IFinanciallaunchService financiallaunchService, ILogger<FinancialPayLaunchBackgroundService> logger, IConfiguration configuration)
        {
            _financiallaunchService = financiallaunchService;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await InitializeRabbitMQConnectionAsync();

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var jsonMessage = Encoding.UTF8.GetString(body);
                    _logger.LogInformation($"Received message: {jsonMessage}");

                    var receivedEvent = JsonSerializer.Deserialize<FinanciallaunchEvent>(jsonMessage);

                    _logger.LogInformation($"Processing event: {receivedEvent}", receivedEvent);

                    await _financiallaunchService.ProcessesPayFinancialLauchAsync(receivedEvent);

                    await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false); // Acknowledge the message
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error processing message: {ex.Message}");
                    await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true); // Nack and requeue
                }
            };

            _logger.LogInformation($"ConnectionQueueMenssage:QueuePaid: {_configuration["ConnectionQueueMenssage:QueuePaid"]}");

            await _channel.BasicConsumeAsync(queue: _configuration["ConnectionQueueMenssage:QueuePaid"], autoAck: false, consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker Queue Paid running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken); // Small delay to avoid busy-waiting
            }
        }

        public override void Dispose()
        {
            if (_channel != null)
                _channel.CloseAsync();
            if (_connection != null)
                _connection.CloseAsync();
            base.Dispose();
        }


        private async Task InitializeRabbitMQConnectionAsync()
        {
            try
            {

                var config = GetConfig();
                var factory = GetConnectionFactory(config);
                var connection = await factory.CreateConnectionAsync();
                var channel = await connection.CreateChannelAsync();

                _connection = connection;
                _channel = channel;

                await _channel.QueueDeclareAsync(queue: config.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                _logger.LogInformation("RabbitMQ Connection and Channel are ready.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not create connection. Error: {ex.Message}");
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
                QueueName = _configuration["ConnectionQueueMenssage:QueuePaid"],
                ExchangeName = _configuration["ConnectionQueueMenssage:ExchangeName"],
                RoutingKey = _configuration["ConnectionQueueMenssage:RoutingKeyPaid"]
            };

            return config;
        }
    }
}
