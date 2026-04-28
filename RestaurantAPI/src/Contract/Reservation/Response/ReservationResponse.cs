using RestaurantAPI.src.Modal.Enums;

namespace RestaurantAPI.src.Contract.Reservation.Response {
    public class ReservationResponse {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public long TableId { get; set; }
        public string TableName { get; set; } = string.Empty;
        public DateTime ReservationTime { get; set; }
        public int GuestCount { get; set; }
        public ReservationStatus Status { get; set; }
    }
}
