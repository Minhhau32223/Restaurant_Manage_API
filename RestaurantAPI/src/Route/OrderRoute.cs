using RestaurantAPI.src.Contract.Order.Request;
using RestaurantAPI.src.Contract.Order.Response;
using RestaurantAPI.src.Services.Interfaces;

namespace RestaurantAPI.src.Route;

public static class OrderRoute {
    public static RouteGroupBuilder MapOrderRoute(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/api/orders")
            .WithTags("Orders (Đơn hàng & Phục vụ)")
            .RequireAuthorization("staff");

        group.MapPost("", async (CreateOrderRequest req, IOrderService service) =>
            Results.Ok(ApiResponse<OrderResponse>.SuccessResponse(await service.Create(req))))
            .WithSummary("Mở bàn mới");

        group.MapGet("/{id:long}", async (long id, IOrderService service) =>
            Results.Ok(ApiResponse<OrderDetailResponse>.SuccessResponse(await service.GetById(id))))
            .WithSummary("Chi tiết đơn hàng");

        group.MapPost("/{id:long}/items", async (long id, AddOrderItemRequest req, IOrderService service) =>
            Results.Ok(ApiResponse<OrderDetailResponse>.SuccessResponse(await service.AddItem(id, req))))
            .WithSummary("Thêm món lẻ/Combo vào đơn");

        group.MapPut("/{id:long}/items/{itemId:long}", async (long id, long itemId, UpdateOrderItemRequest req, IOrderService service) =>
            Results.Ok(ApiResponse<OrderDetailResponse>.SuccessResponse(await service.UpdateItem(id, itemId, req.Quantity))))
            .WithSummary("Cập nhật số lượng món");

        group.MapDelete("/{id:long}/items/{itemId:long}", async (long id, long itemId, IOrderService service) =>
            Results.Ok(ApiResponse<OrderDetailResponse>.SuccessResponse(await service.DeleteItem(id, itemId))))
            .WithSummary("Xóa món khỏi đơn");

        return group;
    }
}