using RestaurantAPI.src.Modal.Enums;

namespace RestaurantAPI.src.Contract.Order.Response {
    public class OrderResponse {
        public long Id { get; set; }
        public long TableId { get; set; }
        public OrderStatus Status { get; set; }

        public static OrderResponse Map(RestaurantAPI.src.Modal.Order o) => new() {
            Id = o.Id,
            TableId = o.TableId,
            Status = o.Status
        };
    }
}
