namespace RestaurantAPI.src.Contract.Order.Request {
    public class AddOrderItemRequest {
        public long? MenuItemId { get; set; }
        public long? ComboId { get; set; }
        public int Quantity { get; set; }
    }
}
