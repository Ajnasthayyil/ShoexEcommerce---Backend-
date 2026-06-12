using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoexEcommerce.Application.Interfaces.Order;
using ShoexEcommerce.Application.DTOs.Payment;
using System.Security.Claims;

[ApiController]
[Route("api/payment")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly PaymentService _paymentService;
    private readonly IOrderService _orderService;

    public PaymentController(PaymentService paymentService, IOrderService orderService)
    {
        _paymentService = paymentService;
        _orderService = orderService;
    }

    private int? GetUserId()
    {
        var userIdStr = User.FindFirstValue("userid");
        if (int.TryParse(userIdStr, out var userId))
            return userId;
        return null;
    }

    [HttpPost("create-order")]
    public IActionResult CreateOrder(CreateOrderRequest request)
    {
        var result = _paymentService.CreateOrder(request.Amount);

        return Ok(result);
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentDto dto, CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var res = await _orderService.VerifyAndConfirmOrderAsync(userId.Value, dto, ct);
        return StatusCode(res.StatusCode, res);
    }
}