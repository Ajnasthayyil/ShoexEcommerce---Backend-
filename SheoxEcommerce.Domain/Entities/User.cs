using ShoexEcommerce.Domain.Common;

namespace ShoexEcommerce.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;

        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;
        public int RoleId { get; set; }  // backend sets this (2 for User)
        public Role Role { get; set; } = null!;

        public bool IsBlocked { get; set; } = false;

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
