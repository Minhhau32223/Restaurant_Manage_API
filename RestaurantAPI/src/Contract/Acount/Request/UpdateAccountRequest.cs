using RestaurantAPI.src.Modal.Enums;
using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.src.Contract.Acount.Request;

public class UpdateAccountRequest {
    public string? Password { get; set; }

    public Role? Role { get; set; }

    public bool? IsActive { get; set; }
}
