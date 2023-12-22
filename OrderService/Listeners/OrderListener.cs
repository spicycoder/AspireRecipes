using AspireRecipes.ServiceDefaults.Entities;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace OrderService.Listeners;

public class OrderListener : BackgroundService
{
    private readonly ILogger<OrderListener> _logger;
    private readonly IModel _channel;
    private readonly EventingBasicConsumer _consumer;
    private readonly OrdersContext _dbContext;

    public OrderListener(
        ILogger<OrderListener> logger,
        IConnection connection,
        OrdersContext dbContext)
    {
        _logger = logger;
        _channel = connection.CreateModel();
        _consumer = new(_channel);
        _dbContext = dbContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel.ExchangeDeclare("aspire-recipe-exchange", ExchangeType.Direct);
        _channel.QueueDeclare("orders-queue", false, false, false, null);
        _channel.QueueBind("orders-queue", "aspire-recipe-exchange", "orders", null);

        _consumer.Received += async (object? sender, BasicDeliverEventArgs e) =>
        {
            string content = System.Text.Encoding.UTF8.GetString(e.Body.Span);
            _logger.LogInformation("Received: {content}", content);
            _channel.BasicAck(e.DeliveryTag, false);

            Order? order = JsonSerializer.Deserialize<Order>(content);

            if (order != null)
            {
                order.Total = order.Product.Price * order.Quantity;

                try
                {
                    // save order to database
                    await _dbContext.Orders.InsertOneAsync(order);

                    IAsyncCursor<Order> readOrder = await _dbContext.Orders.FindAsync(x => x.Product.Id == order.Product.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving order: {order}", order);
                }
            }
            else
            {
                _logger.LogError("Unable to deserialize order {content}", content);
            }
        };

        _channel.BasicConsume("orders-queue", false, _consumer);
    }

    public override void Dispose()
    {
        _channel.Close();
        base.Dispose();
    }
}
