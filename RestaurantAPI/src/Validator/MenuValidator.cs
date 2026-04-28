using RestaurantAPI.src.Contract.Menu.Request;
using RestaurantAPI.src.Exceptions;

namespace RestaurantAPI.src.Validator {
    public static class MenuValidator {
        public static void ValidateCreateCategory(CreateMenuCategoryRequest request) {
            if (string.IsNullOrWhiteSpace(request.name))
                throw ApiException.BadRequest("Tên danh mục (name) không được để trống.");
        }

        public static void ValidateCreateItem(CreateMenuItemRequest request) {
            if (request.CategoryId <= 0)
                throw ApiException.BadRequest("CategoryId không hợp lệ.");
            if (string.IsNullOrWhiteSpace(request.Name))
                throw ApiException.BadRequest("Tên món ăn (Name) không được để trống.");
            if (request.Price <= 0)
                throw ApiException.BadRequest("Giá món ăn (Price) phải lớn hơn 0.");
        }

        public static void ValidateUpdateItem(UpdateMenuItemRequest request) {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw ApiException.BadRequest("Tên món ăn không được để trống.");
            if (request.Price <= 0)
                throw ApiException.BadRequest("Giá món ăn phải lớn hơn 0.");
        }
    }
}