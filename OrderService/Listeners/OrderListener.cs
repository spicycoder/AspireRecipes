using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderService.Listeners;

public class OrderListener : BackgroundService
{
    private readonly ILogger<OrderListener> _logger;
    private readonly IModel _channel;
    private readonly EventingBasicConsumer _consumer;

    public OrderListener(
        ILogger<OrderListener> logger,
        IConnection connection)
    {
        _logger = logger;
        _channel = connection.CreateModel();
        _consumer = new(_channel);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel.ExchangeDeclare("my-exchange", ExchangeType.Direct);
        _channel.QueueDeclare("my-queue", false, false, false, null);
        _channel.QueueBind("my-queue", "my-exchange", "my-route", null);

        _consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
        {
            var content = System.Text.Encoding.UTF8.GetString(e.Body.Span);
            _logger.LogInformation("Received: {content}", content);
            _channel.BasicAck(e.DeliveryTag, false);
        };

        _channel.BasicConsume("my-queue", false, _consumer);
    }

    public override void Dispose()
    {
        _channel.Close();
        base.Dispose();
    }
}
