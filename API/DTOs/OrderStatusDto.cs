namespace API.DTOs
{
    public enum OrderStatusDto
    {
        Pending,
        PaymentReceived,
        PaymentFailed,
        PaymentMismatch,
        Refunded
    }
}
