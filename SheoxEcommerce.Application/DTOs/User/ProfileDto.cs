namespace ShoexEcommerce.Application.DTOs.User
{
    public class ProfileDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string MobileNumber { get; set; } = "";
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
