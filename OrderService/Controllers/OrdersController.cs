using AspireRecipes.ServiceDefaults.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersContext _ordersContext;

        public OrdersController(OrdersContext ordersContext)
        {
            _ordersContext = ordersContext;
        }

        [HttpGet]
        public async Task<ActionResult<Order>> ReadOrders()
        {
            var orders = await _ordersContext.Orders.AsQueryable().ToListAsync();
            return Ok(orders);
        }
    }
}
