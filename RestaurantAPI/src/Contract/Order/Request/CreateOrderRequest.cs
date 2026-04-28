namespace RestaurantAPI.src.Contract.Order.Request {
    public class CreateOrderRequest {
        public long TableId { get; set; }
        public long AccountId { get; set; }
        public long? CustomerId { get; set; }
    }
}
