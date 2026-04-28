using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RestaurantAPI.src.Modal {
    [Table("menu_categories")]
    public class MenuCategory {
        [Key]
        public long Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }
    }
}
