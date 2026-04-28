using RestaurantAPI.src.Contract.Pricing.Request;
using RestaurantAPI.src.Contract.Pricing.Response;

namespace RestaurantAPI.src.Services.Interfaces {
    public interface IPricingService {
        Task<PricingResponse> CalculateAsync(PricingRequest request);

        Task<PricingResponse> PreviewAsync(PricingPreviewRequest request);
    }
}
