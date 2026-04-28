namespace RestaurantAPI.src.Contract.Ingredients.Request {
    public class CreateIngredientRequest {
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal StockQuantity { get; set; }
        public decimal MinQuantity { get; set; }
    }
}
