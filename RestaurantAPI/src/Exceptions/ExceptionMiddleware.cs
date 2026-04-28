using System.Text.Json;

namespace RestaurantAPI.src.Exceptions;

public class ExceptionMiddleware {
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger) {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context) {
        try {
            await _next(context);
        } catch (Exception ex) {
            await HandleException(context, ex);
        }
    }

    private async Task HandleException(HttpContext context, Exception ex) {
        context.Response.ContentType = "application/json";

        // Mặc định là lỗi 500
        int statusCode = StatusCodes.Status500InternalServerError;
        string message = "Lỗi hệ thống, vui lòng thử lại sau";

        // Kiểm tra xem có phải lỗi do mình chủ động throw (ApiException) không
        if (ex is ApiException apiEx) {
            statusCode = apiEx.StatusCode;
            message = apiEx.Message;

            // Log Warning cho các lỗi nghiệp vụ (4xx)
            _logger.LogWarning("API Error: {Message} | StatusCode: {StatusCode} | Path: {Path}",
                message, statusCode, context.Request.Path);
        } else {
            // Log Error cho các lỗi hệ thống không xác định (500)
            _logger.LogError(ex, "Unhandled exception occurred at {Path}", context.Request.Path);
        }

        // Thiết lập Http Status Code cho Response
        context.Response.StatusCode = statusCode;

        // Khởi tạo Response theo format của bạn
        var response = ApiResponse<object>.ErrorResponse(message);

        // Serialize với CamelCase để Front-end dễ dùng (Vd: success thay vì Success)
        var options = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}