using OrderService;
using OrderService.Listeners;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddRabbitMQ("messageQueue");
builder.AddMongoDBClient("mongoDB");
var connectionString = builder.Configuration.GetConnectionString("mongoDB");
builder.Services.AddTransient(_ => new OrdersContext(connectionString!));
builder.Services.AddHostedService<OrderListener>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
