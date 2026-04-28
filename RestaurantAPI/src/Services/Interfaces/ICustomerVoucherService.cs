using RestaurantAPI.src.Contract.CustomerVoucher.Request;
using RestaurantAPI.src.Contract.CustomerVoucher.Response;

namespace RestaurantAPI.src.Services.Interfaces
{
    public interface ICustomerVoucherService
    {
        Task<ApiResponse<List<CustomerVoucherReponse>>> GetAllCustomerVouchersAsync();
        Task<ApiResponse<List<CustomerVoucherReponse>>> GetCustomerVouchersByCustomerIdAsync(long customerId);
        Task<ApiResponse<List<CustomerVoucherReponse>>> GetCustomerVouchersByVoucherIdAsync(long voucherId);
        Task<ApiResponse<CustomerVoucherReponse>> CreateVoucherAsync (CreateCustomerVoucherRequest request);
        Task<ApiResponse<CustomerVoucherReponse>>UseVoucherAsync (UseVoucherRequest request);

    }
}
