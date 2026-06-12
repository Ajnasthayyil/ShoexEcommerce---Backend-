using System.ComponentModel.DataAnnotations;

public class UpdateProfileDto
{
    [Required(ErrorMessage = "Full name is required")]
    [MinLength(3, ErrorMessage = "Full name must be at least 3 characters")]
    [MaxLength(100, ErrorMessage = "Full name must be at most 100 characters")]
    [RegularExpression(@"^[A-Za-z]+(?: [A-Za-z]+)*$",
        ErrorMessage = "Full name must contain alphabets only. Spaces allowed.")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [MaxLength(150)]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [RegularExpression(@"^[a-z][a-z0-9._%+\-]*@[a-z0-9.\-]+\.[a-z]{2,}$",
        ErrorMessage = "Email must be lowercase and start with a letter.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Mobile number is required")]
    [RegularExpression(@"^\d{10}$",
        ErrorMessage = "Mobile number must be exactly 10 digits")]
    public string MobileNumber { get; set; } = null!;
}
