using RestaurantAPI.src.Contract.Recipe.Request;
using RestaurantAPI.src.Modal;

namespace RestaurantAPI.src.Services.Interfaces {
    public interface IRecipeService {
        Task<List<Recipe>> GetRecipeId(int MenuItemId);
        Task<List<Recipe>> CreateBulkRecipe(CreateBulkRecipeRequest request);
        public Task<bool> DeleteRecipe(long menuItemId, long ingredientId);
    }
}
