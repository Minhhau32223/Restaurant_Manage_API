namespace RestaurantAPI.src.Contract.Pricing.Request {
    public class PricingRequest {
        public long OrderId { get; set; }
        public string? VoucherCode { get; set; }
        public long? CustomerId { get; set; }
    }
}
