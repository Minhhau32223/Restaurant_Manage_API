using RestaurantAPI.src.Contract.CustomerVoucher.Request;
using RestaurantAPI.src.Exceptions;

namespace RestaurantAPI.src.Validator {
    public class CustomerVoucherValidator {
        public static void ValidateCreate(CreateCustomerVoucherRequest request) {
            if (request == null)
                throw ApiException.BadRequest("Dữ liệu yêu cầu không được để trống.");

            if (request.CustomerId <= 0)
                throw ApiException.BadRequest("Mã khách hàng (CustomerId) không hợp lệ.");

            if (string.IsNullOrWhiteSpace(request.Code))
                throw ApiException.BadRequest("Mã Voucher không được để trống.");

            if (request.Discount <= 0)
                throw ApiException.BadRequest("Mức giảm giá phải lớn hơn 0.");

            if (request.ExpiryDate <= DateTime.Now)
                throw ApiException.BadRequest("Ngày hết hạn phải là một thời điểm trong tương lai.");
        }

        public static void ValidateUse(UseVoucherRequest request) {
            if (request == null)
                throw ApiException.BadRequest("Dữ liệu yêu cầu không được để trống.");

            if (request.VoucherId <= 0)
                throw ApiException.BadRequest("Mã Voucher không hợp lệ.");

            if (request.CustomerId <= 0)
                throw ApiException.BadRequest("Mã khách hàng không hợp lệ.");
        }
    }
}