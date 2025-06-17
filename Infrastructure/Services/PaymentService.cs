using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;
using Entities = Core.Entities;
using System.Threading;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IGenericRepository<Entities.Product> _productRepository;
        private readonly IGenericRepository<Entities.DeliveryMethod> _deliveMethodRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(
            IConfiguration configuration,
            IShoppingCartService shoppingCartService,
            IGenericRepository<Entities.Product> productRepository,
            IGenericRepository<Entities.DeliveryMethod> deliveMethodRepository,
            IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _shoppingCartService = shoppingCartService;
            _productRepository = productRepository;
            _deliveMethodRepository = deliveMethodRepository;
            _unitOfWork = unitOfWork;

            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];
        }

        public async Task<string> RefundPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default)
        {
            var refundOptions = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId
            };

            var refundService = new RefundService();

            var result = await refundService.CreateAsync(refundOptions, cancellationToken: cancellationToken);

            return result.Status;
        }

        public async Task<ShoppingCart?> CreateOrUpdatePaymentIntentAsync(string shoppingCartId, CancellationToken cancellationToken = default)
        {
            var shoppingCart = await _shoppingCartService.GetShoppingCartAsync(shoppingCartId, cancellationToken) ?? throw new Exception("Shopping Cart unavailable");

            var shippingPrice = await GetShippingPriceAsync(shoppingCart, cancellationToken) ?? 0;

            await ValidateCartItemsInCartAsync(shoppingCart, cancellationToken);

            var subtotal = CalculateSubtotal(shoppingCart);

            if (shoppingCart.Coupon != null)
            {
                subtotal = await ApplyDiscountAsync(shoppingCart.Coupon, subtotal, cancellationToken);
            }

            var total = subtotal + shippingPrice;

            await CreateUpdatePaymentIntentAsync(shoppingCart, total, cancellationToken);

            await _shoppingCartService.SetShoppingCartAsync(shoppingCart, cancellationToken);

            return shoppingCart;
        }

        private async Task CreateUpdatePaymentIntentAsync(ShoppingCart cart, long total, CancellationToken cancellationToken = default)
        {
            var service = new PaymentIntentService();

            if (string.IsNullOrWhiteSpace(cart.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = total,
                    Currency = "usd",
                    PaymentMethodTypes = ["card"]
                };

                var intent = await service.CreateAsync(options, cancellationToken: cancellationToken);
                cart.PaymentIntentId = intent.Id;
                cart.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = total
                };

                await service.UpdateAsync(cart.PaymentIntentId, options, cancellationToken: cancellationToken);
            }
        }

        private static async Task<long> ApplyDiscountAsync(Entities.Coupon coupon, long amount, CancellationToken cancellationToken = default)
        {
            var couponService = new Stripe.CouponService();

            var stripeCoupon = await couponService.GetAsync(coupon.Id, cancellationToken: cancellationToken);

            if (stripeCoupon.AmountOff.HasValue)
            {
                amount -= (long)stripeCoupon.AmountOff * 100;
            }

            if (stripeCoupon.PercentOff.HasValue)
            {
                var discount = amount * (stripeCoupon.PercentOff.Value / 100);
                amount -= (long)discount;
            }

            return amount;
        }

        private static long CalculateSubtotal(ShoppingCart shoppingCart)
        {
            var itemTotal = shoppingCart.Items.Sum(x => x.Quantity * x.Price * 100);

            return (long)itemTotal;
        }

        private async Task ValidateCartItemsInCartAsync(ShoppingCart shoppingCart, CancellationToken cancellationToken = default)
        {
            foreach (var item in shoppingCart.Items)
            {
                var productItem = await _unitOfWork.Repository<Core.Entities.Product>().GetByIdAsync(item.ProductId, cancellationToken) ?? throw new Exception("Problem getting product in cart");

                if (item.Price != productItem.Price)
                {
                    item.Price = productItem.Price;
                }
            }
        }

        private async Task<long?> GetShippingPriceAsync(ShoppingCart shoppingCart, CancellationToken cancellationToken = default)
        {
            if (shoppingCart.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync((Guid)shoppingCart.DeliveryMethodId, cancellationToken) ?? throw new Exception("Problem with delivery method");

                return (long)deliveryMethod.Price * 100;
            }

            return null;
        }
    }
}
