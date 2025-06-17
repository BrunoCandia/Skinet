using Core.Entities;
using System.Threading;

namespace Core.Interfaces
{
    public interface ICouponService
    {
        Task<Coupon?> GetCouponFromPromoCodeAsync(string code, CancellationToken cancellationToken = default);
    }
}
