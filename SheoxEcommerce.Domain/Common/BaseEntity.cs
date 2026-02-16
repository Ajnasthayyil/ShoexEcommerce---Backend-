namespace ShoexEcommerce.Domain.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = "system";

        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
