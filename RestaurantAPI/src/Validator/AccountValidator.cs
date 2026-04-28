using RestaurantAPI.src.Contract.Acount.Request;
using RestaurantAPI.src.Exceptions;
using RestaurantAPI.src.Modal.Enums;
using System.Text.RegularExpressions;

namespace RestaurantAPI.src.Validator {
    public class AccountValidator {
        public static void ValidateCreateAccountRequest(CreateAccountRequest req) {
            if (string.IsNullOrWhiteSpace(req.Username))
                throw ApiException.BadRequest("Tên tài khoản không được để trống");

            if (req.Username.Length < 5 || req.Username.Length > 20)
                throw ApiException.BadRequest("Tên tài khoản phải từ 5 đến 20 ký tự");

            if (Regex.IsMatch(req.Username, @"[^a-zA-Z0-9_]"))
                throw ApiException.BadRequest("Tên tài khoản chỉ được chứa chữ cái, số và dấu gạch dưới");

            ValidatePassword(req.Password);

            if (!Enum.IsDefined(typeof(Role), req.Role))
                throw ApiException.BadRequest("Vai trò người dùng không hợp lệ");
        }

        public static void ValidateUpdateAccountRequest(UpdateAccountRequest req) {
            // Nếu có cập nhật Password thì mới validate
            if (req.Password != null)
                ValidatePassword(req.Password);

            if (req.Role.HasValue) {
                if (!Enum.IsDefined(typeof(Role), req.Role.Value))
                    throw ApiException.BadRequest("Vai trò người dùng không hợp lệ");
            }
        }

        public static void ValidateUpdateStatusAccountRequest(UpdateStatusAccountRequest req) {
            if (req.IsActive == null)
                throw ApiException.BadRequest("Trạng thái tài khoản là bắt buộc");
        }

        public static void ValidatePassword(string password) {
            if (string.IsNullOrWhiteSpace(password))
                throw ApiException.BadRequest("Mật khẩu không được để trống");

            if (password.Length < 5)
                throw ApiException.BadRequest("Mật khẩu phải dài ít nhất 5 ký tự");

            if (password.Length > 50)
                throw ApiException.BadRequest("Mật khẩu không được vượt quá 50 ký tự");

            if (!Regex.IsMatch(password, @"^(?=.*[A-Za-z])(?=.*\d).+$"))
                throw ApiException.BadRequest("Mật khẩu phải bao gồm cả chữ và số");
        }
    }
}