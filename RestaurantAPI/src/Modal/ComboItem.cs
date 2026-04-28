using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.src.Modal {
    [Table("combo_items")]
    public class ComboItem {
        [Column("combo_id")]
        public long ComboId { get; set; }

        [Column("menu_item_id")]
        public long MenuItemId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [ForeignKey(nameof(ComboId))]
        public Combo? Combo { get; set; }

        [ForeignKey(nameof(MenuItemId))]
        public MenuItem? MenuItem { get; set; }
    }
}