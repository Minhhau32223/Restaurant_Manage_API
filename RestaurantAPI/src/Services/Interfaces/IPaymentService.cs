using RestaurantAPI.src.Contract.Payment.Request;
using RestaurantAPI.src.Contract.Payment.Response;
using RestaurantAPI.src.Modal;

namespace RestaurantAPI.src.Services.Interfaces {
    public interface IPaymentService {
        Task<PaymentResponse> Pay(CreatePaymentRequest request);
    }
}
