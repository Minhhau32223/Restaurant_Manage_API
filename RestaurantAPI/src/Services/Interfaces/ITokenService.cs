using RestaurantAPI.src.Modal;

namespace RestaurantAPI.src.Services.Interfaces;

/// Service tạo và quản lý JWT token cho xác thực API
public interface ITokenService
{
    /// Tạo JWT token cho tài khoản đã xác thực
    /// <param name="user">Đối tượng Account đã xác thực</param>
    /// <returns>Chuỗi JWT token</returns>
    string CreateToken(Account user);
}
