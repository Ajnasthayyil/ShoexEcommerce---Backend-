using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoexEcommerce.Application.DTOs.Cart;
using ShoexEcommerce.Application.Interfaces.Cart;
using System.Security.Claims;

namespace ShoexEcommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _service;

        public CartController(ICartService service)
        {
            _service = service;
        }

        private int GetUserId()
        {
            var id = User.FindFirstValue("userid");
            return int.Parse(id!);
        }

        [HttpGet("myCart")]
        public async Task<IActionResult> MyCart(CancellationToken ct)
        {
            var result = await _service.GetMyCartAsync(GetUserId(), ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromForm] AddToCartDto dto, CancellationToken ct)
        {
            var result = await _service.AddAsync(GetUserId(), dto, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] UpdateCartItemDto dto, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(GetUserId(), dto, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _service.DeleteItemAsync(GetUserId(), id, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> Clear(CancellationToken ct)
        {
            var result = await _service.ClearAsync(GetUserId(), ct);
            return StatusCode(result.StatusCode, result);
        }
    }
}
