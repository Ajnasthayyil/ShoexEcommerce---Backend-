
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;


namespace ShoexEcommerce.Application.DTOs.Product
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        [RegularExpression(@"^[A-Za-z][A-Za-z0-9 ]*$",
            ErrorMessage = "Name must start with a letter and contain only letters, digits, and spaces")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Price is required")]
        [Range(typeof(decimal), "1.01", "999999999", ErrorMessage = "Price must be greater than 1")]
        public decimal Price { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "BrandId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "BrandId must be valid")]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "GenderId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "GenderId must be valid")]
        public int GenderId { get; set; }

        [Required(ErrorMessage = "At least one size is required")]
        [MinLength(1, ErrorMessage = "At least one size is required")]
        public List<int> SizeIds { get; set; } = new();

        [Required(ErrorMessage = "At least 3 images are required")]
        [MinLength(3, ErrorMessage = "At least 3 images are required")]
        public List<FileUpload> Images { get; set; } = new();

        [Range(0, int.MaxValue, ErrorMessage = "PrimaryImageIndex must be 0 or greater")]
        public int PrimaryImageIndex { get; set; } = 0;
    }

    public class FileUpload
    {
        public Stream Stream { get; }
        public string FileName { get; }

        public FileUpload(Stream stream, string fileName)
        {
            Stream = stream;
            FileName = fileName;
        }
    }
}