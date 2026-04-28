using RestaurantAPI.src.Contract.Order.Request;
using RestaurantAPI.src.Exceptions;

namespace RestaurantAPI.src.Validator;

public static class OrderValidator {
    public static void ValidateCreate(CreateOrderRequest request) {
        if (request.TableId <= 0) throw ApiException.BadRequest("TableId không hợp lệ.");
        if (request.AccountId <= 0) throw ApiException.BadRequest("AccountId không hợp lệ.");
    }

    public static void ValidateAddItem(AddOrderItemRequest request) {
        if (request.MenuItemId == null && request.ComboId == null)
            throw ApiException.BadRequest("Phải chọn món lẻ hoặc combo.");
        if (request.Quantity <= 0) throw ApiException.BadRequest("Số lượng phải lớn hơn 0.");
    }
}