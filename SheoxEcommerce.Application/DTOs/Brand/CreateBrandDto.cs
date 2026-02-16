using System.ComponentModel.DataAnnotations;

namespace ShoexEcommerce.Application.DTOs.Brand
{
    public class CreateBrandDto
    {
        [Required(ErrorMessage = "Brand name is required")]
        [MaxLength(100, ErrorMessage = "Brand name must be at most 100 characters")]
        [MinLength(2, ErrorMessage = "Brand name must be at least 2 characters")]
        [RegularExpression(@"^[A-Za-z]+(?: [A-Za-z]+)*$",
            ErrorMessage = "Brand name must contain alphabets only. Spaces allowed (no special characters).")]
        public string Name { get; set; } = null!;
    }
}
