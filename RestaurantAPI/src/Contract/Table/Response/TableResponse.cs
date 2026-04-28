using RestaurantAPI.src.Modal.Enums;

namespace RestaurantAPI.src.Contract.Table.Response {
    public class TableResponse {
        public long Id { get; set; }
        public string? TableCode { get; set; }
        public int SeatCount { get; set; }
        public TableStatus Status { get; set; }

        public static TableResponse MapToResponse(RestaurantAPI.src.Modal.Table table) {
            return new TableResponse {
                Id = table.Id,
                TableCode = table.TableCode,
                SeatCount = table.SeatCount,
                Status = table.Status
            };
        }
    }
}
