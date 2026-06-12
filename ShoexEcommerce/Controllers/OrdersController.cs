using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orders, ILogger<OrdersController> logger)
        {
            _orders = orders;
            _logger = logger;
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

            try
            {
                var res = await _orders.PlaceOrderAsync(userId.Value, dto, ct);
                return StatusCode(res.StatusCode, res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error placing order for user {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while placing your order. Please try again." });
            }
        }

        [HttpPost("buy-now")]
        public async Task<IActionResult> BuyNow([FromBody] BuyNowDto dto, CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var res = await _orders.BuyNowAsync(userId.Value, dto, ct);
                return StatusCode(res.StatusCode, res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing buy-now for user {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while executing buy-now. Please try again." });
            }
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

        [HttpPatch("cancel")]
        public async Task<IActionResult> CancelOrder([FromForm] int orderId, [FromForm] string reason, CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var res = await _orders.CancelOrderAsync(userId.Value, orderId, reason, ct);
            return StatusCode(res.StatusCode, res);
        }
    }
}
