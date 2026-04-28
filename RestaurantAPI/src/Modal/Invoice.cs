using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.src.Modal {
    [Table("invoices")]
    public class Invoice {
        [Column("id")]
        public long Id { get; set; }

        [Column("order_id")]
        public long OrderId { get; set; }

        [Column("subtotal")]
        public decimal Subtotal { get; set; }

        [Column("discount")]
        public decimal Discount { get; set; }

        [Column("vat")]
        public decimal Vat { get; set; }

        [Column("service_fee")]
        public decimal ServiceFee { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // navigation property
        public Order? Order { get; set; }
    }
}
