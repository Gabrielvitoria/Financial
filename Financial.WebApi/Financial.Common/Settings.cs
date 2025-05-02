namespace Financial.Common
{
    public static class Settings
    {
        public static string Secret = "43e4dbf0-52ed-4203-895d-42b586496bd4";
    }
    public class ConnectionQueueMenssage
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
        public string RoutingKey { get; set; }
    }
}
