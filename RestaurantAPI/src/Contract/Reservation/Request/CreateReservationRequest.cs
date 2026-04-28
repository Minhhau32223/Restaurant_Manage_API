namespace RestaurantAPI.src.Contract.Reservation.Request {
    public class CreateReservationRequest {
        public long CustomerId { get; set; }
        public long TableId { get; set; }
        public DateTime ReservationTime { get; set; }
        public int GuestCount { get; set; }
    }
}
