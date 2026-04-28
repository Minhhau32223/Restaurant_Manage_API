using RestaurantAPI.src.Contract.Table.Request;
using RestaurantAPI.src.Contract.Table.Response;
using RestaurantAPI.src.Exceptions;
using RestaurantAPI.src.Services.Interfaces;
using RestaurantAPI.src.Validator;

namespace RestaurantAPI.src.Route;

public static class TableRoute {
    public static RouteGroupBuilder MapTableRoute(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/api/tables")
            .WithTags("Table");

        group.MapPost("", async (CreateTableRequest request, ITableService service) => {
            TableValidator.ValidateCreate(request);
            var data = await service.Create(request);
            return Results.Ok(ApiResponse<TableResponse>.SuccessResponse(data, "Tạo bàn thành công"));
        })
        .RequireAuthorization("admin")
        .WithName("CreateTable");

        group.MapPut("/{id:long}", async (long id, UpdateTableRequest request, ITableService service) => {
            TableValidator.ValidateUpdate(request);
            var data = await service.Update(id, request);
            return Results.Ok(ApiResponse<TableResponse>.SuccessResponse(data, "Cập nhật thành công"));
        })
        .RequireAuthorization("admin")
        .WithName("UpdateTable");

        group.MapDelete("/{id:long}", async (long id, ITableService service) => {
            await service.Delete(id);
            return Results.Ok(ApiResponse<object>.SuccessResponse(null, "Xóa thành công"));
        })
        .RequireAuthorization("admin")
        .WithName("DeleteTable");

        group.MapGet("", async (ITableService service) => {
            var data = await service.GetAll();
            return Results.Ok(ApiResponse<List<TableResponse>>.SuccessResponse(data));
        })
        .AllowAnonymous()
        .WithName("GetAllTables");

        group.MapGet("/{id:long}", async (long id, ITableService service) => {
            var data = await service.GetById(id);
            return Results.Ok(ApiResponse<TableResponse>.SuccessResponse(data));
        })
        .AllowAnonymous()
        .WithName("GetTableById");

        group.MapGet("/status", async (ITableService service) => {
            var data = await service.GetStatus();
            return Results.Ok(ApiResponse<List<TableStatusResponse>>.SuccessResponse(data));
        })
        .RequireAuthorization("staff")
        .WithName("GetTableStatus");

        group.MapPut("/reset/{id:long}", async (long id, ITableService service) => {
            await service.Reset(id);
            return Results.Ok(ApiResponse<object>.SuccessResponse(null, "Reset bàn thành công"));
        })
        .RequireAuthorization("staff")
        .WithName("ResetTable");

        // MERGE TABLES
        group.MapPut("/merge", async (long sourceId, long targetId, ITableService service) => {
            if (sourceId == targetId)
                throw new BadRequestException("Không thể gộp một bàn vào chính nó");

            await service.MergeTable(sourceId, targetId);

            return Results.Ok(ApiResponse<object>.SuccessResponse(null, "Đã gộp bàn {targetId} vào bàn {sourceId}"));
        })
        .RequireAuthorization("staff")
        .WithName("MergeTable")
        .WithSummary("Gộp bàn nguồn vào bàn đích (Chuyển món và giải phóng bàn nguồn)");

        return group;
    }
}