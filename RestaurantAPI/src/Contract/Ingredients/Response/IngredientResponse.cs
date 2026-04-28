namespace RestaurantAPI.src.Contract.Ingredients.Response {
    public class IngredientResponse {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal StockQuantity { get; set; }
        public decimal MinQuantity { get; set; }
        public bool IsLowStock => StockQuantity <= MinQuantity;
    }
}
