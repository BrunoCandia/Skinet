using Core.Entities.OrderAggregate;

namespace Core.Specifications
{
    public class OrderSpecification : BaseSpecification<Order>
    {
        public OrderSpecification(string email) : base(x => x.BuyerEmail == email)
        {
            AddIncludes(x => x.OrderItems);
            AddIncludes(x => x.DeliveryMethod);
            AddOrderByDescending(x => x.OrderDate);
        }

        public OrderSpecification(string email, Guid id) : base(x => x.BuyerEmail == email && x.Id == id)
        {
            ////AddIncludes("OrderItems.ProductItem");  // ThenInclude
            AddIncludes("OrderItems");
            AddIncludes("DeliveryMethod");
        }
    }
}
