using Core.Entities;
using System.Threading;

namespace Core.Interfaces
{
    public interface IPaymentService
    {
        Task<ShoppingCart?> CreateOrUpdatePaymentIntentAsync(string shoppingCartId, CancellationToken cancellationToken = default);
        Task<string> RefundPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default);
    }
}
