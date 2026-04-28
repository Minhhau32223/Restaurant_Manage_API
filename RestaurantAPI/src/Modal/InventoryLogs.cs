using RestaurantAPI.src.Modal.Enums;
using System;
using System.ComponentModel.DataAnnotations;    
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.src.Modal {
    [Table("inventory_logs")]
    public class InventoryLogs {
        [Key]
        public long Id { get; set; }
        [Column("ingredient_id")]
        public long IngredientId { get; set; }

        [Required]
        [Column("log_type")]
        public InventoryLogType LogType { get; set; }
        [Column("expiry_date")]
        public DateTime? ExpiryDate { get; set; }
        public Decimal Quantity { get; set; }
        [Column("created_at")]
        public DateTime? CreateAt { get; set; }
        [ForeignKey(nameof(IngredientId))]
        public Ingredients? Ingredients { get; set; }
    }
}
