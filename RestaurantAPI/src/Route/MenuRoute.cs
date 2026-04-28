using RestaurantAPI.src.Contract.Menu.Request;
using RestaurantAPI.src.Contract.Menu.Response;
using RestaurantAPI.src.Services.Interfaces;
using RestaurantAPI.src.Validator;

namespace RestaurantAPI.src.Route {
    public static class MenuRoute {
        public static void MapMenuRoute(this IEndpointRouteBuilder app) {
            var menu = app.MapGroup("/api")
                .WithTags("Menu")
                .WithDescription("Các endpoint liên quan đến quản lý menu, bao gồm danh mục và món ăn.");

            // GET: Categories (Công khai)
            menu.MapGet("/categories", async (IMenuService menuService) => {
                var menuCategory = await menuService.GetAllMenuCategory();
                return Results.Ok(ApiResponse<List<MenuCategoryResponse>>.SuccessResponse(menuCategory.Select(MenuCategoryResponse.Map).ToList()));
            })
            .WithDescription("Lấy danh sách tất cả danh mục menu.")
            .WithTags("Menu Categories")
            .AllowAnonymous();

            // POST: Categories (Chỉ Staff)
            menu.MapPost("/categories", async (IMenuService menuService, CreateMenuCategoryRequest request) => {
                MenuValidator.ValidateCreateCategory(request);
                var result = await menuService.CreateMenuCategory(request);
                return Results.Ok(ApiResponse<MenuCategoryResponse>.SuccessResponse(MenuCategoryResponse.Map(result)));
            })
            .WithDescription("Tạo mới một danh mục menu.")
            .RequireAuthorization("staff");

            // GET: Menu Items (Công khai)
            menu.MapGet("/menu-items", async (IMenuService menuService) => {
                var menuItem = await menuService.GetAllMenuItem();
                return Results.Ok(ApiResponse<List<MenuItemResponse>>.SuccessResponse(menuItem.Select(MenuItemResponse.Map).ToList()));
            })
            .WithTags("Menu Items")
            .WithDescription("Lấy danh sách tất cả món ăn trong menu. Chỉ trả về những món có trạng thái 'Available'.")
            .AllowAnonymous();

            // POST: Menu Items (Chỉ Staff)
            menu.MapPost("/menu-items", async (IMenuService menuService, CreateMenuItemRequest request) => {
                MenuValidator.ValidateCreateItem(request); // Thêm Validator
                var result = await menuService.CreateMenuItem(request);
                return Results.Ok(ApiResponse<MenuItemResponse>.SuccessResponse(MenuItemResponse.Map(result)));
            })
            .WithTags("Menu Items")
            .WithDescription("Tạo mới một món ăn trong menu.")
            .RequireAuthorization("staff");

            // PUT: Update Item (Chỉ Staff)
            menu.MapPut("/menu-items/{id}", async (IMenuService menuService, long id, UpdateMenuItemRequest request) => {
                MenuValidator.ValidateUpdateItem(request); // Thêm Validator
                var result = await menuService.UpdateMenuItem(id, request);
                return Results.Ok(ApiResponse<MenuItemResponse>.SuccessResponse(MenuItemResponse.Map(result)));
            })
            .WithTags("Menu Items")
            .WithDescription("Cập nhật thông tin của một món ăn trong menu.")
            .RequireAuthorization("staff");

            // PATCH: Update Status (Chỉ Staff)
            menu.MapPatch("/menu-items/{id}/status", async (IMenuService menuService, long id, UpdateMenuItemStatusRequest request) => {
                var status = await menuService.UpdateMenuItemStatus(id, request);
                return Results.Ok(ApiResponse<string>.SuccessResponse("Cập nhật trạng thái thành công"));
            })
            .WithTags("Menu Items")
            .WithDescription("Cập nhật trạng thái của một món ăn trong menu. Trạng thái có thể là 'Available', 'Unavailable', hoặc 'OutOfStock'.")
            .RequireAuthorization("staff");
        }
    }
}
