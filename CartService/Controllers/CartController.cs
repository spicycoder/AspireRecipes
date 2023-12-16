using AspireRecipes.ServiceDefaults.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CartService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public CartController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("productsClient");
        }

        [HttpGet]
        public async Task<ActionResult<ProductDetails>> ReadProductDetails(int productId)
        {
            ProductDetails? response = await _httpClient.GetFromJsonAsync<ProductDetails>($"/api/Products?id={productId}");

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}
