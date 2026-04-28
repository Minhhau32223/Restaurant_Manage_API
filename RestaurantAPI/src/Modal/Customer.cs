using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RestaurantAPI.src.Modal {
    [Table("customers")]
    public class Customer {
        [Key]
        public long Id { get; set; }

        [MaxLength(150)]
        [Column("full_name")]
        public string? FullName { get; set; }

        [MaxLength(20)]
        [Column("phone")]
        public string? Phone { get; set; }

        [MaxLength(150)]
        [Column("email")]
        public string? Email { get; set; }

        [Column("points")]
        public int Points { get; set; }

        [Column("account_id")]
        public long? AccountId { get; set; }

        [ForeignKey("AccountId")]
        public virtual Account? Account { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public virtual ICollection<CustomerVoucher> CustomerVouchers { get; set; } = new List<CustomerVoucher>();
    }
}
