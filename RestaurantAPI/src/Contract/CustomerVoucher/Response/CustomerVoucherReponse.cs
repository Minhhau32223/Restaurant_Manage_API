namespace RestaurantAPI.src.Contract.CustomerVoucher.Response {
    public class CustomerVoucherReponse {
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal Discount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
        public bool IsValid => !IsUsed && ExpiryDate > DateTime.Now;
    }
}
