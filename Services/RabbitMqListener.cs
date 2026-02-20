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

        private const string QueuePropriedade = "queue.propriedade.command";
        private const string QueueTalhao = "queue.talhao.command";

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
                queue: QueuePropriedade,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await _channel.QueueDeclareAsync(
                queue: QueueTalhao,
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
          
            #region Consumer de Propriedade
            var consumerProp = new AsyncEventingBasicConsumer(_channel);

            consumerProp.ReceivedAsync += async (sender, ea) =>
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
            #endregion

            #region Consumer de Talhoes
            var consumerTalhao = new AsyncEventingBasicConsumer(_channel);

            consumerTalhao.ReceivedAsync += async (sender, ea) =>
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
                        var idPropriedade = dataEl.GetProperty("propriedadeId").GetInt32();
                        var nome = dataEl.GetProperty("Nome").GetString();
                        var area = dataEl.GetProperty("Area").GetDouble();
                        var descricao = dataEl.GetProperty("Descricao").GetString();

                        using var scope = _scopeFactory.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        var talhao = new Talhao
                        {
                            IdPropriedade = idPropriedade,
                            Nome = nome ?? string.Empty,
                            Area = area,
                            Descricao = descricao ?? string.Empty
                        };

                        db.Talhao.Add(talhao);
                        await db.SaveChangesAsync(stoppingToken);
                    }

                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch
                {
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                }
            };
            #endregion


            await _channel.BasicConsumeAsync(
                queue: QueuePropriedade,
                autoAck: false,
                consumer: consumerProp);

            await _channel.BasicConsumeAsync(
                queue: QueueTalhao,
                autoAck: false,
                consumer: consumerTalhao);

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
