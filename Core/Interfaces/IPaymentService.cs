using Core.Entities;

namespace Core.Interfaces
{
    public interface IPaymentService
    {
        Task<ShoppingCart?> CreateOrUpdatePaymentIntentAsync(string shoppingCartId);
        Task<string> RefundPaymentAsync(string paymentIntentId);
    }
}
