using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoexEcommerce.Application.DTOs.Product
{
    public class UpdateProductDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Name must be between 2 and 100 characters")]
        [RegularExpression(@"^[A-Za-z][A-Za-z0-9 ]*$",
            ErrorMessage = "Name must start with a letter and contain only letters, digits, and spaces")]
        public string Name { get; set; } = null!;


        [Required(ErrorMessage = "Price is required")]
        [Range(typeof(decimal), "1.01", "999999999",
            ErrorMessage = "Price must be greater than 1")]
        public decimal Price { get; set; }

        [StringLength(1000,
            ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "BrandId is required")]
        [Range(1, int.MaxValue,
            ErrorMessage = "BrandId must be valid")]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "GenderId is required")]
        [Range(1, int.MaxValue,
            ErrorMessage = "GenderId must be valid")]
        public int GenderId { get; set; }

        [Required(ErrorMessage = "At least one size is required")]
        [MinLength(1,
            ErrorMessage = "At least one size is required")]
        public List<int> SizeIds { get; set; } = new();
    }
}