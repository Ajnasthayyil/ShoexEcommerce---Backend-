using Microsoft.EntityFrameworkCore;
using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.DTOs.Address;
using ShoexEcommerce.Application.Interfaces.Address;
using ShoexEcommerce.Domain.Entities;
using ShoexEcommerce.Infrastructure.Data;

namespace ShoexEcommerce.Infrastructure.Services
{
    public class AddressService : IAddressService
    {
        private readonly AppDbContext _db;
        public AddressService(AppDbContext db) => _db = db;

        public async Task<ApiResponse<AddressDto>> AddAsync(int userId, AddAddressDto dto, CancellationToken ct = default)
        {
            var userExists = await _db.Users.AnyAsync(x => x.Id == userId && x.IsActive, ct);
            if (!userExists) return ApiResponse<AddressDto>.Fail("User not found", 404);

            if (dto.IsDefault)
            {
                var currentDefaults = await _db.Addresses
                    .Where(x => x.UserId == userId && x.IsActive && x.IsDefault)
                    .ToListAsync(ct);
                foreach (var a in currentDefaults) a.IsDefault = false;
            }

            // if user has no address, make first address default
            var hasAny = await _db.Addresses.AnyAsync(x => x.UserId == userId && x.IsActive, ct);
            var isDefault = dto.IsDefault || !hasAny;

            var entity = new Address
            {
                UserId = userId,
                FullName = dto.FullName.Trim(),
                PhoneNumber = dto.PhoneNumber.Trim(),
                Street = dto.Street.Trim(),
                City = dto.City.Trim(),
                Pincode = dto.Pincode.Trim(),
                State = dto.State.Trim(),
                IsDefault = isDefault,
                IsActive = true
            };

            _db.Addresses.Add(entity);
            await _db.SaveChangesAsync(ct);

            return ApiResponse<AddressDto>.Success(new AddressDto
            {
                Id = entity.Id,
                FullName = entity.FullName,
                PhoneNumber = entity.PhoneNumber,
                Street = entity.Street,
                City = entity.City,
                Pincode = entity.Pincode,
                State = entity.State,
                IsDefault = entity.IsDefault
            }, "Address added", 201);
        }

        public async Task<ApiResponse<List<AddressDto>>> GetMyAsync(int userId, CancellationToken ct = default)
        {
            var list = await _db.Addresses.AsNoTracking()
                .Where(x => x.UserId == userId && x.IsActive)
                .OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.CreatedOn)
                .Select(x => new AddressDto
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    PhoneNumber = x.PhoneNumber,
                    Street = x.Street,
                    City = x.City,
                    Pincode = x.Pincode,
                    State = x.State,
                    IsDefault = x.IsDefault
                })
                .ToListAsync(ct);

            return ApiResponse<List<AddressDto>>.Success(list, "My addresses", 200);
        }

        public async Task<ApiResponse<string>> UpdateAsync(int userId, int addressId, AddAddressDto dto, CancellationToken ct = default)
        {
            var entity = await _db.Addresses
                .FirstOrDefaultAsync(x => x.Id == addressId && x.UserId == userId && x.IsActive, ct);

            if (entity == null) return ApiResponse<string>.Fail("Address not found", 404);

            entity.FullName = dto.FullName.Trim();
            entity.PhoneNumber = dto.PhoneNumber.Trim();
            entity.Street = dto.Street.Trim();
            entity.City = dto.City.Trim();
            entity.Pincode = dto.Pincode.Trim();
            entity.State = dto.State.Trim();

            if (dto.IsDefault && !entity.IsDefault)
            {
                var currentDefaults = await _db.Addresses
                    .Where(x => x.UserId == userId && x.IsActive && x.IsDefault)
                    .ToListAsync(ct);
                foreach (var a in currentDefaults) a.IsDefault = false;

                entity.IsDefault = true;
            }

            await _db.SaveChangesAsync(ct);
            return ApiResponse<string>.Success(null, "Address updated", 200);
        }

        public async Task<ApiResponse<string>> DeleteAsync(int userId, int id, CancellationToken ct = default)
        {
            var address = await _db.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId, ct);

            if (address == null)
                return ApiResponse<string>.Fail("Address not found", 404);

            _db.Addresses.Remove(address);
            await _db.SaveChangesAsync(ct);

            return ApiResponse<string>.Success(null, "Address deleted permanently", 200);
        }

        public async Task<ApiResponse<string>> SetDefaultAsync(int userId, int addressId, CancellationToken ct = default)
        {
            var entity = await _db.Addresses
                .FirstOrDefaultAsync(x => x.Id == addressId && x.UserId == userId && x.IsActive, ct);

            if (entity == null) return ApiResponse<string>.Fail("Address not found", 404);

            var currentDefaults = await _db.Addresses
                .Where(x => x.UserId == userId && x.IsActive && x.IsDefault)
                .ToListAsync(ct);
            foreach (var a in currentDefaults) a.IsDefault = false;

            entity.IsDefault = true;
            await _db.SaveChangesAsync(ct);

            return ApiResponse<string>.Success(null, "Default address updated", 200);
        }
    }
}
