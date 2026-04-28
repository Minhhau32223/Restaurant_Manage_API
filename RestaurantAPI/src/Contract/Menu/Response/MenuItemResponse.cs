using RestaurantAPI.src.Modal.Enums;

namespace RestaurantAPI.src.Contract.Menu.Response {
    public class MenuItemResponse {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public MenuCategoryResponse Category { get; set; }
        public StatusMenuItem Status { get; set; }
        public static MenuItemResponse Map(RestaurantAPI.src.Modal.MenuItem m) {
            if (m == null) return null;

            return new MenuItemResponse {
                Id = m.Id,
                Name = m.Name,
                Price = m.Price,
                ImageUrl = m.ImageUrl,
                Description = m.Description,
                Status = m.Status,
                Category = MenuCategoryResponse.Map(m.Category)
            };
        
        }
    }
}
