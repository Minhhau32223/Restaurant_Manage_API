using Microsoft.EntityFrameworkCore;
using RestaurantAPI.src.Contract.Customer.Request;
using RestaurantAPI.src.Contract.Customer.Response;
using RestaurantAPI.src.Contract.CustomerVoucher.Response;
using RestaurantAPI.src.Contract.Reservation.Response;
using RestaurantAPI.src.Data;
using RestaurantAPI.src.Exceptions;
using RestaurantAPI.src.Modal;
using RestaurantAPI.src.Services.Interfaces;

namespace RestaurantAPI.src.Services {
    public class CustomerService : ICustomerService {
        private readonly AppDbContext _context;
        public CustomerService(AppDbContext context) {
            _context = context;
        }

        public async Task<ApiResponse<CustomerResponse>> CreateCustomerAsync(CreateCustomerRequest request) {
            var exists = await _context.Customers.AnyAsync(c => c.Phone == request.Phone);
            if (exists) {
                throw ApiException.BadRequest("Số điện thoại đã được đăng ký.");
            }

            var customer = new Customer {
                FullName = request.FullName,
                Phone = request.Phone,
                Email = request.Email,
                Points = 0,
                AccountId = null // Mặc định null nếu tạo thủ công bởi nhân viên
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return await GetCustomerByIdAsync(customer.Id);
        }

        public async Task<ApiResponse<List<CustomerResponse>>> GetAllCustomersAsync() {
            var customers = await _context.Customers
                .AsNoTracking()
                .Select(c => new CustomerResponse {
                    Id = c.Id,
                    FullName = c.FullName,
                    Phone = c.Phone,
                    Email = c.Email,
                    Points = c.Points,
                    AccountId = c.AccountId
                }).ToListAsync();

            return ApiResponse<List<CustomerResponse>>.SuccessResponse(customers);
        }

        public async Task<ApiResponse<CustomerResponse>> GetCustomerByIdAsync(long id) {
            // Sử dụng Projection (.Select) để tối ưu SQL và lấy luôn Account thông qua navigation property
            var response = await _context.Customers
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CustomerResponse {
                    Id = c.Id,
                    FullName = c.FullName,
                    Phone = c.Phone,
                    Email = c.Email,
                    Points = c.Points,
                    AccountId = c.AccountId,
                    Username = c.Account != null ? c.Account.Username : null,
                    Reservations = c.Reservations.Select(r => new ReservationResponse {
                        Id = r.Id,
                        ReservationTime = r.ReservationTime,
                        CustomerId = r.CustomerId,
                        CustomerName = c.FullName,
                        TableId = r.TableId,
                        GuestCount = r.GuestCount,
                        Status = r.Status,
                    }).ToList(),
                    Voucher = c.CustomerVouchers.Select(cv => new CustomerVoucherReponse {
                        Id = cv.Id,
                        Code = cv.Code,
                        Discount = cv.Discount,
                        ExpiryDate = cv.ExpiryDate,
                        IsUsed = cv.IsUsed
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (response == null) {
                throw ApiException.NotFound("Khách hàng không tồn tại.");
            }

            return ApiResponse<CustomerResponse>.SuccessResponse(response);
        }

        public async Task<ApiResponse<CustomerResponse>> GetCustomerByPhoneAsync(string phone) {
            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Phone == phone);

            if (customer == null) {
                throw ApiException.NotFound($"Không tìm thấy khách hàng với số điện thoại: {phone}");
            }

            return await GetCustomerByIdAsync(customer.Id);
        }

        public async Task<ApiResponse<CustomerResponse>> GetMyProfileAsync(long accountId) {
            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.AccountId == accountId);

            if (customer == null) {
                throw ApiException.NotFound("Không tìm thấy hồ sơ khách hàng liên kết với tài khoản này.");
            }

            return await GetCustomerByIdAsync(customer.Id);
        }

        public async Task<ApiResponse<CustomerResponse>> UpdateCustomerAsync(UpdateCustomerRequest request) {
            var customer = await _context.Customers.FindAsync(request.Id);
            if (customer == null)
                throw ApiException.NotFound("Khách hàng không tồn tại.");

            customer.FullName = request.FullName;
            customer.Email = request.Email;

            await _context.SaveChangesAsync();

            return await GetCustomerByIdAsync(customer.Id);
        }
    }
}