using Microsoft.EntityFrameworkCore;
using RestaurantAPI.src.Contract.Acount.Response;
using RestaurantAPI.src.Contract.Auth.Response;
using RestaurantAPI.src.Data;
using RestaurantAPI.src.Exceptions;
using RestaurantAPI.src.Extensions;
using RestaurantAPI.src.Modal;
using RestaurantAPI.src.Services.Interfaces;
using RestaurantAPI.src.Validator;
using AuthRequest = RestaurantAPI.src.Contract.Auth.Request.AuthRequest;

namespace RestaurantAPI.src.Route {
    public static class AuthRoute {
        public static RouteGroupBuilder MapAuthRoute(this IEndpointRouteBuilder app) {

            var group = app.MapGroup("/api/auth")
                .AllowAnonymous()
                .WithTags("Authentication");

            group.MapPost("/login", Login)
                .AllowAnonymous()
                .WithName("Login")
                .WithSummary("Đăng nhập");

            group.MapPost("/register", Register)
                .AllowAnonymous()
                .WithName("Register")
                .WithSummary("Đăng ký tài khoản");

            return group;
        }

        static async Task<IResult> Login(AppDbContext db, AuthRequest req, ITokenService tokenService, CancellationToken ct = default) {
            AuthValidator.Validate(req);

            var user = await db.Accounts
                .FirstOrDefaultAsync(u => u.Username == req.Username, ct);

            if (user == null)
                throw ApiException.NotFound("Tài khoản không tồn tại");

            if (!user.IsActive)
                throw ApiException.Forbidden("Tài khoản đã bị khóa");

            if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
                throw ApiException.BadRequest("Mật khẩu không chính xác");

            var token = tokenService.CreateToken(user);

            var response = new AuthResponse {
                Token = token,
                Account = AccountResponse.FromEntity(user)
            };

            return EndpointExtensions.Ok(response, "Đăng nhập thành công");
        }

        static async Task<IResult> Register(AppDbContext db, AuthRequest req, CancellationToken ct = default) {
            AuthValidator.Validate(req);

            // 2. Kiểm tra Username đã tồn tại chưa
            var existed = await db.Accounts
                .AnyAsync(x => x.Username == req.Username, ct);

            if (existed)
                throw ApiException.Conflict($"Username '{req.Username}' đã tồn tại");

            // 3. Sử dụng Transaction để đảm bảo tính toàn vẹn dữ liệu
            using var transaction = await db.Database.BeginTransactionAsync(ct);
            try {
                var hash = BCrypt.Net.BCrypt.HashPassword(req.Password);

                // 4. Tạo Account
                var account = new Account {
                    Username = req.Username,
                    PasswordHash = hash,
                    Role = Modal.Enums.Role.CUSTOMER,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                db.Accounts.Add(account);
                await db.SaveChangesAsync(ct); // Lưu để lấy ID của account

                var newCustomer = new Modal.Customer {
                        Points = 0,
                        AccountId = account.Id
                };
                db.Customers.Add(newCustomer);

                await db.SaveChangesAsync(ct);

                // Xác nhận hoàn tất cả 2 thao tác
                await transaction.CommitAsync(ct);

                return EndpointExtensions.Ok(AccountResponse.FromEntity(account), "Đăng ký thành công");
            } catch (Exception) {
                // Nếu có lỗi bất kỳ, hủy bỏ mọi thay đổi trong DB
                await transaction.RollbackAsync(ct);
                throw;
            }
        }
    }
}