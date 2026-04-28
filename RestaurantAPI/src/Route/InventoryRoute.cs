using RestaurantAPI.src.Contract.Inventory.Request;
using RestaurantAPI.src.Exceptions;
using RestaurantAPI.src.Services.Interfaces;
using RestaurantAPI.src.Validator;

namespace RestaurantAPI.src.Route {
    public static class InventoryRoute {
        public static RouteGroupBuilder MapInventoryRoute(this IEndpointRouteBuilder app) {
            var group = app.MapGroup("/api/inventory")
                .WithTags("Inventory")
                .RequireAuthorization("staff");

            // Inventory In
            group.MapPost("/in", async (InventoryInRequest request, IInventoryService service) => {
                InventoryValidator.ValidateIn(request);
                var data = await service.InventoryInAsync(request);
                return Results.Ok(data);
            })
            .WithName("GlobalInventoryIn")
            .WithSummary("Nhập kho nguyên liệu");

            // Inventory Out
            group.MapPost("/out", async (InventoryOutRequest request, IInventoryService service) => {
                InventoryValidator.ValidateOut(request);
                var data = await service.InventoryOutAsync(request);
                return Results.Ok(data);
            })
            .WithName("GlobalInventoryOut")
            .WithSummary("Xuất kho nguyên liệu");

            // Get inventory logs by ingredient
            group.MapGet("/logs/{ingredientId:long}", async (long ingredientId, IInventoryService service) => {
                var data = await service.GetInventoryLogsByIngredientAsync(ingredientId);
                return Results.Ok(data);
            })
            .WithName("GetInventoryLogs")
            .WithSummary("Lấy lịch sử nhập/xuất theo nguyên liệu");

            group.MapPost("/deduct-from-order/{orderId:long}", async Task<IResult> (long orderId, IInventoryService service) => {
                try {
                    await service.DeductStockFromOrderAsync(orderId);
                    return Results.Ok(ApiResponse<object>.SuccessResponse(null, "Khấu trừ kho thành công."));
                } catch (ApiException ex) {
                    return Results.BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
                } catch (Exception) {
                    return Results.StatusCode(500);
                }
            })
            .WithName("DeductStockFromOrder")
            .WithSummary("Khấu trừ kho thủ công dựa trên ID đơn hàng")
            .RequireAuthorization("staff");

            return group;
        }
    }
}
