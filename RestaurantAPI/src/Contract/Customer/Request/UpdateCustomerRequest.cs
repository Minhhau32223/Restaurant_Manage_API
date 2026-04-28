namespace RestaurantAPI.src.Contract.Customer.Request {
    public class UpdateCustomerRequest {
        public long Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
    }
}
