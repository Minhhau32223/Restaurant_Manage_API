using RestaurantAPI.src.Modal.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.src.Modal {
    [Table("accounts")]
    public class Account {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("username")]
        public string Username { get; set; } = string.Empty;

        [Column("password")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("role")]
        public Role Role { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
