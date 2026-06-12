using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoexEcommerce.API.Requests.Product
{
    public class CreateProductRequest
    {
        [Required(ErrorMessage = "Product name is required")]
        [MinLength(3, ErrorMessage = "Product name must be at least 3 characters")]
        [MaxLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
        [RegularExpression(@"^[A-Za-z][A-Za-z0-9\s\-]*$",
            ErrorMessage = "Product name must start with a letter and contain only letters, numbers, spaces or hyphens")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Price is required")]
        [Range(1, 1000000, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid BrandId")]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid GenderId")]
        public int GenderId { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "At least one size must be selected")]
        [MinLength(1, ErrorMessage = "At least one size must be selected")]
        public List<int> SizeIds { get; set; } = new();

        [Required(ErrorMessage = "At least one image is required")]
        [MinLength(1, ErrorMessage = "At least one image is required")]
        public List<IFormFile> Images { get; set; } = new();

        [Range(0, int.MaxValue, ErrorMessage = "Invalid primary image index")]
        public int PrimaryImageIndex { get; set; } = 0;
    }
}