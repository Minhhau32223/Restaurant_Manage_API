using RestaurantAPI.src.Contract.Combo.Request;
using RestaurantAPI.src.Contract.Combo.Response;
using RestaurantAPI.src.Modal;

namespace RestaurantAPI.src.Services.Interfaces {
    public interface IComboService {
        Task<ComboResponse> GetComboId(long id);
        Task<ComboResponse> CreateCombo(CreateComboRequest request);

        Task<ComboResponse> AddItem(CreateComboItemRequest request);

        Task<ComboResponse> RemoveItem(long comboId, long menuItemId);

        Task<ComboResponse> UpdateItem(long comboId, long menuItemId, int quantity);

        Task<ComboItem> CreateComboItem(CreateComboItemRequest ComboItemRequest);

        Task<List<ComboResponse>> GetAllCombos();
    }
}
