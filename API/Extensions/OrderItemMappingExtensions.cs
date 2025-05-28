using API.DTOs;
using Core.Entities.OrderAggregate;

namespace API.Extensions
{
    public static class OrderItemMappingExtensions
    {
        public static OrderItemDto ToDto(this OrderItem orderItem)
        {
            ArgumentNullException.ThrowIfNull(orderItem);

            return new OrderItemDto
            {
                Id = orderItem.Id,
                ItemOrdered = new ProductItemOrderedDto
                {
                    ProductId = orderItem.ItemOrdered.ProductId,
                    ProductName = orderItem.ItemOrdered.ProductName,
                    PictureUrl = orderItem.ItemOrdered.PictureUrl
                },
                Price = orderItem.Price,
                Quantity = orderItem.Quantity
            };
        }

        public static OrderItem ToEntity(this OrderItemDto orderItemDto)
        {
            ArgumentNullException.ThrowIfNull(orderItemDto);

            return new OrderItem
            {
                ItemOrdered = new ProductItemOrdered
                {
                    ProductId = orderItemDto.ItemOrdered.ProductId,
                    ProductName = orderItemDto.ItemOrdered.ProductName,
                    PictureUrl = orderItemDto.ItemOrdered.PictureUrl
                },
                Price = orderItemDto.Price,
                Quantity = orderItemDto.Quantity
            };
        }
    }
}
