using RestaurantAPI.src.Contract.Order.Request;
using RestaurantAPI.src.Contract.Order.Response;
using RestaurantAPI.src.Modal;

namespace RestaurantAPI.src.Services.Interfaces {
    public interface IOrderService {
        Task<OrderResponse> Create(CreateOrderRequest request);
        Task<OrderDetailResponse> GetById(long id);

        Task<OrderDetailResponse> AddItem(long orderId, AddOrderItemRequest request);
        Task<OrderDetailResponse> UpdateItem(long orderId, long itemId, int quantity);
        Task<OrderDetailResponse> DeleteItem(long orderId, long itemId);
    }
}
