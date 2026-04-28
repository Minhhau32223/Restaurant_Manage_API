namespace RestaurantAPI.src.Contract.CustomerVoucher.Request {
    public class CreateCustomerVoucherRequest {
        public long CustomerId { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal Discount { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
