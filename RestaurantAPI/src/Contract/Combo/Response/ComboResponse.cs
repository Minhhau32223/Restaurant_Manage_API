namespace RestaurantAPI.src.Contract.Combo.Response {
    public class ComboResponse {
        public long Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }

        public List<ComboItemResponse> Items { get; set; } = new();

        public static ComboResponse Map(RestaurantAPI.src.Modal.Combo combo) {
            if (combo == null) return null;
            return new ComboResponse {
                Id = combo.Id,
                Name = combo.Name,
                Price = combo.Price,
                Items = combo.Items?.Select(i => new ComboItemResponse {
                    MenuItemId = i.MenuItemId,
                    MenuItemName = i.MenuItem?.Name,
                    Price = i.MenuItem?.Price ?? 0,
                    Quantity = i.Quantity
                }).ToList() ?? new List<ComboItemResponse>()
            };
        }
    }
}
