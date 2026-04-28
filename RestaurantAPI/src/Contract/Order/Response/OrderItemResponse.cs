namespace RestaurantAPI.src.Contract.Order.Response {
    public class OrderItemResponse {
        public long Id { get; set; }
        public long MenuItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
    }
}
