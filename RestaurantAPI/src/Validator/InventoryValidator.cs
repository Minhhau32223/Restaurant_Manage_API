using RestaurantAPI.src.Contract.Inventory.Request;
using RestaurantAPI.src.Exceptions;

namespace RestaurantAPI.src.Validator {
    public class InventoryValidator {
        public static void ValidateIn(InventoryInRequest request) {
            if (request == null)
                throw ApiException.BadRequest("Dữ liệu nhập kho không được để trống.");

            if (request.IngredientId <= 0)
                throw ApiException.BadRequest("Mã nguyên liệu (IngredientId) không hợp lệ.");

            if (request.Quantity <= 0)
                throw ApiException.BadRequest("Số lượng nhập kho phải lớn hơn 0.");

            if (request.ExpiryDate.HasValue && request.ExpiryDate.Value <= DateTime.UtcNow)
                throw ApiException.BadRequest("Ngày hết hạn phải lớn hơn thời gian hiện tại.");
        }

        public static void ValidateOut(InventoryOutRequest request) {
            if (request == null)
                throw ApiException.BadRequest("Dữ liệu xuất kho không được để trống.");

            if (request.IngredientId <= 0)
                throw ApiException.BadRequest("Mã nguyên liệu (IngredientId) không hợp lệ.");

            if (request.Quantity <= 0)
                throw ApiException.BadRequest("Số lượng xuất kho phải lớn hơn 0.");
        }
    }
}