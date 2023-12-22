using AspireRecipes.ServiceDefaults.Entities;
using AspireRecipes.ServiceDefaults.Requests;
using AspireRecipes.ServiceDefaults.Responses;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text.Json;

namespace CartService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ILogger<CartController> _logger;
        private readonly HttpClient _httpClient;
        private readonly IModel _channel;

        public CartController(
            ILogger<CartController> logger,
            IHttpClientFactory httpClientFactory,
            IConnection connection)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("productsClient");
            _channel = connection.CreateModel();
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(OrderRequest request)
        {
            ProductDetails? response = await _httpClient.GetFromJsonAsync<ProductDetails>($"/api/Products?id={request.ProductId}");

            if (response == null)
            {
                return NotFound();
            }

            Order order = new()
            {
                Product = response!,
                Quantity = request.Quantity,
                Customer = request.Customer
            };

            byte[] orderBytes = System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(order));
            _channel.BasicPublish("aspire-recipe-exchange", "orders", null, orderBytes);

            return Ok();
        }
    }
}
