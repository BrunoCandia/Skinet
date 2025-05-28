using API.DTOs;
using Core.Entities.OrderAggregate;

namespace API.Extensions
{
    public static class PaymentSummaryMappingExtensions
    {
        public static PaymentSummaryDto ToDto(this PaymentSummary paymentSummary)
        {
            ArgumentNullException.ThrowIfNull(paymentSummary);

            return new PaymentSummaryDto
            {
                Brand = paymentSummary.Brand,
                Last4 = paymentSummary.Last4,
                ExpirationMonth = paymentSummary.ExpirationMonth,
                ExpirationYear = paymentSummary.ExpirationYear
            };
        }

        public static PaymentSummary ToEntity(this PaymentSummaryDto paymentSummaryDto)
        {
            ArgumentNullException.ThrowIfNull(paymentSummaryDto);

            return new PaymentSummary
            {
                Brand = paymentSummaryDto.Brand,
                Last4 = paymentSummaryDto.Last4,
                ExpirationMonth = paymentSummaryDto.ExpirationMonth,
                ExpirationYear = paymentSummaryDto.ExpirationYear
            };
        }
    }
}
