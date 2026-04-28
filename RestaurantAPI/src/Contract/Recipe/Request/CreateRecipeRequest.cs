namespace RestaurantAPI.src.Contract.Recipe.Request {
    public class CreateRecipeRequest {
        public int MenuItemId { get; set; }
        public int IngredientId { get; set; }
        public decimal Quantity { get; set; }
    }
}
