namespace RestaurantAPI.src.Contract.Combo.Response {
    public class ComboItemResponse {
        public long MenuItemId { get; set; }
        public string? MenuItemName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
