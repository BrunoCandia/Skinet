﻿namespace API.DTOs
{
    public class ProductItemOrderedDto
    {
        public Guid ProductId { get; set; }
        public required string ProductName { get; set; }
        public required string PictureUrl { get; set; }
    }
}
