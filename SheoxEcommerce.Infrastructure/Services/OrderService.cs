using Microsoft.EntityFrameworkCore;
using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.DTOs.Order;
using ShoexEcommerce.Application.Interfaces.Order;
using ShoexEcommerce.Domain.Entities;
using ShoexEcommerce.Domain.Enums;
using ShoexEcommerce.Infrastructure.Data;

namespace ShoexEcommerce.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;

        public OrderService(AppDbContext db)
        {
            _db = db;
        }


        public async Task<ApiResponse<string>> PlaceOrderAsync(int userId, PlaceOrderDto dto, CancellationToken ct = default)
        {
            //  Validate user
            var userExists = await _db.Users.AnyAsync(x => x.Id == userId && x.IsActive, ct);
            if (!userExists)
                return ApiResponse<string>.Fail("User not found", 404);

            //  Pick address: dto.AddressId OR default address
            Address? address = null;


            if (dto.AddressId.HasValue && dto.AddressId.Value > 0)
            {
                address = await _db.Addresses
                    .FirstOrDefaultAsync(a =>
                        a.Id == dto.AddressId.Value &&
                        a.UserId == userId &&
                        a.IsActive, ct);

                if (address == null)
                    return ApiResponse<string>.Fail("Invalid address", 400);
            }
            else
            {
                // Otherwise use default address
                address = await _db.Addresses
                    .FirstOrDefaultAsync(a =>
                        a.UserId == userId &&
                        a.IsActive &&
                        a.IsDefault, ct);

                if (address == null)
                    return ApiResponse<string>.Fail("No default address set. Please select an address.", 400);
            }

            //  Load cart items for this user
            var cartItems = await _db.CartItems
                .Include(x => x.Product)
                .Include(x => x.Size)
                .Include(x => x.Cart)
                .Where(x => x.Cart.UserId == userId && x.IsActive)
                .ToListAsync(ct);

            if (cartItems.Count == 0)
                return ApiResponse<string>.Fail("Cart is empty", 400);

            await using var trx = await _db.Database.BeginTransactionAsync(ct);

            try
            {
                // Create order
                var order = new Order
                {
                    UserId = userId,
                    PaymentMethod = dto.PaymentMethod,
                    Status = OrderStatus.Ordered,
                    SubTotal = 0,
                    TotalAmount = 0,


                    ShippingAddressId = address.Id
                };

                decimal subTotal = 0;

                foreach (var ci in cartItems)
                {
                    if (ci.Product == null)
                        return ApiResponse<string>.Fail("Invalid cart item", 400);

                    // Stock check from ProductSizes (ProductId + SizeId)
                    var ps = await _db.ProductSizes
                        .FirstOrDefaultAsync(x => x.ProductId == ci.ProductId && x.SizeId == ci.SizeId, ct);

                    if (ps == null)
                        return ApiResponse<string>.Fail($"Selected size not available for {ci.Product.Name}", 400);

                    if (ps.Stock < ci.Quantity)
                        return ApiResponse<string>.Fail($"Insufficient stock for {ci.Product.Name} (selected size)", 400);

                    var unitPrice = ci.Product.Price;
                    var lineTotal = unitPrice * ci.Quantity;

                    order.Items.Add(new OrderItem
                    {
                        ProductId = ci.ProductId,
                        SizeId = ci.SizeId,
                        Quantity = ci.Quantity,
                        UnitPrice = unitPrice,
                        TotalPrice = lineTotal
                    });

                    subTotal += lineTotal;

                    //  reduce size-wise stock
                    ps.Stock -= ci.Quantity;
                }

                order.SubTotal = subTotal;
                order.TotalAmount = subTotal;

                _db.Orders.Add(order);

                // clear cart
                _db.CartItems.RemoveRange(cartItems);

                await _db.SaveChangesAsync(ct);
                await trx.CommitAsync(ct);

                return ApiResponse<string>.Success(null, "Order placed successfully", 201);
            }
            catch
            {
                await trx.RollbackAsync(ct);
                throw;
            }
        }


        public async Task<ApiResponse<string>> BuyNowAsync(int userId, BuyNowDto dto, CancellationToken ct = default)
        {
            var userExists = await _db.Users.AnyAsync(x => x.Id == userId && x.IsActive, ct);
            if (!userExists)
                return ApiResponse<string>.Fail("User not found", 404);

            //  Pick address: dto.AddressId OR default address
            Address? address = null;

            if (dto.AddressId > 0)
            {
                address = await _db.Addresses
                    .FirstOrDefaultAsync(a => a.Id == dto.AddressId && a.UserId == userId && a.IsActive, ct);

                if (address == null)
                    return ApiResponse<string>.Fail("Invalid address", 400);
            }
            else
            {
                address = await _db.Addresses
                    .FirstOrDefaultAsync(a => a.UserId == userId && a.IsActive && a.IsDefault, ct);

                if (address == null)
                    return ApiResponse<string>.Fail("No default address set. Please select an address.", 400);
            }

            var product = await _db.Products.FirstOrDefaultAsync(x => x.Id == dto.ProductId && x.IsActive, ct);
            if (product == null)
                return ApiResponse<string>.Fail("Product not found", 404);

            // Stock check from ProductSizes
            var ps = await _db.ProductSizes
                .FirstOrDefaultAsync(x => x.ProductId == dto.ProductId && x.SizeId == dto.SizeId, ct);

            if (ps == null)
                return ApiResponse<string>.Fail("Selected size not available", 400);

            if (ps.Stock < dto.Quantity)
                return ApiResponse<string>.Fail("Insufficient stock for this size", 400);

            await using var trx = await _db.Database.BeginTransactionAsync(ct);

            try
            {
                var total = product.Price * dto.Quantity;

                var order = new Order
                {
                    UserId = userId,
                    PaymentMethod = dto.PaymentMethod,
                    Status = OrderStatus.Ordered,
                    SubTotal = total,
                    TotalAmount = total,

                   
                    ShippingAddressId = address.Id
                };

                order.Items.Add(new OrderItem
                {
                    ProductId = dto.ProductId,
                    SizeId = dto.SizeId,
                    Quantity = dto.Quantity,
                    UnitPrice = product.Price,
                    TotalPrice = total
                });

                _db.Orders.Add(order);

                // reduce size-wise stock
                ps.Stock -= dto.Quantity;

                await _db.SaveChangesAsync(ct);
                await trx.CommitAsync(ct);

                return ApiResponse<string>.Success(null, "Order placed successfully", 201);
            }
            catch
            {
                await trx.RollbackAsync(ct);
                throw;
            }
        }

        // My Orders
        public async Task<ApiResponse<List<OrderListDto>>> GetMyOrdersAsync(int userId, CancellationToken ct = default)
        {
            var list = await _db.Orders
                .AsNoTracking()
                .Where(o => o.UserId == userId && o.IsActive)
                .OrderByDescending(o => o.CreatedOn)
                .Select(o => new OrderListDto
                {
                    OrderId = o.Id,
                    CustomerName = o.User != null ? o.User.FullName : "",  // optional
                    Status = o.Status.ToString(),
                    TotalAmount = o.TotalAmount,
                    CreatedOn = o.CreatedOn,

                    Items = o.Items
                        .OrderBy(i => i.Id)
                        .Select(i => new OrderListItemDto
                        {
                            ProductId = i.ProductId,
                            ProductName = i.Product != null ? i.Product.Name : "",
                            SizeId = i.SizeId,
                            SizeName = i.Size != null ? i.Size.Name : "",
                            Quantity = i.Quantity,
                            UnitPrice = i.UnitPrice,
                            TotalPrice = i.TotalPrice
                        }).ToList()
                })
                .ToListAsync(ct);

            return ApiResponse<List<OrderListDto>>.Success(list, "My orders", 200);
        }


        //My Order Detail

        public async Task<ApiResponse<OrderDetailDto>> GetMyOrderDetailAsync(int userId, int orderId, CancellationToken ct = default)
        {
            var order = await _db.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .Include(o => o.Items).ThenInclude(i => i.Size)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId && o.IsActive, ct);

            if (order == null)
                return ApiResponse<OrderDetailDto>.Fail("Order not found", 404);

            var dto = new OrderDetailDto
            {
                OrderId = order.Id,
                Status = order.Status.ToString(),
                SubTotal = order.SubTotal,
                TotalAmount = order.TotalAmount,
                PaymentMethod = order.PaymentMethod,
                CreatedOn = order.CreatedOn,
                CustomerName = order.User?.FullName ?? "",
                CustomerEmail = order.User?.Email ?? "",
                CustomerMobile = order.User?.MobileNumber ?? "",
                Items = order.Items.Select(i => new OrderDetailDto.OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "",
                    SizeId = i.SizeId,
                    SizeName = i.Size?.Name ?? "",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Total = i.TotalPrice
                }).ToList()
            };

            return ApiResponse<OrderDetailDto>.Success(dto, "Order details", 200);
        }


        public async Task<ApiResponse<string>> CancelMyOrderAsync(int userId, CancelOrderDto dto, CancellationToken ct = default)
        {
            // load order with items (needed to restore stock)
            var order = await _db.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == dto.OrderId && o.UserId == userId && o.IsActive, ct);

            if (order == null)
                return ApiResponse<string>.Fail("Order not found", 404);

            // already cancelled
            if (order.Status == OrderStatus.Cancelled)
                return ApiResponse<string>.Fail("Order already cancelled", 409);

            // cannot cancel after shipped/delivered
            if (order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered)
                return ApiResponse<string>.Fail("Order cannot be cancelled now", 409);

            // only allow cancel for these statuses
            var canCancel = order.Status == OrderStatus.Ordered
                         || order.Status == OrderStatus.UnderProcess
                         || order.Status == OrderStatus.Packed;

            if (!canCancel)
                return ApiResponse<string>.Fail("Order cannot be cancelled", 409);

            await using var trx = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                // restore size-wise stock
                foreach (var item in order.Items)
                {
                    var ps = await _db.ProductSizes
                        .FirstOrDefaultAsync(x => x.ProductId == item.ProductId && x.SizeId == item.SizeId, ct);

                    // if mapping missing, still continue (but normally shouldn't happen)
                    if (ps != null)
                        ps.Stock += item.Quantity;
                }

                // update status
                order.Status = OrderStatus.Cancelled;
                order.ModifiedOn = DateTime.UtcNow;

                // OPTIONAL: if you have a column for cancel reason, set it here
                // order.CancelReason = dto.Reason;

                await _db.SaveChangesAsync(ct);
                await trx.CommitAsync(ct);

                return ApiResponse<string>.Success(null!, "Order cancelled successfully", 200);
            }
            catch
            {
                await trx.RollbackAsync(ct);
                throw;
            }
        }


        // Admin -  Orders list
        public async Task<ApiResponse<List<OrderListDto>>> AdminGetOrdersAsync(CancellationToken ct = default)
        {
            var list = await _db.Orders
                .AsNoTracking()
                .Where(o => o.IsActive)
                .Include(o => o.User)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreatedOn)
                .Select(o => new OrderListDto
                {
                    OrderId = o.Id,
                    CustomerName = o.User != null ? o.User.FullName : "",
                    Status = o.Status.ToString(),
                    TotalAmount = o.TotalAmount,
                    CreatedOn = o.CreatedOn,

                    Items = o.Items.OrderBy(i => i.Id).Select(i => new OrderListItemDto
                    {
                        ProductId = i.ProductId,
                        ProductName = i.Product != null ? i.Product.Name : "",
                        SizeId = i.SizeId,
                        SizeName = i.Size != null ? i.Size.Name : "",
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        TotalPrice = i.TotalPrice
                    }).ToList()
                })

                .ToListAsync(ct);

            return ApiResponse<List<OrderListDto>>.Success(list, "Orders", 200);
        }

        // Admin -  Order detail
        public async Task<ApiResponse<OrderDetailDto>> AdminGetOrderDetailAsync(int orderId, CancellationToken ct = default)
        {
            var order = await _db.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .Include(o => o.Items).ThenInclude(i => i.Size)
                // .Include(o => o.ShippingAddress)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.IsActive, ct);

            if (order == null)
                return ApiResponse<OrderDetailDto>.Fail("Order not found", 404);

            var dto = new OrderDetailDto
            {
                OrderId = order.Id,
                Status = order.Status.ToString(),
                SubTotal = order.SubTotal,
                TotalAmount = order.TotalAmount,
                PaymentMethod = order.PaymentMethod,
                CreatedOn = order.CreatedOn,
                CustomerName = order.User?.FullName ?? "",
                CustomerEmail = order.User?.Email ?? "",
                CustomerMobile = order.User?.MobileNumber ?? "",
                Items = order.Items.Select(i => new OrderDetailDto.OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "",
                    SizeId = i.SizeId,
                    SizeName = i.Size?.Name ?? "",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Total = i.TotalPrice
                }).ToList()
            };

            return ApiResponse<OrderDetailDto>.Success(dto, "Order details", 200);
        }

        // Admin- Update status
        public async Task<ApiResponse<string>> AdminUpdateStatusAsync(int orderId, OrderStatus status, CancellationToken ct = default)
        {
            if (!Enum.IsDefined(typeof(OrderStatus), status))
                return ApiResponse<string>.Fail("Invalid status value", 400);

            var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.IsActive, ct);
            if (order == null)
                return ApiResponse<string>.Fail("Order not found", 404);

            if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
                return ApiResponse<string>.Fail("Cannot update final status", 400);

            if ((int)status < (int)order.Status)
                return ApiResponse<string>.Fail("Cannot revert order status", 400);

            if (order.Status == status)
                return ApiResponse<string>.Success(null, "Status unchanged", 200);

            order.Status = status;
            order.ModifiedOn = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);

            return ApiResponse<string>.Success(null, "Status updated", 200);
        }


    }
}
