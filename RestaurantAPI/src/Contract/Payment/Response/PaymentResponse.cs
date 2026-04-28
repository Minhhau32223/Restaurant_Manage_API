namespace RestaurantAPI.src.Contract.Payment.Response {
    public class PaymentResponse {
        public long InvoiceId { get; set; }
        public decimal Total { get; set; }
        public string Method { get; set; } = default!;
        public DateTime PaymentTime { get; set; }
    }
}
