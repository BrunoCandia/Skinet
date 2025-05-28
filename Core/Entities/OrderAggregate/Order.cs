namespace Core.Entities.OrderAggregate
{
    public class Order : BaseEntity
    {
        // Regular properties
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
        public required string BuyerEmail { get; set; }

        public Guid DeliveryMethodId { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; } = null!;

        // Owned entities
        public ShippingAddress ShippingAddress { get; set; } = null!;
        public PaymentSummary PaymentSummary { get; set; } = null!;

        // Regular properties
        public decimal Subtotal { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public required string PaymentIntentId { get; set; }

        // Navigation property to related entities
        public IReadOnlyList<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public decimal GetTotal()
        {
            return Subtotal + DeliveryMethod.Price;
        }
    }
}
