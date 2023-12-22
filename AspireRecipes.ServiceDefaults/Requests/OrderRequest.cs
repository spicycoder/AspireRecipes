namespace AspireRecipes.ServiceDefaults.Requests;

public class OrderRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string Customer { get; set; }
    public decimal Total { get; set; }
}
