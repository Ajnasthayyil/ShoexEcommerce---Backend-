using ShoexEcommerce.Domain.Common;

namespace ShoexEcommerce.Domain.Entities
{
    public class Address : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Pincode { get; set; } = null!;
        public string State { get; set; } = null!;

        public bool IsDefault { get; set; } = false;
    }
}
