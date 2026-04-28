namespace RestaurantAPI.src.Contract.Pricing.Request {
    public class PricingPreviewRequest {
        public List<PricingItem> Items { get; set; } = new();
        public string? VoucherCode { get; set; }
        public long? CustomerId { get; set; }
        public decimal? VatRate { get; set; }
        public decimal? ServiceRate { get; set; }
    }
}
