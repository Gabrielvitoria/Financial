using Financial.Domain.Dtos;
using Financial.Service.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Financial.Service
{
    public class NotificationEventService : INotificationEvent
    {

        public async Task SendAsync(FinanciallaunchEvent financiallaunchEvent)
        {
            try
            {

                var factory = new ConnectionFactory
                {
                    HostName = "localhost", // Alteração aqui
                    Port = 5672,          // Alteração aqui
                    UserName = "financial",
                    Password = "financial"
                };

                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();


                await channel.QueueDeclareAsync(queue: "Financiallaunch", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var jsonMessage = JsonSerializer.Serialize(financiallaunchEvent);

                var body = Encoding.UTF8.GetBytes(jsonMessage);

                await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "Financiallaunch", body: body);

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
    }
}
