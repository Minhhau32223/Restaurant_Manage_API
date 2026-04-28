namespace RestaurantAPI.src.Exceptions;

public class BadRequestException : Exception {
    public BadRequestException(string message) : base(message) { 
    
    }
}