using RestaurantAPI.src.Contract.Inventory.Request;
using RestaurantAPI.src.Contract.Inventory.Response;
namespace RestaurantAPI.src.Services.Interfaces
{
    public interface IInventoryService {
        Task<ApiResponse<List<InventoryLogResponse>>> InventoryInAsync(InventoryInRequest request);
        Task<ApiResponse<List<InventoryLogResponse>>> InventoryOutAsync(InventoryOutRequest request);
        Task<ApiResponse<List<InventoryLogResponse>>> GetInventoryLogsByIngredientAsync(long ingredientId);
        Task<bool> DeductStockFromOrderAsync(long orderId);
    }
}
