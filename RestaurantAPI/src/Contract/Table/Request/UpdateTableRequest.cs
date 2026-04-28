using RestaurantAPI.src.Modal.Enums;

namespace RestaurantAPI.src.Contract.Table.Request {
    public class UpdateTableRequest {
        public string TableCode { get; set; } = string.Empty;

        public int SeatCount { get; set; }

        public TableStatus Status { get; set; }
    }
}
