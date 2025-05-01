using Financial.Domain.Dtos;
using Financial.Service.Interfaces;
using RabbitMQ.Client;
using System.Text;

namespace Financial.Service
{
    public class NotificationEventService : INotificationEvent
    {

        public async Task SendAsync(FinanciallaunchEvent financiallaunchEvent)
        {

            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();


            await channel.QueueDeclareAsync(queue: "hello", durable: false, exclusive: false, autoDelete: false,arguments: null);

            const string message = "Hello World!";
            var body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "hello", body: body);
            Console.WriteLine($" [x] Sent {message}");

        }
    }
}
