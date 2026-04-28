using RestaurantAPI.src.Contract.Payment.Request;
using RestaurantAPI.src.Exceptions;

namespace RestaurantAPI.src.Validator {
    public class PaymentValidator {
        public static void Validate(CreatePaymentRequest request) {
            if (request.OrderId <= 0)
                throw new BadRequestException("OrderId không hợp lệ");
        }
    }
}
