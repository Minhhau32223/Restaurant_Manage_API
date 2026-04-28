using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RestaurantAPI.src.Modal.Enums;

namespace RestaurantAPI.src.Modal {
    [Table("Tables")]
    public class Table {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("table_code")]
        public string TableCode { get; set; } = string.Empty;

        [Column("seat_count")]
        public int SeatCount { get; set; }

        [Column("status")]
        public TableStatus Status { get; set; } = TableStatus.EMPTY;


  
    }
}
