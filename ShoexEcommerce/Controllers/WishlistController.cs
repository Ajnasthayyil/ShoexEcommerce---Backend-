using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoexEcommerce.Application.Interfaces.Wishlist;
using System.Security.Claims;

namespace ShoexEcommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _service;

        public WishlistController(IWishlistService service)
        {
            _service = service;
        }

        private int GetUserId()
        {
            var id = User.FindFirstValue("userid");
            return int.Parse(id!);
        }

        [HttpGet("MyWishlist")]
        public async Task<IActionResult> MyWishlist(CancellationToken ct)
        {
            var result = await _service.GetMyWishlistAsync(GetUserId(), ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Toggle/{productId:int}")]
        public async Task<IActionResult> Toggle(int productId, CancellationToken ct)
        {
            var result = await _service.ToggleAsync(GetUserId(), productId, ct);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("Clear")]
        public async Task<IActionResult> Clear(CancellationToken ct)
        {
            var result = await _service.ClearAsync(GetUserId(), ct);
            return StatusCode(result.StatusCode, result);
        }
    }
}
