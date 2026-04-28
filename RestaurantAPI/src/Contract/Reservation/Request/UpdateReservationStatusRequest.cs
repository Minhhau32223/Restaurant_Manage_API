using RestaurantAPI.src.Modal.Enums;

namespace RestaurantAPI.src.Contract.Reservation.Request {
    public class UpdateReservationStatusRequest {
        public ReservationStatus Status { get; set; } = ReservationStatus.PENDING;
    }
}
