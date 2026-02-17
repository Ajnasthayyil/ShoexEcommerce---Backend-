using System.ComponentModel.DataAnnotations;

namespace ShoexEcommerce.Application.DTOs.Address
{
    public class AddAddressDto
    {
        private const string StartsWithLetterPattern = @"^[A-Za-z][A-Za-z0-9\s,.-]*$";

        [Required(ErrorMessage = "Full name is required")]
        [MinLength(3, ErrorMessage = "Full name must be at least 3 characters")]
        [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        [RegularExpression(
            StartsWithLetterPattern,
            ErrorMessage = "Full name must start with a letter and cannot start with a number or special character"
        )]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(
            @"^[6-9]\d{9}$",
            ErrorMessage = "Phone number must be a valid 10-digit Indian mobile number"
        )]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Street / Apartment is required")]
        [MinLength(5, ErrorMessage = "Street must be at least 5 characters")]
        [MaxLength(200, ErrorMessage = "Street cannot exceed 200 characters")]
        [RegularExpression(
            StartsWithLetterPattern,
            ErrorMessage = "Street must start with a letter and cannot start with a number or special character"
        )]
        public string Street { get; set; } = null!;

        [Required(ErrorMessage = "City is required")]
        [MinLength(2, ErrorMessage = "City must be at least 2 characters")]
        [MaxLength(50, ErrorMessage = "City cannot exceed 50 characters")]
        [RegularExpression(
            StartsWithLetterPattern,
            ErrorMessage = "City must start with a letter and cannot start with a number or special character"
        )]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "Pincode is required")]
        [RegularExpression(
            @"^\d{6}$",
            ErrorMessage = "Pincode must be exactly 6 digits"
        )]
        public string Pincode { get; set; } = null!;

        [Required(ErrorMessage = "State is required")]
        [MinLength(2, ErrorMessage = "State must be at least 2 characters")]
        [MaxLength(50, ErrorMessage = "State cannot exceed 50 characters")]
        [RegularExpression(
            StartsWithLetterPattern,
            ErrorMessage = "State must start with a letter and cannot start with a number or special character"
        )]
        public string State { get; set; } = null!;

        public bool IsDefault { get; set; } = false;
    }
}
