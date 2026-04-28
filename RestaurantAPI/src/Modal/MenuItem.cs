using RestaurantAPI.src.Modal.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.src.Modal
{
    [Table("menu_items")]   
    
    public class MenuItem
    {
        [Key]
        public long Id { get; set; }

        [Column("category_id")]
        public long CategoryId { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("price", TypeName = "decimal(12,2)")]
        public decimal Price { get; set; }

        [Column("image_url")]
        public string? ImageUrl { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("status") ]
        public StatusMenuItem Status { get; set; } = StatusMenuItem.AVAILABLE;

        [ForeignKey(nameof(CategoryId))]
        public MenuCategory? Category { get; set; }

    }
}
