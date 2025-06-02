using API.Extensions;
using API.SignalR;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;

namespace API.Controllers
{
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly IGenericRepository<DeliveryMethod> _deliveryMethodRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentsController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<NotificationHub> _hubContext;

        private readonly string _whSecret = string.Empty;

        public PaymentsController(IPaymentService paymentService, IGenericRepository<DeliveryMethod> deliveryMethodRepository, IUnitOfWork unitOfWork, ILogger<PaymentsController> logger, IConfiguration configuration, IHubContext<NotificationHub> hubContext)
        {
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            _deliveryMethodRepository = deliveryMethodRepository ?? throw new ArgumentNullException(nameof(deliveryMethodRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));

            var whSecret = _configuration["StripeSettings:WhSecret"];

            if (string.IsNullOrWhiteSpace(whSecret))
            {
                throw new InvalidOperationException("StripeSettings:WhSecret is not configured.");
            }

            _whSecret = whSecret;
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
            var deliveryMethods = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();

            if (deliveryMethods is null || !deliveryMethods.Any())
            {
                return NotFound("No delivery methods found");
            }

            return Ok(deliveryMethods);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            _logger.LogInformation("StripeWebhook started");

            var json = await new StreamReader(Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = ConstructStripeEvent(json);

                if (stripeEvent.Data.Object is not PaymentIntent intent)
                {
                    return BadRequest("Invalid event data");
                }

                await HandlePaymentIntentSucceededAsync(intent);

                _logger.LogInformation("HandlePaymentIntentSucceededAsync executed succesfully");

                return Ok();
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe webhook error occurred");
                return StatusCode(StatusCodes.Status500InternalServerError, "Stripe webhook error occurred");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
            }
            finally
            {
                _logger.LogInformation("StripeWebhook finished");
            }
        }

        private Event ConstructStripeEvent(string json)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _whSecret);

                return stripeEvent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to construct stripe event");
                throw new StripeException("Invalid signature");
            }
        }

        private async Task HandlePaymentIntentSucceededAsync(PaymentIntent intent)
        {
            if (intent.Status == "succeeded")
            {
                var spec = new OrderSpecification(intent.Id, true);

                var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);

                if (order is null)
                {
                    throw new Exception("Order not found");
                }

                var orderTotalInCents = (long)Math.Round(order.GetTotal() * 100, MidpointRounding.AwayFromZero);

                if (orderTotalInCents != intent.Amount)
                {
                    order.Status = OrderStatus.PaymentMismatch;
                }
                else
                {
                    order.Status = OrderStatus.PaymentReceived;
                }

                await _unitOfWork.CompleteAsync();

                // SignalR to notify the user in the client side
                var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);
                Console.WriteLine("HandlePaymentIntentSucceededAsync executed: " + connectionId);

                if (!string.IsNullOrWhiteSpace(connectionId))
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("OrderCompleteNotification", order.ToDto());
                    Console.WriteLine("OrderCompleteNotification sent to the client" + order.ToDto());
                }
            }
        }

        #region Without Unit of Work        

        ////[Authorize]
        ////[HttpPost("{shoppingCartId}")]
        ////public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent([FromRoute] string shoppingCartId)
        ////{
        ////    var shoppingCart = await _paymentService.CreateOrUpdatePaymentIntentAsync(shoppingCartId);

        ////    if (shoppingCart is null)
        ////    {
        ////        return BadRequest("Problem creating or updating the payment intent");
        ////    }

        ////    return Ok(shoppingCart);
        ////}

        ////[HttpGet("delivery-methods")]
        ////public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        ////{
        ////    var deliveryMethods = await _deliveryMethodRepository.GetAllAsync();

        ////    if (deliveryMethods is null || !deliveryMethods.Any())
        ////    {
        ////        return NotFound("No delivery methods found");
        ////    }

        ////    return Ok(deliveryMethods);
        ////}

        #endregion
    }
}
