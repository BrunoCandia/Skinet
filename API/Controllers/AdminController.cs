using API.DTOs;
using API.Extensions;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public AdminController(IUnitOfWork unitOfWork, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }

        [HttpGet("orders")]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrders([FromQuery] OrderSpecParams orderSpecParams, CancellationToken cancellationToken)
        {
            var spec = new OrderSpecification(orderSpecParams);

            return await CreatePagedResult(_unitOfWork.Repository<Order>(), spec, orderSpecParams.PageIndex, orderSpecParams.PageSize, order => order.ToDto(), cancellationToken);
        }

        [HttpGet("orders/{id:Guid}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(Guid id, CancellationToken cancellationToken)
        {
            var spec = new OrderSpecification(id);

            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec, cancellationToken);

            if (order is null)
            {
                return BadRequest("No order with that id");
            }

            return Ok(order.ToDto());
        }

        [HttpPost("orders/refund/{id:Guid}")]
        public async Task<ActionResult<OrderDto>> RefundOrder(Guid id, CancellationToken cancellationToken)
        {
            var spec = new OrderSpecification(id);

            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec, cancellationToken);

            if (order is null)
            {
                return BadRequest("No order with that id");
            }

            if (order.Status == OrderStatus.Pending)
            {
                return BadRequest("Payment not received for this order");
            }

            var result = await _paymentService.RefundPaymentAsync(order.PaymentIntentId, cancellationToken);

            if (result == "succeeded")
            {
                order.Status = OrderStatus.Refunded;

                await _unitOfWork.CompleteAsync(cancellationToken);

                return Ok(order.ToDto());
            }

            return BadRequest("Problem refunding the order");
        }
    }
}
