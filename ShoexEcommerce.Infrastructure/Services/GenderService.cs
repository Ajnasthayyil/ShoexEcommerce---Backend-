using Microsoft.EntityFrameworkCore;
using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.DTOs.Gender;
using ShoexEcommerce.Application.Interfaces.Gender;
using ShoexEcommerce.Infrastructure.Data;

namespace ShoexEcommerce.Infrastructure.Services
{
    public class GenderService : IGenderService
    {
        private readonly AppDbContext _db;

        public GenderService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<List<GenderDto>>> GetAllAsync()
        {
            var list = await _db.Genders
                .Select(x => new GenderDto
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .ToListAsync();

            return ApiResponse<List<GenderDto>>.Success(list, "Genders fetched", 200);
        }

        public async Task<ApiResponse<string>> ToggleActiveAsync(int id)
        {
            var gender = await _db.Genders.FindAsync(id);
            if (gender == null)
                return ApiResponse<string>.Fail("Gender not found", 404);

            gender.IsActive = !gender.IsActive;
            await _db.SaveChangesAsync();

            return ApiResponse<string>.Success(null,
                gender.IsActive ? "Gender activated" : "Gender deactivated",
                200);
        }
    }
}
