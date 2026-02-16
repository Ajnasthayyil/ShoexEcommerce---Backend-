using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.DTOs.Product;
using ShoexEcommerce.Application.DTOs.Products;
using ShoexEcommerce.Application.Interfaces.Media;
using ShoexEcommerce.Application.Interfaces.Product;
using ShoexEcommerce.Domain.Entities;
using ShoexEcommerce.Infrastructure.Data;
using System.Text.RegularExpressions;

namespace ShoexEcommerce.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;
        private readonly ICloudinaryService _cloudinary;

        public ProductService(AppDbContext db, ICloudinaryService cloudinary)
        {
            _db = db;
            _cloudinary = cloudinary;
        }

        // =========================================================
        // USER: Get all ACTIVE products (list view)
        // =========================================================
        public async Task<ApiResponse<List<ProductListDto>>> GetActiveProductsAsync(CancellationToken ct = default)
        {
            var products = await _db.Products
                .AsNoTracking()
                .Where(p => p.IsActive)
                .Include(p => p.Brand)
                .Include(p => p.Gender)
                .Include(p => p.Images)
                .OrderByDescending(p => p.Id)
                .ToListAsync(ct);

            var dto = products.Select(p => new ProductListDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                BrandName = p.Brand?.Name ?? "",
                GenderName = p.Gender?.Name ?? "",
                ImageUrl = p.Images
                    .OrderBy(i => i.Id)
                    .Select(i => i.Url)
                    .FirstOrDefault() ?? ""
            }).ToList();

            return ApiResponse<List<ProductListDto>>.Success(dto, "Active products fetched");
        }

        // =========================================================
        // USER: Get ACTIVE product by id (details)
        // =========================================================
        public async Task<ApiResponse<ProductDto>> GetActiveProductByIdAsync(int id, CancellationToken ct = default)
        {
            var p = await _db.Products
                .AsNoTracking()
                .Where(x => x.IsActive && x.Id == id)
                .Include(x => x.Brand)
                .Include(x => x.Gender)
                .Include(x => x.Images)
                .Include(x => x.ProductSizes)
                .FirstOrDefaultAsync(ct);

            if (p == null)
                return ApiResponse<ProductDto>.Fail("Product not found", 404);

            var dto = new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                //Stock = p.Stock,
                Description = p.Description,
                BrandId = p.BrandId,
                GenderId = p.GenderId,
                BrandName = p.Brand?.Name ?? "",
                GenderName = p.Gender?.Name ?? "",
                IsActive = p.IsActive,
                SizeIds = p.ProductSizes.Select(s => s.SizeId).ToList(),
                Images = p.Images.Select(i => new ProductImageDto
                {
                    Id = i.Id,
                    Url = i.Url
                }).ToList()
            };

            return ApiResponse<ProductDto>.Success(dto, "Product fetched");
        }

        // =========================================================
        // ADMIN: Create product (sizes + upload multiple images)
        // =========================================================
        //public async Task<ApiResponse<ProductDto>> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
        //{
        //    // ---------- BASIC VALIDATIONS ----------
        //    if (dto == null)
        //        return ApiResponse<ProductDto>.Fail("Invalid request", 400);

        //    dto.Name = dto.Name?.Trim() ?? "";

        //    if (string.IsNullOrWhiteSpace(dto.Name))
        //        return ApiResponse<ProductDto>.Fail("Name is required", 400);

        //    if (!Regex.IsMatch(dto.Name, @"^[A-Za-z][A-Za-z0-9 ]*$"))
        //        return ApiResponse<ProductDto>.Fail("Name must start with a letter and contain only letters, digits, and spaces", 400);

        //    if (dto.Price <= 1)
        //        return ApiResponse<ProductDto>.Fail("Price must be greater than 1", 400);

        //    if (dto.Stock <= 1)
        //        return ApiResponse<ProductDto>.Fail("Stock must be greater than 1", 400);

        //    if (dto.Images == null || dto.Images.Count == 0)
        //        return ApiResponse<ProductDto>.Fail("At least 1 image is required", 400);

        //    if (dto.PrimaryImageIndex < 0 || dto.PrimaryImageIndex >= dto.Images.Count)
        //        return ApiResponse<ProductDto>.Fail("Invalid PrimaryImageIndex", 400);

        //    foreach (var img in dto.Images)
        //    {
        //        if (img?.Stream == null || img.Stream.Length == 0 || string.IsNullOrWhiteSpace(img.FileName))
        //            return ApiResponse<ProductDto>.Fail("Invalid image file", 400);
        //    }

        //    var sizeIds = dto.SizeIds?.Distinct().ToList() ?? new List<int>();
        //    if (sizeIds.Count == 0)
        //        return ApiResponse<ProductDto>.Fail("At least 1 size is required", 400);

        //    // ---------- DB VALIDATIONS ----------
        //    var brandExists = await _db.Brands.AnyAsync(b => b.Id == dto.BrandId && b.IsActive, ct);
        //    if (!brandExists) return ApiResponse<ProductDto>.Fail("Invalid BrandId", 400);

        //    var genderExists = await _db.Genders.AnyAsync(g => g.Id == dto.GenderId && g.IsActive, ct);
        //    if (!genderExists) return ApiResponse<ProductDto>.Fail("Invalid GenderId", 400);

        //    var validSizeIds = await _db.Sizes
        //        .Where(s => sizeIds.Contains(s.Id) && s.IsActive)
        //        .Select(s => s.Id)
        //        .ToListAsync(ct);

        //    if (validSizeIds.Count != sizeIds.Count)
        //        return ApiResponse<ProductDto>.Fail("Invalid SizeIds", 400);

        //    // ---------- CREATE PRODUCT ----------
        //    var product = new Product
        //    {
        //        Name = dto.Name,
        //        Description = dto.Description?.Trim(),
        //        Price = dto.Price,
        //        //Stock = dto.Stock,
        //        BrandId = dto.BrandId,
        //        GenderId = dto.GenderId,
        //        IsActive = true
        //    };

        //    _db.Products.Add(product);
        //    await _db.SaveChangesAsync(ct);

        //    // ---------- SIZES ----------
        //    foreach (var sid in sizeIds)
        //    {
        //        _db.ProductSizes.Add(new ProductSize
        //        {
        //            ProductId = product.Id,
        //            SizeId = sid
        //        });
        //    }

        //    // ---------- IMAGES UPLOAD ----------
        //    for (int i = 0; i < dto.Images.Count; i++)
        //    {
        //        var f = dto.Images[i];

        //        var (url, publicId) = await _cloudinary.UploadAsync(
        //            f.Stream,
        //            f.FileName,
        //            "shoex/products",
        //            ct
        //        );

        //        _db.ProductImages.Add(new ProductImage
        //        {
        //            ProductId = product.Id,
        //            Url = url,
        //            PublicId = publicId
        //        });
        //    }

        //    await _db.SaveChangesAsync(ct);

        //    return await GetByIdAsync(product.Id, ct);
        //}
        public async Task<ApiResponse<ProductDto>> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
        {
            // ---------- BASIC VALIDATIONS ----------
            if (dto == null)
                return ApiResponse<ProductDto>.Fail("Invalid request", 400);

            dto.Name = dto.Name?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(dto.Name))
                return ApiResponse<ProductDto>.Fail("Name is required", 400);

            if (!Regex.IsMatch(dto.Name, @"^[A-Za-z][A-Za-z0-9 ]*$"))
                return ApiResponse<ProductDto>.Fail("Name must start with a letter and contain only letters, digits, and spaces", 400);

            if (dto.Price <= 1)
                return ApiResponse<ProductDto>.Fail("Price must be greater than 1", 400);

            if (dto.Images == null || dto.Images.Count == 0)
                return ApiResponse<ProductDto>.Fail("At least 1 image is required", 400);

            if (dto.PrimaryImageIndex < 0 || dto.PrimaryImageIndex >= dto.Images.Count)
                return ApiResponse<ProductDto>.Fail("Invalid PrimaryImageIndex", 400);

            foreach (var img in dto.Images)
            {
                if (img?.Stream == null || img.Stream.Length == 0 || string.IsNullOrWhiteSpace(img.FileName))
                    return ApiResponse<ProductDto>.Fail("Invalid image file", 400);
            }

            //  Sizes only (no stock in create)
            var sizeIds = dto.SizeIds?.Distinct().ToList() ?? new List<int>();
            if (sizeIds.Count == 0)
                return ApiResponse<ProductDto>.Fail("At least 1 size is required", 400);

            // ---------- DB VALIDATIONS ----------
            var brandExists = await _db.Brands.AnyAsync(b => b.Id == dto.BrandId && b.IsActive, ct);
            if (!brandExists) return ApiResponse<ProductDto>.Fail("Invalid BrandId", 400);

            var genderExists = await _db.Genders.AnyAsync(g => g.Id == dto.GenderId && g.IsActive, ct);
            if (!genderExists) return ApiResponse<ProductDto>.Fail("Invalid GenderId", 400);

            var validSizeIds = await _db.Sizes
                .Where(s => sizeIds.Contains(s.Id) && s.IsActive)
                .Select(s => s.Id)
                .ToListAsync(ct);

            if (validSizeIds.Count != sizeIds.Count)
                return ApiResponse<ProductDto>.Fail("Invalid SizeIds", 400);

            // ---------- CREATE PRODUCT ----------
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description?.Trim(),
                Price = dto.Price,
                BrandId = dto.BrandId,
                GenderId = dto.GenderId,
                IsActive = true
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync(ct);

            // ---------- SIZES (stock default = 0) ----------
            foreach (var sid in sizeIds)
            {
                _db.ProductSizes.Add(new ProductSize
                {
                    ProductId = product.Id,
                    SizeId = sid,
                    Stock = 0
                });
            }

            // ---------- IMAGES UPLOAD ----------
            for (int i = 0; i < dto.Images.Count; i++)
            {
                var f = dto.Images[i];

                var (url, publicId) = await _cloudinary.UploadAsync(
                    f.Stream,
                    f.FileName,
                    "shoex/products",
                    ct
                );

                _db.ProductImages.Add(new ProductImage
                {
                    ProductId = product.Id,
                    Url = url,
                    PublicId = publicId
                });
            }

            await _db.SaveChangesAsync(ct);

            return await GetByIdAsync(product.Id, ct);
        }
   
        // ADMIN: Get product by id (active/inactive)
        public async Task<ApiResponse<ProductDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var p = await _db.Products
                .AsNoTracking()
                .Include(x => x.Brand)
                .Include(x => x.Gender)
                .Include(x => x.Images)
                .Include(x => x.ProductSizes)
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (p == null)
                return ApiResponse<ProductDto>.Fail("Product not found", 404);

            var dto = new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                //Stock = p.Stock,
                Description = p.Description,
                BrandId = p.BrandId,
                GenderId = p.GenderId,
                IsActive = p.IsActive,
                BrandName = p.Brand?.Name ?? "",
                GenderName = p.Gender?.Name ?? "",
                SizeIds = p.ProductSizes.Select(s => s.SizeId).ToList(),
                Images = p.Images.Select(i => new ProductImageDto
                {
                    Id = i.Id,
                    Url = i.Url
                }).ToList()
            };

            return ApiResponse<ProductDto>.Success(dto, "Product fetched");
        }

        // ADMIN: Get ALL products (active + inactive)
        public async Task<ApiResponse<List<ProductDto>>> GetAllAsync(CancellationToken ct = default)
        {
            var list = await _db.Products
                .AsNoTracking()
                .Include(x => x.Brand)
                .Include(x => x.Gender)
                .Include(x => x.Images)
                .Include(x => x.ProductSizes)
                .OrderByDescending(x => x.Id)
                .ToListAsync(ct);

            var data = list.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                //Stock = p.Stock,
                Description = p.Description,
                BrandId = p.BrandId,
                GenderId = p.GenderId,
                IsActive = p.IsActive,
                BrandName = p.Brand?.Name ?? "",
                GenderName = p.Gender?.Name ?? "",
                SizeIds = p.ProductSizes.Select(s => s.SizeId).ToList(),
                Images = p.Images.Select(i => new ProductImageDto { Id = i.Id, Url = i.Url }).ToList()
            }).ToList();

            return ApiResponse<List<ProductDto>>.Success(data, "Products fetched");
        }

        // ADMIN: Update product (fields + sizes)
        public async Task<ApiResponse<ProductDto>> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct = default)
        {
            var p = await _db.Products.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (p == null)
                return ApiResponse<ProductDto>.Fail("Product not found", 404);

            // validate brand/gender
            var brandExists = await _db.Brands.AnyAsync(b => b.Id == dto.BrandId && b.IsActive, ct);
            if (!brandExists) return ApiResponse<ProductDto>.Fail("Invalid BrandId", 400);

            var genderExists = await _db.Genders.AnyAsync(g => g.Id == dto.GenderId && g.IsActive, ct);
            if (!genderExists) return ApiResponse<ProductDto>.Fail("Invalid GenderId", 400);

            // validate sizes
            var sizeIds = dto.SizeIds?.Distinct().ToList() ?? new List<int>();
            if (sizeIds.Count == 0)
                return ApiResponse<ProductDto>.Fail("At least 1 size is required", 400);

            var validSizeIds = await _db.Sizes
                .Where(s => sizeIds.Contains(s.Id) && s.IsActive)
                .Select(s => s.Id)
                .ToListAsync(ct);

            if (validSizeIds.Count != sizeIds.Count)
                return ApiResponse<ProductDto>.Fail("Invalid SizeIds", 400);

            // update product fields
            p.Name = dto.Name.Trim();
            p.Description = dto.Description?.Trim();
            p.Price = dto.Price;
            p.BrandId = dto.BrandId;
            p.GenderId = dto.GenderId;

            //  update sizes WITHOUT losing stock
            var existing = await _db.ProductSizes
                .Where(ps => ps.ProductId == id)
                .ToListAsync(ct);

            // remove sizes that are no longer selected
            var toRemove = existing.Where(x => !sizeIds.Contains(x.SizeId)).ToList();
            _db.ProductSizes.RemoveRange(toRemove);

            // add new sizes (start stock at 0)
            var existingSizeIds = existing.Select(x => x.SizeId).ToHashSet();
            var toAdd = sizeIds.Where(sid => !existingSizeIds.Contains(sid)).ToList();

            foreach (var sid in toAdd)
            {
                _db.ProductSizes.Add(new ProductSize
                {
                    ProductId = id,
                    SizeId = sid,
                    Stock = 0
                });
            }

            // existing sizes remain untouched -> stock stays same
            await _db.SaveChangesAsync(ct);

            return await GetByIdAsync(id, ct);
        }

        // ADMIN: Toggle active/inactive
        public async Task<ApiResponse<string>> ToggleAsync(int id, CancellationToken ct = default)
        {
            var p = await _db.Products.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (p == null) return ApiResponse<string>.Fail("Product not found", 404);

            p.IsActive = !p.IsActive;
            await _db.SaveChangesAsync(ct);

            return ApiResponse<string>.Success(null!, $"Product is now {(p.IsActive ? "Active" : "Inactive")}");
        }

       
        // ADMIN: Soft delete (set inactive)
        public async Task<ApiResponse<string>> DeleteAsync(int id, CancellationToken ct = default)
        {
            var p = await _db.Products.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (p == null) return ApiResponse<string>.Fail("Product not found", 404);

            p.IsActive = false;
            await _db.SaveChangesAsync(ct);

            return ApiResponse<string>.Success(null!, "Product soft deleted");
        }


        public async Task<ApiResponse<string>> UpdateSizeStockAsync(
    int productId, int sizeId, UpdateStockDto dto, CancellationToken ct = default)
        {
            var ps = await _db.ProductSizes
                .FirstOrDefaultAsync(x => x.ProductId == productId && x.SizeId == sizeId, ct);



            if (ps == null)
                return ApiResponse<string>.Fail("Size not linked to this product", 404);
            if (dto.Stock < 0)
                return ApiResponse<string>.Fail("Stock cannot be negative", 400);

            ps.Stock = dto.Stock;

            await _db.SaveChangesAsync(ct);
            return ApiResponse<string>.Success(null, "Size stock updated", 200);
        }


        // Adjust stock (+ / -) for a product size
        public async Task<ApiResponse<string>> AdjustSizeStockAsync(
            int productId,
            int sizeId,
            AdjustStockDto dto,
            CancellationToken ct = default)
        {
            var ps = await _db.ProductSizes
                .FirstOrDefaultAsync(x => x.ProductId == productId && x.SizeId == sizeId, ct);

            if (ps == null)
                return ApiResponse<string>.Fail("Size not linked to this product", 404);

            var newStock = ps.Stock + dto.Delta;
            if (newStock < 0)
                return ApiResponse<string>.Fail("Stock cannot go below 0", 400);

            ps.Stock = newStock;
            await _db.SaveChangesAsync(ct);

            return ApiResponse<string>.Success(null, "Size stock adjusted", 200);
        }


        // ADMIN: Add multiple images (browser upload)
        public async Task<ApiResponse<ProductDto>> AddImagesAsync(int productId, List<IFormFile> images, CancellationToken ct = default)
        {
            if (images == null || images.Count == 0)
                return ApiResponse<ProductDto>.Fail("At least 1 image is required", 400);

            var product = await _db.Products
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == productId, ct);

            if (product == null)
                return ApiResponse<ProductDto>.Fail("Product not found", 404);

            foreach (var file in images)
            {
                if (file == null || file.Length == 0)
                    return ApiResponse<ProductDto>.Fail("Invalid image file", 400);

                using var stream = file.OpenReadStream();
                var (url, publicId) = await _cloudinary.UploadAsync(stream, file.FileName, "shoex/products", ct);

                _db.ProductImages.Add(new ProductImage
                {
                    ProductId = productId,
                    Url = url,
                    PublicId = publicId
                });
            }

            await _db.SaveChangesAsync(ct);

            return await GetByIdAsync(productId, ct);
        }


        // ADMIN: Delete ONE image (DB + Cloudinary)
        
        public async Task<ApiResponse<string>> DeleteImageAsync(int productId, int imageId, CancellationToken ct = default)
        {
            var product = await _db.Products
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == productId, ct);

            if (product == null)
                return ApiResponse<string>.Fail("Product not found", 404);

            var img = product.Images.FirstOrDefault(x => x.Id == imageId);
            if (img == null)
                return ApiResponse<string>.Fail("Image not found", 404);

            if (!string.IsNullOrWhiteSpace(img.PublicId))
                await _cloudinary.DeleteAsync(img.PublicId, ct);

            _db.ProductImages.Remove(img);
            await _db.SaveChangesAsync(ct);

            return ApiResponse<string>.Success(null!, "Image deleted successfully");
        }


    }
}
