namespace RestaurantAPI.src.Contract.Menu.Response
{
    public class MenuCategoryResponse {
        public long Id { get; set; }
        public string Name { get; set; }
        public static MenuCategoryResponse Map(RestaurantAPI.src.Modal.MenuCategory m) {
            if (m == null) return null;

            return new MenuCategoryResponse {
                Id = m.Id,
                Name = m.Name
            };
        }
    }
}
