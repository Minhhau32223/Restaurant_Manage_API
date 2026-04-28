using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.src.Modal {
    [Table("combos")]
    public class Combo {
        [Key]
        public long Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("price", TypeName = "decimal(12,2)")] 
        public decimal Price { get; set; }

        public List<ComboItem> Items { get; set; } = new();
    }
}
