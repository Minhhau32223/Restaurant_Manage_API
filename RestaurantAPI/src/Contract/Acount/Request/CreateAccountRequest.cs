using RestaurantAPI.src.Modal.Enums;
using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.src.Contract.Acount.Request;

public class CreateAccountRequest {
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public Role Role { get; set; } = Role.CUSTOMER;

    public bool IsActive { get; set; } = true;
}
