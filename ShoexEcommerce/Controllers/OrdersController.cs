using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoexEcommerce.Application.DTOs.Order;
using ShoexEcommerce.Application.Interfaces.Order;
using System.Security.Claims;

namespace ShoexEcommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orders;

        public OrdersController(IOrderService orders)
        {
            _orders = orders;
        }

        private int? GetUserId()
        {
            var userIdStr = User.FindFirstValue("userid");
            if (int.TryParse(userIdStr, out var userId))
                return userId;
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto, CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var res = await _orders.PlaceOrderAsync(userId.Value, dto, ct);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("buy-now")]
        public async Task<IActionResult> BuyNow([FromBody] BuyNowDto dto, CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var res = await _orders.BuyNowAsync(userId.Value, dto, ct);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("my")]
        public async Task<IActionResult> MyOrders(CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var res = await _orders.GetMyOrdersAsync(userId.Value, ct);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("{orderId:int}")]
        public async Task<IActionResult> MyOrderDetail(int orderId, CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var res = await _orders.GetMyOrderDetailAsync(userId.Value, orderId, ct);
            return StatusCode(res.StatusCode, res);
        }
    }
}
