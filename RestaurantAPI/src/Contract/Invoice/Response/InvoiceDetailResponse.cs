using RestaurantAPI.src.Contract.Invoice.Response;

namespace RestaurantAPI.src.Contract.Invoice.Response {
    public class InvoiceDetailResponse {
        public long InvoiceId { get; set; }
        public long OrderId { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Vat { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal Total { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<InvoiceItemResponse> Items { get; set; } = new();
    }
}
