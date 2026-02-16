using Microsoft.EntityFrameworkCore;
using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.DTOs.Brand;
using ShoexEcommerce.Application.Interfaces.Brand;
using ShoexEcommerce.Domain.Entities;
using ShoexEcommerce.Infrastructure.Data;
using System.Text.RegularExpressions;

namespace ShoexEcommerce.Infrastructure.Services
{
    public class BrandService : IBrandService
    {
        private readonly AppDbContext _db;

        public BrandService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<List<BrandDto>>> GetAllAsync()
        {
            var list = await _db.Brands
                .OrderByDescending(x => x.Id)
                .Select(x => new BrandDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive
                })
                .ToListAsync();

            return ApiResponse<List<BrandDto>>.Success(list, "Brands fetched", 200);
        }

        public async Task<ApiResponse<BrandDto>> GetByIdAsync(int id)
        {
            var brand = await _db.Brands.FirstOrDefaultAsync(x => x.Id == id);
            if (brand == null)
                return ApiResponse<BrandDto>.Fail("Brand not found", 404);

            var dto = new BrandDto
            {
                Id = brand.Id,
                Name = brand.Name,
                IsActive = brand.IsActive
            };

            return ApiResponse<BrandDto>.Success(dto, "Brand fetched", 200);
        }

        public async Task<ApiResponse<BrandDto>> CreateAsync(CreateBrandDto dto)
        {
            var rawName = string.Join(" ", dto.Name.Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries));

            var norm = NormalizeName(dto.Name);

            var exists = await _db.Brands.AnyAsync(x => x.Name.ToLower() == norm);
            if (exists)
                return ApiResponse<BrandDto>.Fail("Brand already exists", 409);

            var brand = new Brand { Name = rawName, IsActive = true };

            _db.Brands.Add(brand);
            await _db.SaveChangesAsync();

            return ApiResponse<BrandDto>.Success(new BrandDto
            {
                Id = brand.Id,
                Name = brand.Name,
                IsActive = brand.IsActive
            }, "Brand created", 201);
        }

        public async Task<ApiResponse<BrandDto>> UpdateAsync(int id, BrandUpdateDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
                return ApiResponse<BrandDto>.Fail("Brand name is required", 400);

            var brand = await _db.Brands.FirstOrDefaultAsync(x => x.Id == id);
            if (brand == null)
                return ApiResponse<BrandDto>.Fail("Brand not found", 404);

            // keep user's casing, just clean extra spaces
            var rawName = string.Join(" ", dto.Name.Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries));

            // SQL translatable normalization for duplicate check
            var norm = rawName.Trim().ToLower();

            var duplicate = await _db.Brands.AnyAsync(x =>
                x.Id != id &&
                x.Name.Trim().ToLower() == norm
            );

            if (duplicate)
                return ApiResponse<BrandDto>.Fail("Another brand with same name exists", 409);

            // update even if only casing changed
            brand.Name = rawName;

            // Force update for case-only change (SQL collation may ignore case)
            _db.Entry(brand).Property(x => x.Name).IsModified = true;

            await _db.SaveChangesAsync();

            return ApiResponse<BrandDto>.Success(new BrandDto
            {
                Id = brand.Id,
                Name = brand.Name,
                IsActive = brand.IsActive
            }, "Brand updated", 200);
        }




        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var brand = await _db.Brands.FirstOrDefaultAsync(x => x.Id == id);
            if (brand == null)
                return ApiResponse<string>.Fail("Brand not found", 404);

            _db.Brands.Remove(brand);
            await _db.SaveChangesAsync();

            return ApiResponse<string>.Success(null, "Brand deleted", 200);
        }

        public async Task<ApiResponse<string>> ToggleActiveAsync(int id)
        {
            var brand = await _db.Brands.FirstOrDefaultAsync(x => x.Id == id);
            if (brand == null)
                return ApiResponse<string>.Fail("Brand not found", 404);

            brand.IsActive = !brand.IsActive;
            await _db.SaveChangesAsync();

            return ApiResponse<string>.Success(
                null,
                brand.IsActive ? "Brand activated" : "Brand deactivated",
                200
            );
        }

        private static string NormalizeName(string name)
        {
            name = (name ?? "").Trim();

            name = Regex.Replace(name, @"\s+", " ");

            return name.ToLowerInvariant();
        }
    }
}
