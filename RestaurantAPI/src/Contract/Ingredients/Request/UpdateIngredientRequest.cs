namespace RestaurantAPI.src.Contract.Ingredients.Request {
    public class UpdateIngredientRequest {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal MinQuantity { get; set; }

    }
}
