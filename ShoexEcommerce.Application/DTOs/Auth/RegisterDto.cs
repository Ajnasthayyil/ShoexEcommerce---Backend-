using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ShoexEcommerce.Application.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [MinLength(3, ErrorMessage = "Full name must be at least 3 characters")]
        [MaxLength(100, ErrorMessage = "Full name must be at most 100 characters")]
        [RegularExpression(@"^[A-Za-z]+(?: [A-Za-z]+)*$",
            ErrorMessage = "Full name must contain alphabets only. Spaces allowed (no special characters).")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(150)]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [RegularExpression(@"^[a-z][a-z0-9._%+\-]*@[a-z0-9.\-]+\.[a-z]{2,}$",
            ErrorMessage = "Email must start with a lowercase letter and be in lowercase.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Mobile number is required")]
        [RegularExpression(@"^[1-9][0-9]{9}$",
        ErrorMessage = "Mobile number must be exactly 10 digits and cannot start with 0")]
        public string MobileNumber { get; set; } = null!;


        [Required(ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
        [MaxLength(30, ErrorMessage = "Username must be at most 30 characters")]
        [RegularExpression(
             @"^[A-Za-z0-9]\S*$",
             ErrorMessage = "Username cannot contain whitespace and must start with a letter or digit"
         )]
        public string Username { get; set; } = null!;



        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [MaxLength(100)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{6,}$",
            ErrorMessage = "Password must have at least 1 uppercase, 1 lowercase, 1 digit, 1 special character, and minimum 6 length.")]
        public string Password { get; set; } = null!;
    }
}
