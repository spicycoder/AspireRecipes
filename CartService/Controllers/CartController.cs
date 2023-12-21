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

        [HttpGet]
        public async Task<ActionResult<ProductDetails>> ReadProductDetails(int productId)
        {
            ProductDetails? response = await _httpClient.GetFromJsonAsync<ProductDetails>($"/api/Products?id={productId}");

            if (response == null)
            {
                return NotFound();
            }

            byte[] product = System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response!));
            _channel.BasicPublish("aspire-recipe-exchange", "orders", null, product);

            return Ok(response);
        }
    }
}
