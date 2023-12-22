using AspireRecipes.ServiceDefaults.Entities;
using MongoDB.Driver;

namespace OrderService;

public class OrdersContext
{
    private readonly IMongoDatabase _database;

    public OrdersContext(string connectionString)
    {
        IMongoClient _client = new MongoClient(connectionString);
        _database = _client.GetDatabase("OrdersDB");
    }

    public IMongoCollection<Order> Orders
    {
        get
        {
            return _database.GetCollection<Order>("orders");
        }
    }
}
