using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;
using Entities = Core.Entities;

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

        public async Task<string> RefundPaymentAsync(string paymentIntentId)
        {
            var refundOptions = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId
            };

            var refundService = new RefundService();

            var result = await refundService.CreateAsync(refundOptions);

            return result.Status;
        }

        public async Task<ShoppingCart?> CreateOrUpdatePaymentIntentAsync(string shoppingCartId)
        {
            ////StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

            var shoppingCart = await _shoppingCartService.GetShoppingCartAsync(shoppingCartId) ?? throw new Exception("Shopping Cart unavailable");

            var shippingPrice = await GetShippingPriceAsync(shoppingCart) ?? 0;

            await ValidateCartItemsInCartAsync(shoppingCart);

            var subtotal = CalculateSubtotal(shoppingCart);

            if (shoppingCart.Coupon != null)
            {
                subtotal = await ApplyDiscountAsync(shoppingCart.Coupon, subtotal);
            }

            var total = subtotal + shippingPrice;

            await CreateUpdatePaymentIntentAsync(shoppingCart, total);

            await _shoppingCartService.SetShoppingCartAsync(shoppingCart);

            return shoppingCart;
        }

        public async Task<string> RefundPayment(string paymentIntentId)
        {
            var refundOptions = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId
            };

            var refundService = new RefundService();
            var result = await refundService.CreateAsync(refundOptions);

            return result.Status;
        }

        private async Task CreateUpdatePaymentIntentAsync(ShoppingCart cart, long total)
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

                var intent = await service.CreateAsync(options);
                cart.PaymentIntentId = intent.Id;
                cart.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = total
                };

                await service.UpdateAsync(cart.PaymentIntentId, options);
            }
        }

        private static async Task<long> ApplyDiscountAsync(Entities.Coupon coupon, long amount)
        {
            var couponService = new Stripe.CouponService();

            var stripeCoupon = await couponService.GetAsync(coupon.Id);

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

        private async Task ValidateCartItemsInCartAsync(ShoppingCart shoppingCart)
        {
            foreach (var item in shoppingCart.Items)
            {
                var productItem = await _unitOfWork.Repository<Core.Entities.Product>().GetByIdAsync(item.ProductId) ?? throw new Exception("Problem getting product in cart");

                if (item.Price != productItem.Price)
                {
                    item.Price = productItem.Price;
                }
            }
        }

        private async Task<long?> GetShippingPriceAsync(ShoppingCart shoppingCart)
        {
            if (shoppingCart.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync((Guid)shoppingCart.DeliveryMethodId) ?? throw new Exception("Problem with delivery method");

                return (long)deliveryMethod.Price * 100;
            }

            return null;
        }

        ////public async Task<Entities.ShoppingCart?> CreateOrUpdatePaymentIntentAsync(string shoppingCartId)
        ////{
        ////    StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

        ////    var shoppingCart = await _shoppingCartService.GetShoppingCartAsync(shoppingCartId);

        ////    if (shoppingCart is null)
        ////    {
        ////        return null;
        ////    }

        ////    var shippingPrice = 0m;

        ////    if (shoppingCart.DeliveryMethodId.HasValue)
        ////    {
        ////        var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(shoppingCart.DeliveryMethodId.Value);
        ////        ////var deliveryMethod = await _deliveMethodRepository.GetByIdAsync(shoppingCart.DeliveryMethodId.Value);

        ////        if (deliveryMethod is null)
        ////        {
        ////            return null;
        ////        }

        ////        shippingPrice = deliveryMethod.Price;
        ////    }

        ////    foreach (var item in shoppingCart.Items)
        ////    {
        ////        var product = await _unitOfWork.Repository<Entities.Product>().GetByIdAsync(item.ProductId);
        ////        ////var product = await _productRepository.GetByIdAsync(item.ProductId);

        ////        if (product is null)
        ////        {
        ////            return null;
        ////        }

        ////        if (item.Price != product.Price)
        ////        {
        ////            item.Price = product.Price;
        ////        }
        ////    }

        ////    var paymentIntentService = new PaymentIntentService();
        ////    PaymentIntent? intent;

        ////    if (string.IsNullOrWhiteSpace(shoppingCart.PaymentIntentId))
        ////    {
        ////        var options = new PaymentIntentCreateOptions
        ////        {
        ////            Amount = (long)shoppingCart.Items.Sum(item => item.Quantity * item.Price * 100) + (long)shippingPrice * 100,
        ////            Currency = "usd",
        ////            PaymentMethodTypes = new List<string> { "card" }
        ////        };

        ////        intent = await paymentIntentService.CreateAsync(options);

        ////        shoppingCart.PaymentIntentId = intent.Id;
        ////        shoppingCart.ClientSecret = intent.ClientSecret;
        ////    }
        ////    else
        ////    {
        ////        var options = new PaymentIntentUpdateOptions
        ////        {
        ////            Amount = (long)shoppingCart.Items.Sum(item => item.Quantity * item.Price * 100) + (long)shippingPrice * 100
        ////        };

        ////        intent = await paymentIntentService.UpdateAsync(shoppingCart.PaymentIntentId, options);
        ////    }

        ////    await _shoppingCartService.SetShoppingCartAsync(shoppingCart);

        ////    return shoppingCart;
        ////}
    }
}
