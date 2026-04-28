using RestaurantAPI.src.Contract.Ingredients.Request;
using RestaurantAPI.src.Contract.Ingredients.Response;


namespace RestaurantAPI.src.Services.Interfaces

{
    public interface IIngredientServices
    {
        Task<ApiResponse<List<IngredientResponse>>> GetAllIngredientAsync();
        Task<ApiResponse<IngredientResponse>> GetIngredientByIdAsync(long id);
        Task<ApiResponse<IngredientResponse>> CreateIngredientAsync(CreateIngredientRequest request);
        Task<ApiResponse<IngredientResponse>> UpdateIngredientAsync(long id, UpdateIngredientRequest request);
    }
}
