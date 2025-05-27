using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly IGenericRepository<DeliveryMethod> _deliveryMethodRepository;

        public PaymentsController(IPaymentService paymentService, IGenericRepository<DeliveryMethod> deliveryMethodRepository)
        {
            _paymentService = paymentService;
            _deliveryMethodRepository = deliveryMethodRepository;
        }

        [Authorize]
        [HttpPost("{shoppingCartId}")]
        public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent([FromRoute] string shoppingCartId)
        {
            var shoppingCart = await _paymentService.CreateOrUpdatePaymentIntentAsync(shoppingCartId);

            if (shoppingCart is null)
            {
                return BadRequest("Problem creating or updating the payment intent");
            }

            return Ok(shoppingCart);
        }

        [HttpGet("delivery-methods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            var deliveryMethods = await _deliveryMethodRepository.GetAllAsync();

            if (deliveryMethods is null || !deliveryMethods.Any())
            {
                return NotFound("No delivery methods found");
            }

            return Ok(deliveryMethods);
        }
    }
}
