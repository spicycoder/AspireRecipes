using AspireRecipes.ServiceDefaults.Responses;
using MongoDB.Bson;

namespace AspireRecipes.ServiceDefaults.Entities;

public class Order
{
    public ObjectId Id { get; set; }
    public string Customer { get; set; }
    public ProductDetails Product { get; set; }
    public int Quantity { get; set; }
    public decimal Total { get; set; }
}
