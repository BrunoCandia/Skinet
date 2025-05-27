namespace Core.Entities.OrderAggregate
{
    /// <summary>
    /// Owned entity representing the product items ordered in an order.
    /// </summary>
    public class ProductItemOrdered
    {
        public Guid ProductId { get; set; }
        public required string ProductName { get; set; }
        public required string PictureUrl { get; set; }
    }
}
