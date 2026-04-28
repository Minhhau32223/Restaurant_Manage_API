using RestaurantAPI.src.Contract.CustomerVoucher.Request;
using RestaurantAPI.src.Services.Interfaces;
using RestaurantAPI.src.Validator;

namespace RestaurantAPI.src.Route {
    public static class VoucherRoute {
        public static RouteGroupBuilder MapVoucherRoute(this IEndpointRouteBuilder app) {
            var group = app.MapGroup("/api/vouchers").WithTags("Voucher");

            group.MapGet("", async (ICustomerVoucherService service) => {
                var data = await service.GetAllCustomerVouchersAsync();
                return Results.Ok(data);
            })
            .RequireAuthorization("staff")
            .WithName("GetAllVouchers")
            .WithSummary("Lấy danh sách voucher của tất cả khách hàng");

            group.MapGet("/customer/{customerId:long}", async (long customerId, ICustomerVoucherService service) => {
                var data = await service.GetCustomerVouchersByCustomerIdAsync(customerId);
                return Results.Ok(data);
            })
            .RequireAuthorization("Customer")
            .WithName("GetVouchersByCustomer")
            .WithSummary("Lấy voucher theo khách hàng");

            group.MapGet("/{voucherId:long}", async (long voucherId, ICustomerVoucherService service) => {
                var data = await service.GetCustomerVouchersByVoucherIdAsync(voucherId);
                return Results.Ok(data);
            })
            .WithName("GetVoucherById")
            .RequireAuthorization("staff")
            .WithSummary("Lấy voucher theo ID");

            group.MapPost("", async (CreateCustomerVoucherRequest req, ICustomerVoucherService service) => {
                CustomerVoucherValidator.ValidateCreate(req);
                var data = await service.CreateVoucherAsync(req);
                return Results.Ok(data);
            })
            .WithName("CreateVoucher")
            .RequireAuthorization("staff")
            .WithSummary("Tạo voucher cho khách hàng");

            group.MapPost("/use", async (UseVoucherRequest req, ICustomerVoucherService service) => {
                CustomerVoucherValidator.ValidateUse(req);
                var data = await service.UseVoucherAsync(req);
                return Results.Ok(data);
            })
            .WithName("UseVoucher")
            .RequireAuthorization("staff")
            .WithSummary("Sử dụng voucher");

            return group;
        }
    }
}
