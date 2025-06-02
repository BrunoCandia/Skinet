using Core.Entities;

namespace Core.Interfaces
{
    public interface ICouponService
    {
        Task<Coupon?> GetCouponFromPromoCodeAsync(string code);
    }
}
