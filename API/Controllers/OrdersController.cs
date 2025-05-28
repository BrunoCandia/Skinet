using API.DTOs;
using API.Extensions;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class OrdersController : BaseApiController
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IUnitOfWork unitOfWork, IShoppingCartService shoppingCartService)
        {
            _unitOfWork = unitOfWork;
            _shoppingCartService = shoppingCartService;
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createOrderDto)
        {
            var email = User.GetUserEmail();

            var shoppingCart = await _shoppingCartService.GetShoppingCartAsync(createOrderDto.ShoppingCartId);

            if (shoppingCart is null || shoppingCart.Items.Count == 0)
            {
                return BadRequest("Shopping cart is empty or does not exist.");
            }

            if (shoppingCart.PaymentIntentId is null)
            {
                return BadRequest("Payment intent is not created. Please complete the payment first.");
            }

            var orderItems = new List<OrderItem>();

            foreach (var item in shoppingCart.Items)
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);

                if (productItem is null)
                {
                    return BadRequest($"Product with ID {item.ProductId} does not exist.");
                }

                var itemOrdered = new ProductItemOrdered
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    PictureUrl = item.PictureUrl
                };

                var orderItem = new OrderItem
                {
                    ItemOrdered = itemOrdered,
                    Price = productItem.Price,
                    Quantity = item.Quantity
                };

                orderItems.Add(orderItem);
            }

            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(createOrderDto.DeliveryMethodId);

            if (deliveryMethod is null)
            {
                return BadRequest("Delivery method does not exist.");
            }

            var order = new Order
            {
                OrderItems = orderItems,
                DeliveryMethod = deliveryMethod,
                ShippingAddress = createOrderDto.ShippingAddress.ToEntity(),
                Subtotal = orderItems.Sum(item => item.Price * item.Quantity),
                PaymentSummary = createOrderDto.PaymentSummary.ToEntity(),
                PaymentIntentId = shoppingCart.PaymentIntentId,
                BuyerEmail = email
            };

            await _unitOfWork.Repository<Order>().AddAsync(order);

            var test = order.ToDto();

            if (await _unitOfWork.CompleteAsync())
            {
                return Ok(order.ToDto());
            }

            return BadRequest("Problem creating order. Please try again.");
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrderForUser()
        {
            var spec = new OrderSpecification(User.GetUserEmail());

            var orders = await _unitOfWork.Repository<Order>().GetEntitiesWithSpecAsync(spec);

            var ordersDto = orders.Select(x => x.ToDto()).ToList();

            return Ok(ordersDto);
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(Guid id)
        {
            var spec = new OrderSpecification(User.GetUserEmail(), id);

            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);

            if (order is null)
            {
                return NotFound();
            }

            var orderDto = order.ToDto();

            return Ok(orderDto);
        }
    }
}
