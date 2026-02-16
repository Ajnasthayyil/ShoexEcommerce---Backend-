using ShoexEcommerce.Domain.Common;

namespace ShoexEcommerce.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; } = null!;   
        public DateTime ExpiresOn { get; set; }
        public bool IsRevoked { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;      
    }
}
