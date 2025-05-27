namespace Core.Entities.OrderAggregate
{
    public class OrderItem : BaseEntity
    {
        // Owned entities
        public ProductItemOrdered ItemOrdered { get; set; } = null!;

        // Regular properties
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
