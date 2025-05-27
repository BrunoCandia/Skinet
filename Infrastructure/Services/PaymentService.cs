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

        public PaymentService(
            IConfiguration configuration,
            IShoppingCartService shoppingCartService,
            IGenericRepository<Entities.Product> productRepository,
            IGenericRepository<Entities.DeliveryMethod> deliveMethodRepository)
        {
            _configuration = configuration;
            _shoppingCartService = shoppingCartService;
            _productRepository = productRepository;
            _deliveMethodRepository = deliveMethodRepository;
        }

        public async Task<Entities.ShoppingCart?> CreateOrUpdatePaymentIntentAsync(string shoppingCartId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

            var shoppingCart = await _shoppingCartService.GetShoppingCartAsync(shoppingCartId);

            if (shoppingCart is null)
            {
                return null;
            }

            var shippingPrice = 0m;

            if (shoppingCart.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _deliveMethodRepository.GetByIdAsync(shoppingCart.DeliveryMethodId.Value);

                if (deliveryMethod is null)
                {
                    return null;
                }

                shippingPrice = deliveryMethod.Price;
            }

            foreach (var item in shoppingCart.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);

                if (product is null)
                {
                    return null;
                }

                if (item.Price != product.Price)
                {
                    item.Price = product.Price;
                }
            }

            var paymentIntentService = new PaymentIntentService();
            PaymentIntent? intent;

            if (string.IsNullOrWhiteSpace(shoppingCart.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)shoppingCart.Items.Sum(item => item.Quantity * item.Price * 100) + (long)shippingPrice * 100,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                intent = await paymentIntentService.CreateAsync(options);

                shoppingCart.PaymentIntentId = intent.Id;
                shoppingCart.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)shoppingCart.Items.Sum(item => item.Quantity * item.Price * 100) + (long)shippingPrice * 100
                };

                intent = await paymentIntentService.UpdateAsync(shoppingCart.PaymentIntentId, options);
            }

            await _shoppingCartService.SetShoppingCartAsync(shoppingCart);

            return shoppingCart;
        }
    }
}
