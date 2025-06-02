namespace API.DTOs
{
    public class CouponDto
    {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal? AmountOff { get; set; }
        public decimal? PercentOff { get; set; }
    }
}
