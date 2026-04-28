namespace RestaurantAPI.src.Contract.Menu.Request {
    public class CreateMenuItemRequest {
        public long CategoryId { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
    }
}
