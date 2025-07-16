using API.DTOs;
using Core.Entities;

namespace API.Extensions
{
    public static class ProductMappingExtensions
    {
        public static ProductDto ToDto(this Product product)
        {
            ArgumentNullException.ThrowIfNull(product);

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                PictureUrl = product.PictureUrl,
                Type = product.Type,
                Brand = product.Brand,
                QuantityInStock = product.QuantityInStock
            };
        }

        public static Product ToEntity(this ProductDto productDto)
        {
            ArgumentNullException.ThrowIfNull(productDto);

            return new Product
            {
                Id = productDto.Id,
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                PictureUrl = productDto.PictureUrl,
                Type = productDto.Type,
                Brand = productDto.Brand,
                QuantityInStock = productDto.QuantityInStock
            };
        }
    }
}
