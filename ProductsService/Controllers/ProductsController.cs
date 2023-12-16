using AspireRecipes.ServiceDefaults.Responses;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace ProductsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IDistributedCache _cache;
        public ProductsController(
            ILogger<ProductsController> logger,
            IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<ProductDetails>> GetProduct(int id)
        {
            byte[]? cachedProduct = await _cache.GetAsync(id.ToString());

            if (cachedProduct != null)
            {
                ProductDetails? productDetails = JsonSerializer.Deserialize<ProductDetails>(cachedProduct);

                if (productDetails != null)
                {
                    return Ok(productDetails!);
                }

                return NotFound();
            }

            var product = new Faker<ProductDetails>()
                .RuleFor(x => x.Id, _ => id)
                .RuleFor(x => x.Name, x => x.Commerce.ProductName())
                .RuleFor(x => x.Description, x => x.Commerce.ProductDescription())
                .RuleFor(x => x.Price, x => x.Random.Decimal() * 100)
                .Generate();

            _logger.LogInformation("Product {product} matched against the ID: {id}", product.Name, id);

            await _cache.SetAsync(
                id.ToString(),
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(product)),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
                });

            return Ok(product);
        }
    }
}
