using RestaurantAPI.src.Contract.Invoice.Response;
using RestaurantAPI.src.Services.Interfaces;
public static class InvoiceRoute {
    public static RouteGroupBuilder MapInvoiceRoute(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/api/invoices")
            .WithTags("Invoices (Hóa đơn)")
            .RequireAuthorization("staff");
        // Xem tạm tính
        group.MapGet("/order/{orderId:long}", async (long orderId, IInvoiceService service) => {
            var data = await service.GetByOrderId(orderId);
            return Results.Ok(ApiResponse<InvoiceDetailResponse>.SuccessResponse(data));
        })
        .WithSummary("Xem hóa đơn tạm tính (Chưa thanh toán)");

        // Xem hóa đơn đã thanh toán
        group.MapGet("/{id:long}", async (long id, IInvoiceService service) => {
            var data = await service.GetById(id);
            return Results.Ok(ApiResponse<InvoiceDetailResponse>.SuccessResponse(data));
        })
        .WithSummary("Lấy thông tin hóa đơn đã thanh toán");

        return group;
    }
}