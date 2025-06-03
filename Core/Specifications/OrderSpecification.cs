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

        public OrderSpecification(string paymentIntentId, bool isPaymentIntent) : base(x => x.PaymentIntentId == paymentIntentId)
        {
            AddIncludes("OrderItems");
            AddIncludes("DeliveryMethod");
        }

        public OrderSpecification(OrderSpecParams orderSpecParams) : base(x => string.IsNullOrWhiteSpace(orderSpecParams.Status) || x.Status == ParseStatus(orderSpecParams.Status))
        {
            AddIncludes("OrderItems");
            AddIncludes("DeliveryMethod");
            ApplyPaging(orderSpecParams.PageSize * (orderSpecParams.PageIndex - 1), orderSpecParams.PageSize);
            AddOrderByDescending(x => x.OrderDate);
        }

        public OrderSpecification(Guid id) : base(x => x.Id == id)
        {
            AddIncludes("OrderItems");
            AddIncludes("DeliveryMethod");
        }

        private static OrderStatus? ParseStatus(string status)
        {
            if (Enum.TryParse<OrderStatus>(status, true, out var result))
            {
                return result;
            }

            return null;
        }
    }
}
