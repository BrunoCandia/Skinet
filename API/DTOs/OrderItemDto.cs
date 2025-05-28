namespace API.DTOs
{
    public class OrderItemDto
    {
        public Guid Id { get; set; }

        // Owned entities
        public ProductItemOrderedDto ItemOrdered { get; set; } = null!;

        // Regular properties
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
