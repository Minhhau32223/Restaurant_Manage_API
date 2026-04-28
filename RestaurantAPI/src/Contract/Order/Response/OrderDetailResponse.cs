using RestaurantAPI.src.Modal.Enums;

namespace RestaurantAPI.src.Contract.Order.Response {
    public class OrderDetailResponse {
        public long Id { get; set; }
        public long TableId { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItemResponse> Items { get; set; } = new();
        public decimal Total { get; set; }
    }
}
