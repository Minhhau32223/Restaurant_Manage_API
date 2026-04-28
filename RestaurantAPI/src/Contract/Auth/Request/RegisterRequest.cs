using RestaurantAPI.src.Modal.Enums;

namespace RestaurantAPI.src.Contract.Auth.Request
{
    public class RegisterRequest {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}   