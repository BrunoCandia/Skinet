namespace Core.Entities.OrderAggregate
{
    /// <summary>
    /// Owned entity representing the payment summary for an order.
    /// </summary>
    public class PaymentSummary
    {
        public int Last4 { get; set; }
        public required string Brand { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
    }
}
