using RestaurantAPI.src.Modal.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RestaurantAPI.src.Modal
{
    [Table("reservations")]
    public class Reservation {
        [Key]
        public long Id { get; set; }

        [Column("customer_id")]
        public long CustomerId { get; set; }

        [Column("table_id")]
        public long TableId { get; set; }

        [Column("reservation_time")]
        public DateTime ReservationTime { get; set; }

        [Column("guest_count")]
        public int GuestCount { get; set; }

        [Column("status")]
        public ReservationStatus Status { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Customer? Customer { get; set; }
        public Table? Table { get; set; }
    }
}
