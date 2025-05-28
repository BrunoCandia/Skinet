using API.DTOs;
using Core.Entities.OrderAggregate;

namespace API.Extensions
{
    public static class ShippingAddressMappingExtensions
    {
        public static ShippingAddressDto ToDto(this ShippingAddress shippingAddress)
        {
            ArgumentNullException.ThrowIfNull(shippingAddress);

            return new ShippingAddressDto
            {
                Name = shippingAddress.Name,
                Line1 = shippingAddress.Line1,
                Line2 = shippingAddress.Line2,
                City = shippingAddress.City,
                State = shippingAddress.State,
                Country = shippingAddress.Country,
                PostalCode = shippingAddress.PostalCode,
            };
        }

        public static ShippingAddress ToEntity(this ShippingAddressDto shippingAddressDto)
        {
            ArgumentNullException.ThrowIfNull(shippingAddressDto);

            return new ShippingAddress
            {
                Name = shippingAddressDto.Name,
                Line1 = shippingAddressDto.Line1,
                Line2 = shippingAddressDto.Line2,
                City = shippingAddressDto.City,
                State = shippingAddressDto.State,
                Country = shippingAddressDto.Country,
                PostalCode = shippingAddressDto.PostalCode,
            };
        }
    }
}
