using ShoexEcommerce.Application.Common;
using ShoexEcommerce.Application.DTOs.Gender;

namespace ShoexEcommerce.Application.Interfaces.Gender
{
    public interface IGenderService
    {
        Task<ApiResponse<List<GenderDto>>> GetAllAsync();
        Task<ApiResponse<string>> ToggleActiveAsync(int id);
    }
}
