var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedisContainer("cache", 6379);
var messageQueue = builder.AddRabbitMQContainer("messageQueue", 15279);
var mongoDB = builder
    .AddMongoDBContainer("mongoDB", 27017);

var productsService = builder.AddProject<Projects.ProductsService>("productsService")
    .WithReference(cache);

var orderService = builder.AddProject<Projects.OrderService>("orderService")
    .WithReference(mongoDB)
    .WithReference(messageQueue);

var cartService = builder.AddProject<Projects.CartService>("cartService")
    .WithReference(messageQueue)
    .WithReference(productsService);

builder.Build().Run();
