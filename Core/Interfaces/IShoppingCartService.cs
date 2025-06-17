using Core.Entities;
using System.Threading;

namespace Core.Interfaces
{
    public interface IShoppingCartService
    {
        Task<ShoppingCart?> GetShoppingCartAsync(string key, CancellationToken cancellationToken = default);
        Task<ShoppingCart?> SetShoppingCartAsync(ShoppingCart shoppingCart, CancellationToken cancellationToken = default);
        Task<bool> DeleteShoppingCartAsync(string key, CancellationToken cancellationToken = default);
    }
}
