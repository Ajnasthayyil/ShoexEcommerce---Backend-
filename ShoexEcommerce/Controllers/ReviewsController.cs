using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoexEcommerce.Application.DTOs.Product;
using ShoexEcommerce.Application.Interfaces.Product;
using System.Security.Claims;

namespace ShoexEcommerce.API.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IProductReviewService _reviewService;

        public ReviewsController(IProductReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("product/{productId:int}")]
        public async Task<IActionResult> GetReviews(int productId, CancellationToken ct)
        {
            var result = await _reviewService.GetReviewsByProductIdAsync(productId, ct);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] AddProductReviewDto dto, CancellationToken ct)
        {
            var userIdClaim = User.FindFirst("userid")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var result = await _reviewService.AddReviewAsync(userId, dto, ct);
            return StatusCode(result.StatusCode, result);
        }
    }
}
