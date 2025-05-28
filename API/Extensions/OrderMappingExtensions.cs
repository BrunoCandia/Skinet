using API.DTOs;
using Core.Entities.OrderAggregate;

namespace API.Extensions
{
    public static class OrderMappingExtensions
    {
        public static OrderDto ToDto(this Order order)
        {
            ArgumentNullException.ThrowIfNull(order);

            return new OrderDto
            {
                Id = order.Id,
                BuyerEmail = order.BuyerEmail,
                OrderDate = order.OrderDate,
                DeliveryMethodId = order.DeliveryMethodId,
                DeliveryMethod = new DeliveryMethodDto
                {
                    Id = order.DeliveryMethod.Id,
                    DeliveryTime = order.DeliveryMethod.DeliveryTime,
                    Description = order.DeliveryMethod.Description,
                    ShortName = order.DeliveryMethod.ShortName,
                    Price = order.DeliveryMethod.Price
                },
                PaymentIntentId = order.PaymentIntentId,
                ShippingAddress = order.ShippingAddress.ToDto(),
                PaymentSummary = order.PaymentSummary.ToDto(),
                Subtotal = order.Subtotal,
                Total = order.GetTotal(),
                Status = (OrderStatusDto)order.Status,
                StatusString = order.Status.ToString(),
                OrderItems = order.OrderItems.Select(orderItem => orderItem.ToDto()).ToList()
            };
        }

        public static Order ToEntity(this OrderDto orderDto)
        {
            ArgumentNullException.ThrowIfNull(orderDto);

            return new Order
            {
                BuyerEmail = orderDto.BuyerEmail,
                OrderDate = orderDto.OrderDate,
                DeliveryMethodId = orderDto.DeliveryMethodId,
                DeliveryMethod = new Core.Entities.DeliveryMethod
                {
                    DeliveryTime = orderDto.DeliveryMethod.DeliveryTime,
                    Description = orderDto.DeliveryMethod.Description,
                    ShortName = orderDto.DeliveryMethod.ShortName,
                    Price = orderDto.DeliveryMethod.Price
                },
                PaymentIntentId = orderDto.PaymentIntentId,
                ShippingAddress = orderDto.ShippingAddress.ToEntity(),
                PaymentSummary = orderDto.PaymentSummary.ToEntity(),
                Subtotal = orderDto.Subtotal,
                Status = (OrderStatus)orderDto.Status,
                OrderItems = orderDto.OrderItems.Select(orderItem => orderItem.ToEntity()).ToList()
            };
        }
    }
}
