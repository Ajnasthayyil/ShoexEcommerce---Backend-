using Microsoft.EntityFrameworkCore;
using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.DTOs.Product;
using ShoexEcommerce.Application.Interfaces.Product;
using ShoexEcommerce.Domain.Entities;
using ShoexEcommerce.Domain.Enums;
using ShoexEcommerce.Infrastructure.Data;

namespace ShoexEcommerce.Infrastructure.Services
{
    public class ProductReviewService : IProductReviewService
    {
        private readonly AppDbContext _context;

        public ProductReviewService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<string>> AddReviewAsync(int userId, AddProductReviewDto dto, CancellationToken ct = default)
        {
            // 1. Check if user already reviewed this product
            var existingReview = await _context.ProductReviews
                .FirstOrDefaultAsync(r => r.ProductId == dto.ProductId && r.UserId == userId, ct);

            if (existingReview != null)
            {
                return new ApiResponse<string> { IsSuccess = false, StatusCode = 400, Message = "You have already reviewed this product." };
            }

            // 2. Check if user ordered this product
            var hasPurchased = await _context.Orders
                .Include(o => o.Items)
                .AnyAsync(o => o.UserId == userId && o.Items.Any(i => i.ProductId == dto.ProductId), ct);

            if (!hasPurchased)
            {
                return new ApiResponse<string> { IsSuccess = false, StatusCode = 403, Message = "You can only review products you have purchased." };
            }

            var review = new ProductReview
            {
                ProductId = dto.ProductId,
                UserId = userId,
                Rating = dto.Rating,
                ReviewText = dto.ReviewText,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = userId.ToString()
            };

            await _context.ProductReviews.AddAsync(review, ct);
            await _context.SaveChangesAsync(ct);

            return new ApiResponse<string> { IsSuccess = true, StatusCode = 201, Data = "Review submitted successfully." };
        }

        public async Task<ApiResponse<List<ProductReviewDto>>> GetReviewsByProductIdAsync(int productId, CancellationToken ct = default)
        {
            var reviews = await _context.ProductReviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedOn)
                .Select(r => new ProductReviewDto
                {
                    Id = r.Id,
                    ProductId = r.ProductId,
                    UserId = r.UserId,
                    UserName = r.User.FullName,
                    Rating = r.Rating,
                    ReviewText = r.ReviewText,
                    CreatedOn = r.CreatedOn
                })
                .ToListAsync(ct);

            return new ApiResponse<List<ProductReviewDto>> { IsSuccess = true, StatusCode = 200, Data = reviews };
        }
    }
}
