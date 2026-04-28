using RestaurantAPI.src.Contract.Reservation.Request;
using RestaurantAPI.src.Exceptions;
using RestaurantAPI.src.Modal.Enums;
namespace RestaurantAPI.src.Validator;

public class ReservationValidator {
    public static void ValidateCreate(CreateReservationRequest request) {
        if (request == null)
            throw ApiException.BadRequest("Dữ liệu yêu cầu không được để trống.");

        if (request.TableId <= 0)
            throw ApiException.BadRequest("Mã bàn (TableId) không hợp lệ.");

        if (request.GuestCount <= 0)
            throw ApiException.BadRequest("Số lượng khách phải lớn hơn 0.");

        // Kiểm tra thời gian: Không được để trống và không được ở quá khứ
        if (request.ReservationTime == default)
            throw ApiException.BadRequest("Thời gian đặt chỗ không được để trống.");

        if (request.ReservationTime < DateTime.UtcNow)
            throw ApiException.BadRequest("Thời gian đặt chỗ không được ở trong quá khứ.");
    }

    public static void ValidateUpdateStatus(UpdateReservationStatusRequest request) {
        if (request == null)
            throw ApiException.BadRequest("Dữ liệu cập nhật không hợp lệ.");

        // Kiểm tra xem giá trị Enum gửi lên có nằm trong định nghĩa của ReservationStatus không
        if (!Enum.IsDefined(typeof(ReservationStatus), request.Status)) {
            var validStatuses = Enum.GetNames(typeof(ReservationStatus));
            throw ApiException.BadRequest($"Trạng thái không hợp lệ. Các giá trị cho phép: {string.Join(", ", validStatuses)}");
        }
    }
}