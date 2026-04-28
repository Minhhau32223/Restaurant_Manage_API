using RestaurantAPI.src.Contract.Reservation.Request;
using RestaurantAPI.src.Contract.Reservation.Response;

namespace RestaurantAPI.src.Services.Interfaces {
    public interface IReservationService {
        Task<ApiResponse<List<ReservationResponse>>> GetAllReservationsAsync();
        Task<ApiResponse<ReservationResponse>> GetReservationByIdAsync(long id);
        Task<ApiResponse<List<ReservationResponse>>> GetReservationsByCustomerIdAsync(long customerId);
        Task<ApiResponse<object>> CreateReservationAsync(CreateReservationRequest req, long currentUserId, string userRole);
    }
}
