using RestaurantAPI.src.Modal.Enums;

namespace RestaurantAPI.src.Contract.Table.Response {
    public class TableStatusResponse {
        public long TableId { get; set; }
        public string? TableCode { get; set; }
        public TableStatus Status { get; set; }
        public long? OrderId { get; set; }
        public decimal Total { get; set; }

        public static TableStatusResponse MaptoResponse (RestaurantAPI.src.Modal.Table table, long? orderId, decimal total) {
            return new TableStatusResponse {
                TableId = table.Id,
                TableCode = table.TableCode,
                Status = table.Status,
                OrderId = orderId,
                Total = total
            };
        }
    }
}
