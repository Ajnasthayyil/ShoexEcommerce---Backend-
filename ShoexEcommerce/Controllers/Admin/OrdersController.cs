using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoexEcommerce.Application.DTOs.Order;          // UpdateOrderStatusDto
using ShoexEcommerce.Application.Interfaces.Order;    // IOrderService

namespace ShoexEcommerce.API.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/orders")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orders;

        public OrdersController(IOrderService orders)
        {
            _orders = orders;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var res = await _orders.AdminGetOrdersAsync(ct);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("{orderId:int}")]
        public async Task<IActionResult> GetDetail(int orderId, CancellationToken ct)
        {
            var res = await _orders.AdminGetOrderDetailAsync(orderId, ct);

            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("{orderId:int}/status")]
        public async Task<IActionResult> UpdateStatus(
            int orderId,
            [FromBody] UpdateOrderStatusDto dto,
            CancellationToken ct)
        {
            var result = await _orders.AdminUpdateStatusAsync(orderId, dto.Status, ct);
            return StatusCode(result.StatusCode, result);
        }
    }
}