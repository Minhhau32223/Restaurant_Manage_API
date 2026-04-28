using Microsoft.EntityFrameworkCore;
using RestaurantAPI.src.Contract.Reservation.Request;
using RestaurantAPI.src.Data;
using RestaurantAPI.src.Exceptions;
using RestaurantAPI.src.Services.Interfaces;
using System.Security.Claims;

namespace RestaurantAPI.src.Route {
    public static class ReservationRoute {
        public static RouteGroupBuilder MapReservationRoute(this IEndpointRouteBuilder app) {
            var group = app.MapGroup("/api/reservations").WithTags("Reservation");

            // 1. LẤY TẤT CẢ (Chỉ STAFF/ADMIN)
            group.MapGet("", async (IReservationService service) => {
                var data = await service.GetAllReservationsAsync();
                return Results.Ok(data);
            })
            .RequireAuthorization("staff");

            // 2. LẤY CHI TIẾT (OWNER hoặc STAFF/ADMIN)
            group.MapGet("/{id:long}", async (long id, HttpContext context, IReservationService service, AppDbContext db) => {
                await CheckReservationOwner(id, context, db);
                var data = await service.GetReservationByIdAsync(id);
                return Results.Ok(data);
            })
            .RequireAuthorization("customer");

            // 3. LẤY THEO CUSTOMER ID (Chỉ chính mình hoặc STAFF)
            group.MapGet("/customer/{customerId:long}", async (long customerId, HttpContext context, IReservationService service) => {
                var userId = GetCurrentUserId(context);
                var role = GetCurrentUserRole(context);

                if (role == "CUSTOMER" && userId != customerId)
                    throw ApiException.Forbidden("Bạn không có quyền xem dữ liệu của người khác.");

                var data = await service.GetReservationsByCustomerIdAsync(customerId);
                return Results.Ok(data);
            })
            .RequireAuthorization("customer");

            // 4. TẠO ĐẶT CHỖ (Xử lý chống trùng và Owner)
            group.MapPost("", async (CreateReservationRequest req, HttpContext context, IReservationService service) => {
                Validator.ReservationValidator.ValidateCreate(req);

                var userId = GetCurrentUserId(context);
                var role = GetCurrentUserRole(context);

                // Gọi vào service đã có logic Transaction Serializable
                var result = await service.CreateReservationAsync(req, userId, role);
                return Results.Ok(result);
            })
            .RequireAuthorization("customer");

            // 5. CẬP NHẬT TRẠNG THÁI (Chỉ STAFF/ADMIN)
            group.MapPatch("/{id:long}/status", async (long id, UpdateReservationStatusRequest req, AppDbContext db) => {
                Validator.ReservationValidator.ValidateUpdateStatus(req);

                var reservation = await db.Reservations.FindAsync(id);
                if (reservation == null) throw ApiException.NotFound("Đặt chỗ không tồn tại.");

                reservation.Status = req.Status;
                await db.SaveChangesAsync();

                return Results.Ok(ApiResponse<object>.SuccessResponse(null, $"Trạng thái đã chuyển sang: {req.Status}"));
            })
            .RequireAuthorization("staff");

            return group;
        }

        private static long GetCurrentUserId(HttpContext context)
            => long.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        private static string GetCurrentUserRole(HttpContext context)
            => context.User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

        private static async Task CheckReservationOwner(long reservationId, HttpContext context, AppDbContext db) {
            var userId = GetCurrentUserId(context);
            var role = GetCurrentUserRole(context);

            if (role == "ADMIN" || role == "STAFF") return;

            var isOwner = await db.Reservations.AnyAsync(r => r.Id == reservationId && r.CustomerId == userId);
            if (!isOwner) throw ApiException.Forbidden("Bạn không có quyền truy cập thông tin này.");
        }
    }
}