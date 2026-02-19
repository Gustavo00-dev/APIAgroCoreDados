using System.Text;
using System.Text.Json;
using APIAgroCoreDados.Data;
using APIAgroCoreDados.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace APIAgroCoreDados.Services
{
    public class RabbitMqListener : BackgroundService
    {
        private readonly RabbitMqService _rabbit;
        private readonly IServiceScopeFactory _scopeFactory;

        private IConnection? _connection;
        private IChannel? _channel;

        private const string QueueName = "queue.propriedade.command";

        public RabbitMqListener(RabbitMqService rabbit, IServiceScopeFactory scopeFactory)
        {
            _rabbit = rabbit;
            _scopeFactory = scopeFactory;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _connection = await _rabbit.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await _channel.BasicQosAsync(0, 1, false);

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_channel == null)
                return;

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                try
                {
                    using var doc = JsonDocument.Parse(json);
                    var root = doc.RootElement;

                    if (root.TryGetProperty("command", out var commandEl)
                        && commandEl.GetString() == "create"
                        && root.TryGetProperty("data", out var dataEl))
                    {
                        var idUsers = dataEl.GetProperty("idUsers").GetInt32();
                        var nome = dataEl.GetProperty("nome").GetString();
                        var area = dataEl.GetProperty("area").GetDouble();

                        using var scope = _scopeFactory.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        var prop = new Propriedade
                        {
                            IdUsers = idUsers,
                            Nome = nome ?? string.Empty,
                            Area = area
                        };
                        Console.WriteLine($"Criando propriedade: IdUsers={idUsers}, Nome={nome}, Area={area}");
                        db.Propriedade.Add(prop);
                        await db.SaveChangesAsync(stoppingToken);
                    }

                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch
                {
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false,
                consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null)
                await _channel.CloseAsync();

            if (_connection != null)
                await _connection.CloseAsync();

            await base.StopAsync(cancellationToken);
        }
    }
}
