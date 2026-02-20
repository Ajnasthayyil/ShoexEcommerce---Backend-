using Microsoft.EntityFrameworkCore;
using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.DTOs.Cart;
using ShoexEcommerce.Application.Interfaces.Cart;
using ShoexEcommerce.Domain.Entities;
using ShoexEcommerce.Infrastructure.Data;

namespace ShoexEcommerce.Infrastructure.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _db;

        public CartService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<CartDto>> GetMyCartAsync(int userId, CancellationToken ct = default)
        {
            var cart = await GetOrCreateCartAsync(userId, ct);
            return ApiResponse<CartDto>.Success(await BuildCartDto(cart.Id, ct), "Cart fetched");
        }

       
        public async Task<ApiResponse<CartItemDto>> AddAsync(int userId, AddToCartDto dto, CancellationToken ct = default)
        {
            // validate product
            var product = await _db.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == dto.ProductId && p.IsActive, ct);

            if (product == null)
                return ApiResponse<CartItemDto>.Fail("Invalid ProductId", 400);

            // validate size
            var sizeExists = await _db.Sizes.AnyAsync(s => s.Id == dto.SizeId && s.IsActive, ct);
            if (!sizeExists)
                return ApiResponse<CartItemDto>.Fail("Invalid SizeId", 400);

            // size availability & stock 
            var ps = await _db.ProductSizes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ProductId == dto.ProductId && x.SizeId == dto.SizeId, ct);

            if (ps == null)
                return ApiResponse<CartItemDto>.Fail("This size is not available for this product", 400);

            if (ps.Stock <= 0)
                return ApiResponse<CartItemDto>.Fail("Out of stock for this size", 409);

            var cart = await GetOrCreateCartAsync(userId, ct);

            // check already exists
            var exists = await _db.CartItems.AnyAsync(i =>
                i.CartId == cart.Id &&
                i.ProductId == dto.ProductId &&
                i.SizeId == dto.SizeId, ct);

            if (exists)
                return ApiResponse<CartItemDto>.Fail("Warning: Product is already in cart", 409); // or 400

            // add new item (Quantity default = 1)
            var item = new CartItem
            {
                CartId = cart.Id,
                ProductId = dto.ProductId,
                SizeId = dto.SizeId,
                Quantity = 1
            };

            _db.CartItems.Add(item);
            await _db.SaveChangesAsync(ct);

            var itemDto = await BuildCartItemDto(item.Id, ct);
            return ApiResponse<CartItemDto>.Success(itemDto, "Added to cart", 200);
        }

        public async Task<ApiResponse<CartItemDto>> UpdateAsync(int userId, UpdateCartItemDto dto, CancellationToken ct = default)
        {
            if (dto.Quantity <= 0)
                return ApiResponse<CartItemDto>.Fail("Quantity must be at least 1", 400);

            var cart = await GetOrCreateCartAsync(userId, ct);

            var item = await _db.CartItems
                .FirstOrDefaultAsync(x => x.Id == dto.CartItemId && x.CartId == cart.Id, ct);

            if (item == null)
                return ApiResponse<CartItemDto>.Fail("Cart item not found", 404);

            // size-stock check
            var ps = await _db.ProductSizes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ProductId == item.ProductId && x.SizeId == item.SizeId, ct);

            if (ps == null)
                return ApiResponse<CartItemDto>.Fail("This size is not available for this product", 400);

            if (ps.Stock <= 0)
                return ApiResponse<CartItemDto>.Fail("Out of stock for this size", 409);

            if (dto.Quantity > ps.Stock)
                return ApiResponse<CartItemDto>.Fail($"Only {ps.Stock} left for this size", 409);

            item.Quantity = dto.Quantity;
            await _db.SaveChangesAsync(ct);

            //  return only updated item
            var itemDto = await BuildCartItemDto(item.Id, ct);
            return ApiResponse<CartItemDto>.Success(itemDto, "Cart item updated", 200);
        }


        public async Task<ApiResponse<string>> DeleteItemAsync(int userId, int cartItemId, CancellationToken ct = default)
        {
            var cart = await GetOrCreateCartAsync(userId, ct);

            var item = await _db.CartItems.FirstOrDefaultAsync(x => x.Id == cartItemId && x.CartId == cart.Id, ct);
            if (item == null) return ApiResponse<string>.Fail("Cart item not found", 404);

            _db.CartItems.Remove(item);
            await _db.SaveChangesAsync(ct);

            return ApiResponse<string>.Success(null!, "Item removed");
        }

        public async Task<ApiResponse<string>> ClearAsync(int userId, CancellationToken ct = default)
        {
            var cart = await GetOrCreateCartAsync(userId, ct);

            var items = await _db.CartItems.Where(x => x.CartId == cart.Id).ToListAsync(ct);
            _db.CartItems.RemoveRange(items);
            await _db.SaveChangesAsync(ct);

            return ApiResponse<string>.Success(null!, "Cart cleared");
        }

        private async Task<Cart> GetOrCreateCartAsync(int userId, CancellationToken ct)
        {
            var cart = await _db.Carts.FirstOrDefaultAsync(c => c.UserId == userId, ct);
            if (cart != null) return cart;

            cart = new Cart { UserId = userId };
            _db.Carts.Add(cart);
            await _db.SaveChangesAsync(ct);
            return cart;
        }

        private async Task<CartDto> BuildCartDto(int cartId, CancellationToken ct)
        {
            var items = await _db.CartItems
                .AsNoTracking()
                .Where(i => i.CartId == cartId)
                .Include(i => i.Product).ThenInclude(p => p.Images)
                .Include(i => i.Size)
                .ToListAsync(ct);

            var dto = new CartDto
            {
                CartId = cartId,
                Items = items.Select(i => new CartItemDto
                {
                    CartItemId = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Price = i.Product.Price,
                    SizeId = i.SizeId,
                    SizeName = i.Size.Name,
                    Quantity = i.Quantity,
                    LineTotal = i.Product.Price * i.Quantity,
                    ImageUrl = i.Product.Images.OrderBy(x => x.Id).Select(x => x.Url).FirstOrDefault()
                }).ToList()
            };

            dto.Total = dto.Items.Sum(x => x.LineTotal);
            return dto;
        }

        private async Task<CartItemDto> BuildCartItemDto(int cartItemId, CancellationToken ct)
        {
            var item = await _db.CartItems
                .AsNoTracking()
                .Where(i => i.Id == cartItemId)
                .Include(i => i.Product).ThenInclude(p => p.Images)
                .Include(i => i.Size)
                .Select(i => new CartItemDto
                {
                    CartItemId = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Price = i.Product.Price,
                    SizeId = i.SizeId,
                    SizeName = i.Size.Name,
                    Quantity = i.Quantity,
                    LineTotal = i.Product.Price * i.Quantity,
                    ImageUrl = i.Product.Images.OrderBy(x => x.Id).Select(x => x.Url).FirstOrDefault()
                })
                .FirstAsync(ct);

            return item;
        }
    }
}