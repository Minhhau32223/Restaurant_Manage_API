namespace RestaurantAPI.src.Contract.Inventory.Request {
    public class InventoryInRequest {
        public long IngredientId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
