using Microsoft.EntityFrameworkCore;
using RestaurantAPI.src.Contract.Reservation.Request;
using RestaurantAPI.src.Contract.Reservation.Response;
using RestaurantAPI.src.Data;
using RestaurantAPI.src.Exceptions;
using RestaurantAPI.src.Modal;
using RestaurantAPI.src.Services.Interfaces;

namespace RestaurantAPI.src.Services
{
    public class ReservationService : IReservationService
    {
        private readonly AppDbContext _context;
        public ReservationService(AppDbContext context) {
            _context = context;
        }
        public Task<ApiResponse<List<ReservationResponse>>> GetAllReservationsAsync() {
            return GetAllReservationsAsyncImpl();
        }

        private async Task<ApiResponse<List<ReservationResponse>>> GetAllReservationsAsyncImpl() {
            var reservations = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Table)
                .Select(r => new ReservationResponse {
                    Id = r.Id,
                    CustomerId = r.CustomerId,
                    CustomerName = r.Customer != null ? r.Customer.FullName ?? string.Empty : string.Empty,
                    TableId = r.TableId,
                    TableName = r.Table != null ? r.Table.TableCode : string.Empty,
                    ReservationTime = r.ReservationTime,
                    GuestCount = r.GuestCount,
                    Status = r.Status
                }).ToListAsync();

            return ApiResponse<List<ReservationResponse>>.SuccessResponse(reservations);
        }

        public async Task<ApiResponse<ReservationResponse>> GetReservationByIdAsync(long id) {
            var reservation = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Table)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                throw ApiException.NotFound("Đặt chỗ không tồn tại.");

            var response = new ReservationResponse {
                Id = reservation.Id,
                CustomerId = reservation.CustomerId,
                CustomerName = reservation.Customer != null ? reservation.Customer.FullName ?? string.Empty : string.Empty,
                TableId = reservation.TableId,
                TableName = reservation.Table != null ? reservation.Table.TableCode : string.Empty,
                ReservationTime = reservation.ReservationTime,
                GuestCount = reservation.GuestCount,
                Status = reservation.Status
            };

            return ApiResponse<ReservationResponse>.SuccessResponse(response);
        }

        public async Task<ApiResponse<List<ReservationResponse>>> GetReservationsByCustomerIdAsync(long customerId) {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
                throw ApiException.NotFound("Khách hàng không tồn tại.");

            var reservations = await _context.Reservations
                .Where(r => r.CustomerId == customerId)
                .Include(r => r.Table)
                .Select(r => new ReservationResponse {
                    Id = r.Id,
                    CustomerId = r.CustomerId,
                    CustomerName = customer.FullName ?? string.Empty,
                    TableId = r.TableId,
                    TableName = r.Table != null ? r.Table.TableCode : string.Empty,
                    ReservationTime = r.ReservationTime,
                    GuestCount = r.GuestCount,
                    Status = r.Status
                }).ToListAsync();

            return ApiResponse<List<ReservationResponse>>.SuccessResponse(reservations);
        }

        public async Task<ApiResponse<object>> CreateReservationAsync(CreateReservationRequest req, long currentUserId, string userRole) {
            // 1. Xác định CustomerId dựa trên quyền hạn
            long finalCustomerId = (userRole == "CUSTOMER") ? currentUserId : req.CustomerId;

            // 2. Sử dụng Transaction với IsolationLevel cao để chống Race Condition (đặt trùng cùng lúc)
            using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try {
                // 3. Kiểm tra bàn có tồn tại không
                var table = await _context.Tables.FindAsync(req.TableId);
                if (table == null) throw ApiException.NotFound("Bàn không tồn tại.");

                // 4. KIỂM TRA TRÙNG LỊCH (AVAILABILITY)
                // Giả sử mỗi lượt đặt bàn chiếm chỗ trong 2 tiếng
                TimeSpan averageEatingTime = TimeSpan.FromHours(2);
                DateTime startTimeCheck = req.ReservationTime.Add(-averageEatingTime);
                DateTime endTimeCheck = req.ReservationTime.Add(averageEatingTime);

                bool isTableOccupied = await _context.Reservations.AnyAsync(r =>
                    r.TableId == req.TableId &&
                    (r.Status == Modal.Enums.ReservationStatus.PENDING || r.Status == Modal.Enums.ReservationStatus.CONFIRMED) &&
                    r.ReservationTime > startTimeCheck &&
                    r.ReservationTime < endTimeCheck
                );

                if (isTableOccupied) {
                    throw ApiException.BadRequest($"Bàn số {table.TableCode} đã có người đặt trong khoảng thời gian này. Vui lòng chọn giờ khác hoặc bàn khác.");
                }

                // 5. Kiểm tra số lượng khách có vượt quá sức chứa của bàn không (Optionally)
                if (req.GuestCount > table.SeatCount) {
                    throw ApiException.BadRequest($"Bàn này chỉ có tối đa {table.SeatCount} chỗ ngồi.");
                }

                // 6. Tạo bản ghi mới
                var reservation = new Reservation {
                    CustomerId = finalCustomerId,
                    TableId = req.TableId,
                    ReservationTime = req.ReservationTime, // Thời gian khách hẹn đến
                    GuestCount = req.GuestCount,
                    Status = Modal.Enums.ReservationStatus.PENDING
                };

                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();

                // 7. Hoàn tất transaction
                await transaction.CommitAsync();

                return ApiResponse<object>.SuccessResponse(null, "Đơn đặt bàn của bạn đã được ghi nhận và đang chờ duyệt.");
            } catch (Exception) {
                // Nếu có lỗi, rollback lại toàn bộ dữ liệu để tránh rác database
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
