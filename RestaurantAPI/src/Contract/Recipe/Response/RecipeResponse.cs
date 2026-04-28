using RestaurantAPI.src.Contract.Menu.Response;

namespace RestaurantAPI.src.Contract.Recipe.Response {
    public class RecipeResponse {
        public MenuItemResponse menuItem { get; set; }

        public string ingredientName { get; set; }

        public decimal Quantity { get; set; }

        public static RecipeResponse Map(Modal.Recipe r) {
            return new RecipeResponse {
                menuItem = MenuItemResponse.Map(r.MenuItem),
                ingredientName = r.Ingredients?.Name ?? "Không xác định",
                Quantity = r.Quantity
            };
        }
    }
}
