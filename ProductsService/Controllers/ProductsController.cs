using AspireRecipes.ServiceDefaults.Responses;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace ProductsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ILogger<ProductsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ProductDetails>> GetProduct(int id)
        {
            var product = new Faker<ProductDetails>()
                .RuleFor(x => x.Id, _ => id)
                .RuleFor(x => x.Name, x => x.Commerce.ProductName())
                .RuleFor(x => x.Description, x => x.Commerce.ProductDescription())
                .RuleFor(x => x.Price, x => x.Random.Decimal() * 100)
                .Generate();

            _logger.LogInformation("Product {product} matched against the ID: {id}", product.Name, id);

            return Ok(product);
        }
    }
}
