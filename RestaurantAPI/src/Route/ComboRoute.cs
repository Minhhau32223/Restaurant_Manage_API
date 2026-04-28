using RestaurantAPI.src.Contract.Combo.Request;
using RestaurantAPI.src.Services.Interfaces;
using RestaurantAPI.src.Validator;

namespace RestaurantAPI.src.Route {
    public static class ComboRoute {
        public static RouteGroupBuilder MapCombosRoute(this IEndpointRouteBuilder app) {
            var combo = app.MapGroup("/api/combos")
                .WithTags("Combos (Suất ăn/Combo)")
                .WithDescription("Quản lý các gói combo món ăn và tự động tính toán giá tiền");

            // GET: Xem chi tiết combo (Công khai)
            combo.MapGet("/{id:long}", async (IComboService s, long id) =>
                Results.Ok(await s.GetComboId((int)id)))
                .WithName("GetComboById")
                .WithSummary("Lấy thông tin chi tiết một Combo")
                .AllowAnonymous();

            // POST: Tạo combo mới
            combo.MapPost("", async (IComboService s, CreateComboRequest req) => {
                ComboValidator.ValidateCreate(req);
                return Results.Ok(await s.CreateCombo(req));
            })
            .RequireAuthorization("staff")
            .WithSummary("Tạo Combo mới (Tự động tính tổng giá)");

            // POST: Thêm món vào combo hiện có
            combo.MapPost("/items", async (IComboService s, CreateComboItemRequest req) => {
                ComboValidator.ValidateAddItem(req);
                return Results.Ok(await s.AddItem(req));
            })
            .RequireAuthorization("staff")
            .WithSummary("Thêm một món ăn mới vào Combo");

            // DELETE: Xóa món khỏi combo
            combo.MapDelete("/{comboId:long}/items/{menuItemId:long}", async (IComboService s, long comboId, long menuItemId) =>
                Results.Ok(await s.RemoveItem(comboId, menuItemId)))
            .RequireAuthorization("staff")
            .WithSummary("Xóa món ăn khỏi Combo");

            // PUT: Cập nhật số lượng món trong combo
            combo.MapPut("/{comboId:long}/items/{menuItemId:long}", async (IComboService s, long comboId, long menuItemId, int quantity) =>
                Results.Ok(await s.UpdateItem(comboId, menuItemId, quantity)))
            .RequireAuthorization("staff")
            .WithSummary("Cập nhật số lượng của một món trong Combo");

            // GET: Lấy danh sách tất cả combo
            combo.MapGet("", async (IComboService s) =>
                Results.Ok(await s.GetAllCombos()))
            .WithName("GetAllCombos")
            .WithSummary("Lấy danh sách tất cả các gói Combo hiện có")
            .WithDescription("Trả về danh sách đầy đủ các combo kèm theo chi tiết món ăn và tổng giá.");
            return combo;
        }
    }
}