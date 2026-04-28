namespace RestaurantAPI.src.Contract.CustomerVoucher.Request {
    public class UseVoucherRequest {
        public long VoucherId { get; set; }
        public long CustomerId { get; set; }
    }
}
