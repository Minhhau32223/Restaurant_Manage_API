namespace RestaurantAPI.src.Contract.Invoice.Request {
    public class CreateInvoiceRequest {
        public long OrderId { get; set; }

        public decimal Discount { get; set; }

        public decimal Vat { get; set; }

        public decimal ServiceFee { get; set; }
    }
}
