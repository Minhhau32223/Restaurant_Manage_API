namespace RestaurantAPI.src.Exceptions {
    public class ApiException : Exception {
        public int StatusCode { get; }
        public ApiException(string message, int statusCode = 400) : base(message) {
            StatusCode = statusCode;
        }

        // 400: Dữ liệu gửi lên không hợp lệ
        public static ApiException BadRequest(string message) => new ApiException(message, 400);

        // 401: Chưa đăng nhập hoặc Token hết hạn
        public static ApiException Unauthorized(string message = "Vui lòng đăng nhập để tiếp tục") => new ApiException(message, 401);

        // 403: Đã đăng nhập nhưng không có quyền truy cập
        public static ApiException Forbidden(string message = "Bạn không có quyền thực hiện hành động này") => new ApiException(message, 403);

        // 404: Không tìm thấy tài nguyên   
        public static ApiException NotFound(string message) => new ApiException(message, 404);

        // 409: Xung đột dữ liệu (Vd: Đăng ký email đã tồn tại, hoặc đặt bàn đã có người ngồi)
        public static ApiException Conflict(string message) => new ApiException(message, 409);

        // 429: Quá nhiều yêu cầu (Spam API, Rate Limit)
        public static ApiException TooManyRequests(string message = "Thao tác quá nhanh, vui lòng thử lại sau") => new ApiException(message, 429);

        // 500: Lỗi hệ thống không xác định
        public static ApiException InternalServerError(string message = "Có lỗi hệ thống xảy ra") => new ApiException(message, 500);
    }
}