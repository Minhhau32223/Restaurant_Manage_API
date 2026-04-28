using RestaurantAPI.src.Contract.Customer.Request;
using RestaurantAPI.src.Exceptions;
using System.Text.RegularExpressions;

namespace RestaurantAPI.src.Validator;

public class CustomerValidator {
    public static void ValidateCreate(CreateCustomerRequest request) {
        if (request == null)
            throw ApiException.BadRequest("Dữ liệu không được để trống.");

        if (string.IsNullOrWhiteSpace(request.FullName))
            throw ApiException.BadRequest("Họ tên không được để trống.");

        // Validate Số điện thoại (Định dạng Việt Nam: 10 số, bắt đầu bằng 0 hoặc +84)
        if (string.IsNullOrWhiteSpace(request.Phone))
            throw ApiException.BadRequest("Số điện thoại không được để trống.");

        string phonePattern = @"^(0|\+84)(\d{9})$";
        if (!Regex.IsMatch(request.Phone, phonePattern))
            throw ApiException.BadRequest("Số điện thoại không đúng định dạng.");

        // Validate Email (nếu có nhập)
        if (!string.IsNullOrWhiteSpace(request.Email)) {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(request.Email, emailPattern))
                throw ApiException.BadRequest("Email không đúng định dạng.");
        }
    }

    public static void ValidateUpdate(UpdateCustomerRequest request) {
        if (request == null)
            throw ApiException.BadRequest("Dữ liệu cập nhật không hợp lệ.");

        if (string.IsNullOrWhiteSpace(request.FullName))
            throw ApiException.BadRequest("Họ tên không được để trống.");

        if (!string.IsNullOrWhiteSpace(request.Email)) {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(request.Email, emailPattern))
                throw ApiException.BadRequest("Email không đúng định dạng.");
        }
    }
}