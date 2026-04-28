using RestaurantAPI.src.Contract.Payment.Request;
using RestaurantAPI.src.Contract.Payment.Response;
using RestaurantAPI.src.Services.Interfaces;

namespace RestaurantAPI.src.Route {
    public static class PaymentRoute {
        public static RouteGroupBuilder MapPaymentRoute(this IEndpointRouteBuilder app) {
            var group = app.MapGroup("/api/payments")
                .WithTags("Payments (Thanh toán & Hóa đơn)")
                .RequireAuthorization("staff");

            group.MapPost("", async (CreatePaymentRequest req, IPaymentService service) => {
                var result = await service.Pay(req);
                return Results.Ok(ApiResponse<PaymentResponse>.SuccessResponse(result, "Thanh toán thành công. Bàn đã được giải phóng và kho đã khấu trừ."));
            })
            .WithName("CreatePayment")
            .WithSummary("Thực hiện thanh toán đơn hàng")
            .WithDescription("Quy trình: Tính giá -> Tạo Invoice -> Lưu Payment -> Trừ kho thực tế (Deduct Inventory) -> Giải phóng bàn.");

            return group;
        }
    }
}