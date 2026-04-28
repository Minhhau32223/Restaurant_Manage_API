using RestaurantAPI.src.Contract.CustomerVoucher.Response;

namespace RestaurantAPI.src.Contract.Customer.Response {
    public class CustomerResponse {
        public long Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int Points { get; set; }
        public long? AccountId { get; set; }
        public string? Username { get; set; }   
        public object Reservations { get; internal set; }
        public List<CustomerVoucherReponse> Voucher { get; set; } = new List<CustomerVoucherReponse>();
    }
}
