namespace RestaurantAPI.src.Contract.Invoice.Response {
    public class InvoiceItemResponse {
        public string Name { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Price * Quantity;
    }
}
