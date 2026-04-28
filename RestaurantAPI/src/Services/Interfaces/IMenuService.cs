using RestaurantAPI.src.Contract.Menu.Request;
using RestaurantAPI.src.Modal;

namespace RestaurantAPI.src.Services.Interfaces {
    public interface IMenuService {
        Task<List<MenuCategory>> GetAllMenuCategory();
        Task<MenuCategory> CreateMenuCategory(CreateMenuCategoryRequest MenuCategoryRequest);
        Task<List<MenuItem>> GetAllMenuItem();
        Task<MenuItem> CreateMenuItem(CreateMenuItemRequest MenuItemRequest);
        Task<MenuItem> UpdateMenuItem(long id,UpdateMenuItemRequest MenuItemRequest);
        Task<bool> UpdateMenuItemStatus(long id,UpdateMenuItemStatusRequest MenuItemStatusRequest);
    }
}
