using RabbitMQ.Client;

namespace APIAgroCoreDados.Services
{
    public class RabbitMqService
    {
        private readonly ConnectionFactory _factory;

        public RabbitMqService(IConfiguration configuration)
        {
            var host = configuration["RabbitMQ:Host"] ?? "host.docker.internal";
            var user = configuration["RabbitMQ:User"] ?? "guest";
            var pass = configuration["RabbitMQ:Password"] ?? "guest";
            var port = int.TryParse(configuration["RabbitMQ:Port"], out var p) ? p : 5672;

            _factory = new ConnectionFactory
            {
                HostName = host,
                UserName = user,
                Password = pass,
                Port = port
            };
        }

        public async Task<IConnection> CreateConnectionAsync()
        {
            return await _factory.CreateConnectionAsync();
        }
    }
}
