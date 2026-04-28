using RestaurantAPI.src.Contract.Invoice.Response;

namespace RestaurantAPI.src.Services.Interfaces {
    public interface IInvoiceService {
        Task<InvoiceDetailResponse> GetByOrderId(long orderId);
        Task<InvoiceDetailResponse> GetById(long invoiceId);
    }
}
