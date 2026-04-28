using RestaurantAPI.src.Exceptions;
using System.Text.RegularExpressions;
using AuthRequest = RestaurantAPI.src.Contract.Auth.Request.AuthRequest;

namespace RestaurantAPI.src.Validator {
    public class AuthValidator {
        public static void Validate(AuthRequest request) {
            if (string.IsNullOrWhiteSpace(request.Username))
                throw ApiException.BadRequest("Tên đăng nhập không được để trống");

            if (request.Username.Length < 5 || request.Username.Length > 50)
                throw ApiException.BadRequest("Tên đăng nhập phải từ 5 đến 50 ký tự");

            if (Regex.IsMatch(request.Username, @"\s"))
                throw ApiException.BadRequest("Tên đăng nhập không được chứa khoảng trắng");

            string input = request.Username.ToLower();
            if (input.Contains("drop table") || input.Contains("delete from") || input.Contains("--")) {
                throw ApiException.BadRequest("Dữ liệu đầu vào chứa ký tự không hợp lệ");
            }

            AccountValidator.ValidatePassword(request.Password);
        }
    }
}