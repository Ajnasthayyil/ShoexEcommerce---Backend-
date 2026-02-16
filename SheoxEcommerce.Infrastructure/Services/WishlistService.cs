using Microsoft.EntityFrameworkCore;
using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.DTOs.Wishlist;
using ShoexEcommerce.Application.Interfaces.Wishlist;
using ShoexEcommerce.Domain.Entities;
using ShoexEcommerce.Infrastructure.Data;

namespace ShoexEcommerce.Infrastructure.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly AppDbContext _db;

        public WishlistService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<WishlistDto>> GetMyWishlistAsync(int userId, CancellationToken ct = default)
        {
            var items = await _db.WishlistItems
                .AsNoTracking()
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                    .ThenInclude(p => p.Images)
                .OrderByDescending(w => w.Id)
                .ToListAsync(ct);

            var dto = new WishlistDto
            {
                Items = items.Select(w => new WishlistProductDto
                {
                    ProductId = w.ProductId,
                    Name = w.Product.Name,
                    Price = w.Product.Price,
                    IsActive = w.Product.IsActive,
                    ImageUrl = w.Product.Images.OrderBy(i => i.Id).Select(i => i.Url).FirstOrDefault()
                }).ToList()
            };

            return ApiResponse<WishlistDto>.Success(dto, "Wishlist fetched");
        }

        public async Task<ApiResponse<string>> ToggleAsync(int userId, int productId, CancellationToken ct = default)
        {
            var product = await _db.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId, ct);

            if (product == null)
                return ApiResponse<string>.Fail("Invalid ProductId", 400);

            if (!product.IsActive)
                return ApiResponse<string>.Fail("Product is inactive", 409);

            //if (product.Stock <= 0)
            //    return ApiResponse<string>.Fail("Out of stock", 409);

            var existing = await _db.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId, ct);

            if (existing != null)
            {
                _db.WishlistItems.Remove(existing);
                await _db.SaveChangesAsync(ct);
                return ApiResponse<string>.Success(null!, "Removed from wishlist");
            }

            _db.WishlistItems.Add(new WishlistItem
            {
                UserId = userId,
                ProductId = productId
            });

            await _db.SaveChangesAsync(ct);
            return ApiResponse<string>.Success(null!, "Added to wishlist");
        }


        public async Task<ApiResponse<string>> ClearAsync(int userId, CancellationToken ct = default)
        {
            var items = await _db.WishlistItems.Where(x => x.UserId == userId).ToListAsync(ct);
            _db.WishlistItems.RemoveRange(items);
            await _db.SaveChangesAsync(ct);

            return ApiResponse<string>.Success(null!, "Wishlist cleared");
        }
    }
}
