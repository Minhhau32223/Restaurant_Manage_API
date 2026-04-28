namespace RestaurantAPI.src.Contract.Pricing.Response {
    public class PricingResponse {
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Vat { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal Total { get; set; }
    }
}
