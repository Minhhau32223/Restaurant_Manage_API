using RestaurantAPI.src.Modal.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.src.Modal {
    [Table("orders")]
    public class Order {
        [Column("id")]
        public long Id { get; set; }

        [Column("table_id")]
        public long TableId { get; set; }

        [Column("account_id")]
        public long AccountId { get; set; }

        [Column("customer_id")]
        public long? CustomerId { get; set; }

        [Column("order_time")]
        public DateTime OrderTime { get; set; }

        public OrderStatus Status { get; set; }

        public List<OrderItem> Items { get; set; } = new();

    }
}
