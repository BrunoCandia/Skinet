namespace API.DTOs
{
    public class PaymentSummaryDto
    {
        public int Last4 { get; set; }
        public required string Brand { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
    }
}