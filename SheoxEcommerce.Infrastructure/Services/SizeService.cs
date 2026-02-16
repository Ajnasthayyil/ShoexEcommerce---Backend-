using Microsoft.EntityFrameworkCore;
using SheoxEcommerce.Application.DTOs.Size;
using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.Interfaces.Product;
using ShoexEcommerce.Domain.Entities;
using ShoexEcommerce.Infrastructure.Data;

namespace ShoexEcommerce.Infrastructure.Services
{
    public class SizeService : ISizeService
    {
        private readonly AppDbContext _db;

        public SizeService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<SizeDto>> CreateAsync(CreateSizeDto dto)
        {
            var name = dto.Name.Trim();

            var exists = await _db.Sizes.AnyAsync(x => x.Name.ToLower() == name.ToLower());
            if (exists) return ApiResponse<SizeDto>.Fail("Size already exists", 400);

            var size = new Size
            {
                Name = name,
                IsActive = true
            };

            _db.Sizes.Add(size);
            await _db.SaveChangesAsync();

            return ApiResponse<SizeDto>.Success(new SizeDto
            {
                Id = size.Id,
                Name = size.Name,
                IsActive = size.IsActive
            }, "Size created", 201);
        }

        public async Task<ApiResponse<List<SizeDto>>> GetAllAsync()
        {
            var list = await _db.Sizes.AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Select(x => new SizeDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive
                })
                .ToListAsync();

            return ApiResponse<List<SizeDto>>.Success(list, "Sizes fetched", 200);
        }

        public async Task<ApiResponse<List<SizeDto>>> GetActiveAsync()
        {
            var list = await _db.Sizes.AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new SizeDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive
                })
                .ToListAsync();

            return ApiResponse<List<SizeDto>>.Success(list, "Active sizes fetched", 200);
        }

        public async Task<ApiResponse<SizeDto>> ToggleAsync(int id)
        {
            var size = await _db.Sizes.FirstOrDefaultAsync(x => x.Id == id);
            if (size == null) return ApiResponse<SizeDto>.Fail("Size not found", 404);

            size.IsActive = !size.IsActive;
            await _db.SaveChangesAsync();

            return ApiResponse<SizeDto>.Success(new SizeDto
            {
                Id = size.Id,
                Name = size.Name,
                IsActive = size.IsActive
            }, "Size status updated", 200);
        }
    }
}
