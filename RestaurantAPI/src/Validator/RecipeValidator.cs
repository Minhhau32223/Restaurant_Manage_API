using RestaurantAPI.src.Contract.Recipe.Request;
using RestaurantAPI.src.Exceptions;

namespace RestaurantAPI.src.Validator {
    public class RecipeValidator {
        public static void ValidateCreate(CreateRecipeRequest request) {
            if (request == null)
                throw ApiException.BadRequest("Dữ liệu không được để trống.");

            if (request.MenuItemId <= 0)
                throw ApiException.BadRequest("ID món ăn (MenuItemId) không hợp lệ.");

            if (request.IngredientId <= 0)
                throw ApiException.BadRequest("ID nguyên liệu (IngredientId) không hợp lệ.");

            if (request.Quantity <= 0)
                throw ApiException.BadRequest("Số lượng nguyên liệu trong công thức phải lớn hơn 0.");
        }

        public static void ValidateCreateBulk(CreateBulkRecipeRequest request) {
            if (request == null)
                throw ApiException.BadRequest("Dữ liệu yêu cầu không được để trống.");

            if (request.MenuItemId <= 0)
                throw ApiException.BadRequest("ID món ăn (MenuItemId) không hợp lệ.");

            if (request.Ingredients == null || !request.Ingredients.Any())
                throw ApiException.BadRequest("Danh sách nguyên liệu cho món ăn không được để trống.");

            foreach (var item in request.Ingredients) {
                if (item.IngredientId <= 0)
                    throw ApiException.BadRequest($"ID nguyên liệu {item.IngredientId} không hợp lệ.");

                if (item.Quantity <= 0)
                    throw ApiException.BadRequest($"Số lượng của nguyên liệu ID {item.IngredientId} phải lớn hơn 0.");
            }
        }
    }
}