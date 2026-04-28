using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.src.Modal {
    [Table("ingredients")]
    public class Ingredients {
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string? Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Unit { get; set; }
        
        [Column("stock_quantity")]
        public Decimal StockQuantity { get; set; }

        [Column("min_quantity")]
        public Decimal MinQuantity { get; set; }

        public ICollection<InventoryLogs>? InventoryLogs { get; set; }
    }
}
