namespace ShoexEcommerce.Application.DTOs.Address
{
    public class AddressDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Pincode { get; set; } = null!;
        public string State { get; set; } = null!;
        public bool IsDefault { get; set; }
    }
}
