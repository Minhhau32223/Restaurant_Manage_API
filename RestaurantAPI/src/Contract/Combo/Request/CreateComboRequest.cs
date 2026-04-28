namespace RestaurantAPI.src.Contract.Combo.Request {
    public class CreateComboRequest {
        public string Name { get; set; } = string.Empty;
        public List<ComboItemRequest> Items { get; set; } = new();
    }
}
