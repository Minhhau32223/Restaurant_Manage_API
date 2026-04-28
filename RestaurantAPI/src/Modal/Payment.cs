using RestaurantAPI.src.Modal.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.src.Modal {
    [Table("payments")]
    public class Payment {
        [Column("id")]
        public long Id { get; set; }

        [Column("invoice_id")]
        public long InvoiceId { get; set; }

        [Column("method")]
        public PaymentMethod Method { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("status")]
        public PaymentStatus Status { get; set; }

        [Column("payment_time")]
        public DateTime PaymentTime { get; set; }
    }
}
