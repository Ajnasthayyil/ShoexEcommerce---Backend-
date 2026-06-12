namespace ShoexEcommerce.Application.DTOs.Product
{
    public class ProductReviewDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public int Rating { get; set; }
        public string? ReviewText { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
