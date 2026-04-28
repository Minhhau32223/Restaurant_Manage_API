namespace RestaurantAPI.src.Contract.Recipe.Request {
    public class IngredientItemRequest {
        public long IngredientId { get; set; }
        public decimal Quantity { get; set; }
    }

    public class CreateBulkRecipeRequest {
        public long MenuItemId { get; set; }
        public List<IngredientItemRequest> Ingredients { get; set; } = new();
    }
}