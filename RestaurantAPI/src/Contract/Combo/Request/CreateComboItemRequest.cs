namespace RestaurantAPI.src.Contract.Combo.Request {
    public class CreateComboItemRequest {
        public long ComboId { get; set; }
        public long MenuItemId { get; set; }
        public int Quantity { get; set; }
    }
}
