using RestaurantAPI.src.Contract.Ingredients.Request;
using RestaurantAPI.src.Contract.Inventory.Request;
using RestaurantAPI.src.Exceptions;

namespace RestaurantAPI.src.Validator {
    public class IngredientValidator {
        public static void ValidateCreate(CreateIngredientRequest request) {
            if (request == null)
                throw ApiException.BadRequest("Dữ liệu không được để trống.");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw ApiException.BadRequest("Tên nguyên liệu không được để trống.");

            if (string.IsNullOrWhiteSpace(request.Unit))
                throw ApiException.BadRequest("Đơn vị tính không được để trống.");

            if (request.StockQuantity < 0)
                throw ApiException.BadRequest("Số lượng tồn kho ban đầu không được nhỏ hơn 0.");

            if (request.MinQuantity < 0)
                throw ApiException.BadRequest("Số lượng tối thiểu không được nhỏ hơn 0.");
        }

        public static void ValidateUpdate(UpdateIngredientRequest request) {
            if (request == null)
                throw ApiException.BadRequest("Dữ liệu cập nhật không hợp lệ.");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw ApiException.BadRequest("Tên nguyên liệu không được để trống.");

            if (string.IsNullOrWhiteSpace(request.Unit))
                throw ApiException.BadRequest("Đơn vị tính không được để trống.");
        }

        public static void ValidateInventoryAction(decimal quantity) {
            if (quantity <= 0)
                throw ApiException.BadRequest("Số lượng nhập/xuất phải lớn hơn 0.");
        }
    }
}