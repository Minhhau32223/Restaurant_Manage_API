using RestaurantAPI.src.Contract.Recipe.Request;
using RestaurantAPI.src.Contract.Recipe.Response;
using RestaurantAPI.src.Modal;
using RestaurantAPI.src.Services.Interfaces;
using RestaurantAPI.src.Validator;

namespace RestaurantAPI.src.Route {
    public static class RecipesRoute {
        public static void MapRecipiesRoute(this IEndpointRouteBuilder app) {
            var group = app.MapGroup("/api/recipes")
                           .WithTags("Recipe")
                           .RequireAuthorization("staff");

            // GET: Lấy công thức
            group.MapGet("/{menuItemId:int}", async (IRecipeService recipeService, int menuItemId) => {
                var recipes = await recipeService.GetRecipeId(menuItemId);
                return Results.Ok(ApiResponse<List<RecipeResponse>>.SuccessResponse(
                    recipes.Select(RecipeResponse.Map).ToList()
                ));
            });

            // POST: Thêm nhiều thành phần
            group.MapPost("/bulk", async (IRecipeService recipeService, CreateBulkRecipeRequest request) => {
                RecipeValidator.ValidateCreateBulk(request);
                var result = await recipeService.CreateBulkRecipe(request);
                return Results.Ok(ApiResponse<List<RecipeResponse>>.SuccessResponse(result.Select(RecipeResponse.Map).ToList()));
            });

            group.MapDelete("/{menuItemId:long}/{ingredientId:long}", async (IRecipeService service, long menuItemId, long ingredientId) => {
                await service.DeleteRecipe(menuItemId, ingredientId);
                return Results.Ok(ApiResponse<object>.SuccessResponse(null, "Đã xóa nguyên liệu khỏi công thức"));
            });
        }
    }
}
