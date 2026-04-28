using RestaurantAPI.src.Contract;
namespace RestaurantAPI.src.Extensions
{
    public static class EndpointExtensions
    {
        public static IResult Ok<T>(T data, string message = "success")
        {
            return Results.Ok(ApiResponse<T>.SuccessResponse(data, message));
        }

        public static IResult Fail<T>(string message, int statusCode = 400)
        {
            return Results.Json(ApiResponse<T>.ErrorResponse(message), statusCode: statusCode);
        }
        public static IResult NotFound<T>(string message = "NotFound", int statusCode = 404)
        {
            return Results.Json(ApiResponse<T>.ErrorResponse(message), statusCode: statusCode);

        }
        public static IResult Unauthorized<T>(string message = "Unauthorized", int statusCode = 401)
        {
            return Results.Json(ApiResponse<T>.ErrorResponse(message), statusCode: statusCode);
        }

    }   
}
