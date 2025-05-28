using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class CreateOrderDto
    {
        [Required]
        public string ShoppingCartId { get; set; } = string.Empty;

        [Required]
        public Guid DeliveryMethodId { get; set; }

        [Required]
        public ShippingAddressDto ShippingAddress { get; set; } = null!;

        [Required]
        public PaymentSummaryDto PaymentSummary { get; set; } = null!;
    }
}
