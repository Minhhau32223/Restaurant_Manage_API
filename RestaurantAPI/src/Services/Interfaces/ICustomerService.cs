using RestaurantAPI.src.Contract.Customer.Request;
using RestaurantAPI.src.Contract.Customer.Response;

namespace RestaurantAPI.src.Services.Interfaces {
    public interface ICustomerService {
        Task<ApiResponse<List<CustomerResponse>>> GetAllCustomersAsync();
        Task<ApiResponse<CustomerResponse>> GetCustomerByIdAsync(long id);
        Task<ApiResponse<CustomerResponse>> CreateCustomerAsync(CreateCustomerRequest request);
        Task<ApiResponse<CustomerResponse>> UpdateCustomerAsync(UpdateCustomerRequest request);
        Task<ApiResponse<CustomerResponse>> GetCustomerByPhoneAsync (string phone);
        Task<ApiResponse<CustomerResponse>> GetMyProfileAsync(long accountId);
    }
}
