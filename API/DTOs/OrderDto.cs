namespace API.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }

        public DateTimeOffset OrderDate { get; set; }
        public required string BuyerEmail { get; set; }

        public Guid DeliveryMethodId { get; set; }
        public DeliveryMethodDto DeliveryMethod { get; set; } = null!;
        ////public required string DeliveryMethod { get; set; }

        // Owned entities
        public required ShippingAddressDto ShippingAddress { get; set; }
        public required PaymentSummaryDto PaymentSummary { get; set; }

        // Regular properties
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public OrderStatusDto Status { get; set; }
        public required string StatusString { get; set; }
        public required string PaymentIntentId { get; set; }

        // Navigation property to related entities
        public IReadOnlyList<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();

        public decimal Total { get; set; }
    }
}
