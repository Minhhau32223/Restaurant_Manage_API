using RestaurantAPI.src.Contract.Pricing.Request;
using RestaurantAPI.src.Services.Interfaces;

namespace RestaurantAPI.src.Route {
    public static class PricingRoute {
        public static RouteGroupBuilder MapPricingRoute(this IEndpointRouteBuilder app) {
            var group = app.MapGroup("/api/pricing")
                .WithTags("Pricing");

            // PREVIEW (client gửi items)
            group.MapPost("/preview", async (PricingPreviewRequest request, IPricingService pricingService) => {
                var result = await pricingService.PreviewAsync(request);
                return Results.Ok(result);
            })
            .WithSummary("Preview giá đơn hàng")
            .WithDescription("Tính tiền dựa trên danh sách món từ client");

            // CALCULATE (order từ DB)
            group.MapPost("/calculate", async (PricingRequest request, IPricingService pricingService) => {
                var result = await pricingService.CalculateAsync(request);
                return Results.Ok(result);
            })
            .WithSummary("Tính tiền đơn hàng")
            .WithDescription("Tính tiền dựa trên order trong hệ thống");

            return group;
        }
    }
}