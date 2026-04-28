using Microsoft.EntityFrameworkCore;
using RestaurantAPI.src.Contract.CustomerVoucher.Request;
using RestaurantAPI.src.Contract.CustomerVoucher.Response;
using RestaurantAPI.src.Data;
using RestaurantAPI.src.Exceptions;
using RestaurantAPI.src.Modal;
using RestaurantAPI.src.Services.Interfaces;

namespace RestaurantAPI.src.Services
{
    public class VoucherService : ICustomerVoucherService {
        private readonly AppDbContext _context;

        public VoucherService(AppDbContext context) {
            _context = context;
        }

        public async Task<ApiResponse<CustomerVoucherReponse>> CreateVoucherAsync(CreateCustomerVoucherRequest request) {
            var customer = await _context.Customers.FindAsync(request.CustomerId);
            if (customer == null)
                throw ApiException.NotFound("Khách hàng không tồn tại.");

            var exists = await _context.CustomerVouchers
                .AnyAsync(cv => cv.CustomerId == request.CustomerId && cv.Code == request.Code);
            if (exists)
                throw ApiException.Conflict("Mã voucher đã tồn tại cho khách hàng này.");

            var voucher = new CustomerVoucher {
                CustomerId = request.CustomerId,
                Code = request.Code,
                Discount = request.Discount,
                ExpiryDate = request.ExpiryDate,
                IsUsed = false
            };

            _context.CustomerVouchers.Add(voucher);
            await _context.SaveChangesAsync();

            var response = new CustomerVoucherReponse {
                Id = voucher.Id,
                Code = voucher.Code ?? string.Empty,
                Discount = voucher.Discount,
                ExpiryDate = voucher.ExpiryDate,
                IsUsed = voucher.IsUsed
            };

            return ApiResponse<CustomerVoucherReponse>.SuccessResponse(response, "Voucher được tạo thành công.");
        }

        public async Task<ApiResponse<List<CustomerVoucherReponse>>> GetAllCustomerVouchersAsync() {
            var vouchers = await _context.CustomerVouchers
                .Select(cv => new CustomerVoucherReponse {
                    Id = cv.Id,
                    Code = cv.Code ?? string.Empty,
                    Discount = cv.Discount,
                    ExpiryDate = cv.ExpiryDate,
                    IsUsed = cv.IsUsed
                }).ToListAsync();

            return ApiResponse<List<CustomerVoucherReponse>>.SuccessResponse(vouchers);
        }

        public async Task<ApiResponse<List<CustomerVoucherReponse>>> GetCustomerVouchersByCustomerIdAsync(long customerId) {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
                throw ApiException.NotFound("Khách hàng không tồn tại.");

            var vouchers = await _context.CustomerVouchers
                .Where(cv => cv.CustomerId == customerId)
                .Select(cv => new CustomerVoucherReponse {
                    Id = cv.Id,
                    Code = cv.Code ?? string.Empty,
                    Discount = cv.Discount,
                    ExpiryDate = cv.ExpiryDate,
                    IsUsed = cv.IsUsed
                }).ToListAsync();

            return ApiResponse<List<CustomerVoucherReponse>>.SuccessResponse(vouchers);
        }

        public async Task<ApiResponse<List<CustomerVoucherReponse>>> GetCustomerVouchersByVoucherIdAsync(long voucherId) {
            var voucher = await _context.CustomerVouchers.FindAsync(voucherId);
            if (voucher == null)
                throw ApiException.NotFound("Voucher không tồn tại.");

            var response = new CustomerVoucherReponse {
                Id = voucher.Id,
                Code = voucher.Code ?? string.Empty,
                Discount = voucher.Discount,
                ExpiryDate = voucher.ExpiryDate,
                IsUsed = voucher.IsUsed
            };

            return ApiResponse<List<CustomerVoucherReponse>>.SuccessResponse(new List<CustomerVoucherReponse> { response });
        }

        public async Task<ApiResponse<CustomerVoucherReponse>> UseVoucherAsync(UseVoucherRequest request) {
            var voucher = await _context.CustomerVouchers
                .FirstOrDefaultAsync(cv => cv.Id == request.VoucherId && cv.CustomerId == request.CustomerId);

            if (voucher == null)
                throw ApiException.NotFound("Voucher không tồn tại cho khách hàng này.");

            if (voucher.IsUsed)
                throw ApiException.BadRequest("Voucher đã được sử dụng.");

            if (voucher.ExpiryDate <= DateTime.Now)
                throw ApiException.BadRequest("Voucher đã hết hạn.");

            voucher.IsUsed = true;
            await _context.SaveChangesAsync();

            var response = new CustomerVoucherReponse {
                Id = voucher.Id,
                Code = voucher.Code ?? string.Empty,
                Discount = voucher.Discount,
                ExpiryDate = voucher.ExpiryDate,
                IsUsed = voucher.IsUsed
            };

            return ApiResponse<CustomerVoucherReponse>.SuccessResponse(response, "Sử dụng voucher thành công.");
        }
    }
}
