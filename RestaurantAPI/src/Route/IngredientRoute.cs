using RestaurantAPI.src.Contract.Ingredients.Request;
using RestaurantAPI.src.Contract.Inventory.Request;
using RestaurantAPI.src.Services.Interfaces;

namespace RestaurantAPI.src.Route {
    public static class IngredientRoute {
        public static RouteGroupBuilder MapIngredientRoute(this IEndpointRouteBuilder app) {
            var group = app.MapGroup("/api/ingredients")
                .WithTags("Ingredient")
                .RequireAuthorization("staff");

            // GET all
            group.MapGet("", async (IIngredientServices service) => {
                var data = await service.GetAllIngredientAsync();
                return Results.Ok(data);
            })
            .WithName("GetAllIngredients")
            .WithSummary("Lấy danh sách nguyên liệu");

            // GET by id
            group.MapGet("/{id:long}", async (long id, IIngredientServices service) => {
                var data = await service.GetIngredientByIdAsync(id);
                return Results.Ok(data);
            })
            .WithName("GetIngredientById")
            .WithSummary("Lấy nguyên liệu theo ID");

            // CREATE
            group.MapPost("", async (CreateIngredientRequest request, IIngredientServices service) => {
                // Optional: validation via IngredientValidator if exists
                var data = await service.CreateIngredientAsync(request);
                return Results.Ok(data);
            })
            .WithName("CreateIngredient")
            .WithSummary("Tạo nguyên liệu mới");

            // UPDATE
            group.MapPut("/{id:long}", async (long id, UpdateIngredientRequest request, IIngredientServices service) => {
                // Ensure id matches request id
                request.Id = id;
                var data = await service.UpdateIngredientAsync(id, request);
                return Results.Ok(data);
            })
            .WithName("UpdateIngredient")
            .WithSummary("Cập nhật nguyên liệu");

            // Inventory in
            group.MapPost("/{id:long}/inventory/in", async (long id, InventoryInRequest req, IInventoryService inventoryService) => {
                req.IngredientId = id;
                var data = await inventoryService.InventoryInAsync(req);
                return Results.Ok(data);
            })
            .WithName("IngredientInventoryIn")
            .WithSummary("Nhập kho nguyên liệu");

            // Inventory out
            group.MapPost("/{id:long}/inventory/out", async (long id, InventoryOutRequest req, IInventoryService inventoryService) => {
                req.IngredientId = id;
                var data = await inventoryService.InventoryOutAsync(req);
                return Results.Ok(data);
            })
            .WithName("IngredientInventoryOut")
            .WithSummary("Xuất kho nguyên liệu");

            // Get inventory logs
            group.MapGet("/{id:long}/inventory/logs", async (long id, IInventoryService inventoryService) => {
                var data = await inventoryService.GetInventoryLogsByIngredientAsync(id);
                return Results.Ok(data);
            })
            .WithName("GetIngredientInventoryLogs")
            .WithSummary("Lấy lịch sử nhập/xuất nguyên liệu");

            return group;
        }
    }
}
