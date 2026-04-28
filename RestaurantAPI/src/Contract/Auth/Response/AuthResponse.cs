using RestaurantAPI.src.Contract.Acount.Response;

namespace RestaurantAPI.src.Contract.Auth.Response;

public class AuthResponse {
    public string Token { get; set; } = string.Empty;

    public AccountResponse Account { get; set; } = null!;
}
