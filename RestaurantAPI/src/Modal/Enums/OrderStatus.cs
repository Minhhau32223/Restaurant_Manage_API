namespace RestaurantAPI.src.Modal.Enums {
    public enum OrderStatus {
        OPEN, // Đơn hàng mới được tạo, chưa thanh toán, chưa giao hàng, chưa trừ kho.
        PAID, // Đơn hàng đã được thanh toán nhưng chưa giao hàng, chưa trừ kho.
        COMPLETED, // Đã giao hàng và đã trừ kho xong.
        CANCELLED // Đơn hàng đã bị hủy, có thể do khách hàng hủy hoặc do nhà hàng hủy. Đơn hàng ở trạng thái này sẽ không được trừ kho.
    }
}
