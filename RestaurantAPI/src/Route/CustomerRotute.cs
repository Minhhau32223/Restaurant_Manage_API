using System.Security.Claims;
using RestaurantAPI.src.Contract.Customer.Request;
using RestaurantAPI.src.Services.Interfaces;
using RestaurantAPI.src.Validator;

namespace RestaurantAPI.src.Route {
    public static class CustomerRoute {
        public static RouteGroupBuilder MapCustomerRoute(this IEndpointRouteBuilder app) {
            var group = app.MapGroup("/api/customers").WithTags("Customer");

            group.MapGet("/me", async (HttpContext context, ICustomerService service) => {
                var accountIdStr = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountIdStr)) return Results.Unauthorized();

                var accountId = long.Parse(accountIdStr);
                var data = await service.GetMyProfileAsync(accountId);
                return Results.Ok(data);
            })
            .RequireAuthorization("customer")
            .WithName("GetMyProfile")
            .WithSummary("Lấy thông tin cá nhân của khách hàng đang đăng nhập");

            group.MapGet("", async (ICustomerService service) => {
                var data = await service.GetAllCustomersAsync();
                return Results.Ok(data);
            })
            .RequireAuthorization("staff")
            .WithName("GetAllCustomers");

            group.MapGet("/{id:long}", async (long id, ICustomerService service) => {
                var data = await service.GetCustomerByIdAsync(id);
                return Results.Ok(data);
            })
            .RequireAuthorization("staff")
            .WithName("GetCustomerById");

            group.MapGet("/phone/{phone}", async (string phone, ICustomerService service) => {
                var data = await service.GetCustomerByPhoneAsync(phone);
                return Results.Ok(data);
            })
            .RequireAuthorization("staff")
            .WithName("GetCustomerByPhone");

            group.MapPost("", async (CreateCustomerRequest req, ICustomerService service) => {
                CustomerValidator.ValidateCreate(req);
                var data = await service.CreateCustomerAsync(req);
                return Results.Ok(data);
            })
            .RequireAuthorization("staff")
            .WithName("CreateCustomer");

            group.MapPut("/{id:long}", async (long id, UpdateCustomerRequest req, ICustomerService service) => {
                req.Id = id;
                CustomerValidator.ValidateUpdate(req);
                var data = await service.UpdateCustomerAsync(req);
                return Results.Ok(data);
            })
            .RequireAuthorization("customer")
            .WithName("UpdateCustomer");

            return group;
        }
    }
}