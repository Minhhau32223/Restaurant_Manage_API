using RestaurantAPI.src.Contract.Table.Request;
using RestaurantAPI.src.Exceptions;
using RestaurantAPI.src.Modal.Enums;

namespace RestaurantAPI.src.Validator;

public class TableValidator {
    public static void ValidateCreate(CreateTableRequest request) {
        if (request == null)
            throw new BadRequestException("Request không hợp lệ");

        if (string.IsNullOrWhiteSpace(request.TableCode))
            throw new BadRequestException("Mã bàn không được để trống");

        if (request.SeatCount <= 0)
            throw new BadRequestException("Số chỗ phải lớn hơn 0");
    }

    public static void ValidateUpdate(UpdateTableRequest request) {
        if (request == null)
            throw new BadRequestException("Request không hợp lệ");

        if (string.IsNullOrWhiteSpace(request.TableCode))
            throw new BadRequestException("Mã bàn không được để trống");

        if (request.SeatCount <= 0)
            throw new BadRequestException("Số chỗ phải lớn hơn 0");
    }
}