using RestaurantAPI.src.Modal.Enums;

namespace RestaurantAPI.src.Contract.Payment.Request {
    public class CreatePaymentRequest {
        public long OrderId { get; set; }
        public PaymentMethod Method { get; set; }
        public string? VoucherCode { get; set; }
        public long? CustomerId { get; set; }
    }
}
