namespace RestaurantAPI.src.Contract.Inventory.Response
{
    public class InventoryLogResponse
    {
        public long Id { get; set; }
        public long IngredientId { get; set; }
        public string IngredientName { get; set; } = string.Empty;
        public string LogType { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
