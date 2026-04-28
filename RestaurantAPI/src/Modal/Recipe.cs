using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.src.Modal {
    [Table("recipes")]
    public class Recipe {
        [Column("menu_item_id")]
        public long MenuItemId { get; set; }

        [Column("ingredient_id")]
        public long IngredientId { get; set; }

        [Column("quantity")]
        public decimal Quantity { get; set; }

        [ForeignKey(nameof(MenuItemId))]
        public MenuItem? MenuItem { get; set; }

        [ForeignKey(nameof(IngredientId))]
        public Ingredients? Ingredients { get; set; }
    }
}