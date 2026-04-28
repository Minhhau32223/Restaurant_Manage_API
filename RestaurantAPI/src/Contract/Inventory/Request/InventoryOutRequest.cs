namespace RestaurantAPI.src.Contract.Inventory.Request
{
    public class InventoryOutRequest
    {
        public long IngredientId { get; set; }
        public decimal Quantity { get; set; }
        public string? Reason { get; set; }
    }
}
