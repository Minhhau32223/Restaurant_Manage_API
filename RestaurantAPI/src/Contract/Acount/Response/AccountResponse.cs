using RestaurantAPI.src.Modal;
using RestaurantAPI.src.Modal.Enums;

namespace RestaurantAPI.src.Contract.Acount.Response;

/// Response trả về thông tin tài khoản (không bao gồm password)
public class AccountResponse
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public Role Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public static AccountResponse FromEntity(Account account) => new()
    {
        Id = account.Id,
        Username = account.Username,
        Role = account.Role,
        IsActive = account.IsActive,
        CreatedAt = account.CreatedAt
    };
}
