using Core.Interfaces;
using Stripe;

namespace Infrastructure.Services
{
    public class CouponService : ICouponService
    {
        public async Task<Core.Entities.Coupon?> GetCouponFromPromoCodeAsync(string code)
        {
            var promotionService = new PromotionCodeService();

            var options = new PromotionCodeListOptions
            {
                Code = code
            };

            var promotionCodes = await promotionService.ListAsync(options);

            var promotionCode = promotionCodes.FirstOrDefault();

            if (promotionCode != null && promotionCode.Coupon != null)
            {
                return new Core.Entities.Coupon
                {
                    Name = promotionCode.Coupon.Name,
                    AmountOff = promotionCode.Coupon.AmountOff,
                    PercentOff = promotionCode.Coupon.PercentOff,
                    Id = promotionCode.Coupon.Id,
                    Code = promotionCode.Code
                };
            }

            return null;
        }
    }
}
