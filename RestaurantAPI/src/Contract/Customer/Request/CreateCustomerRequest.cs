namespace RestaurantAPI.src.Contract.Customer.Request {
    public class CreateCustomerRequest {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
    }
}
