using RestaurantAPI.src.Contract.Combo.Request;
using RestaurantAPI.src.Exceptions;

namespace RestaurantAPI.src.Validator {
    public static class ComboValidator {
        public static void ValidateCreate(CreateComboRequest request) {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw ApiException.BadRequest("Tên Combo không được để trống.");

            if (request.Items == null || !request.Items.Any())
                throw ApiException.BadRequest("Combo phải có ít nhất một món ăn.");

            foreach (var item in request.Items) {
                if (item.MenuItemId <= 0)
                    throw ApiException.BadRequest("MenuItemId không hợp lệ.");
                if (item.Quantity <= 0)
                    throw ApiException.BadRequest("Số lượng món trong combo phải > 0.");
            }
        }

        public static void ValidateAddItem(CreateComboItemRequest request) {
            if (request.ComboId <= 0)
                throw ApiException.BadRequest("ComboId không hợp lệ.");
            if (request.MenuItemId <= 0)
                throw ApiException.BadRequest("MenuItemId không hợp lệ.");
            if (request.Quantity <= 0)
                throw ApiException.BadRequest("Số lượng phải > 0.");
        }
    }
}