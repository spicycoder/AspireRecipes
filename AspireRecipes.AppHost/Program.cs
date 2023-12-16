using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedisContainer("cache");
var productsDatabase = builder.AddSqlServerContainer("productsDB");
var ordersDatabase = builder.AddPostgresContainer("ordersDB");
var messageQueue = builder.AddRabbitMQContainer("messageQueue");

var productsService = builder.AddProject<Projects.ProductsService>("productsService")
    .WithReference(cache)
    .WithReference(productsDatabase);

var orderService = builder.AddProject<Projects.OrderService>("orderService")
    .WithReference(ordersDatabase)
    .WithReference(messageQueue);

var cartService = builder.AddProject<Projects.CartService>("cartService")
    .WithReference(messageQueue)
    .WithReference(productsService);

builder.Build().Run();
