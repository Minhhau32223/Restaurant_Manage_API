using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RestaurantAPI.src.Modal
{
    [Table("customer_vouchers")]
    public class CustomerVoucher {
        [Key]
        public long Id { get; set; }

        [Column("customer_id")]
        public long CustomerId { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Code { get; set; }
        [Column("discount")]
        public decimal Discount { get; set; }

        [Column("expiry_date")]
        public DateTime ExpiryDate { get; set; }

        [Column("is_used")]
        public bool IsUsed { get; set; } = false;

        [ForeignKey(nameof(CustomerId))]
        public Customer? Customer { get; set; }
    }
}
