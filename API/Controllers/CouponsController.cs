using API.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CouponsController : BaseApiController
    {
        private readonly ICouponService _couponService;

        public CouponsController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<CouponDto>> ValidateCoupon(string code, CancellationToken cancellationToken)
        {
            var coupon = await _couponService.GetCouponFromPromoCodeAsync(code, cancellationToken);

            if (coupon == null) return BadRequest("Invalid voucher code");

            var couponDto = new CouponDto
            {
                Id = coupon.Id,
                Name = coupon.Name,
                Code = coupon.Code,
                AmountOff = coupon.AmountOff,
                PercentOff = coupon.PercentOff,
            };

            return couponDto;
        }
    }
}
