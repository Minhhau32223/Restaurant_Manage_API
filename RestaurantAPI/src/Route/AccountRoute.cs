using Microsoft.EntityFrameworkCore;
using RestaurantAPI.src.Contract.Acount.Request;
using RestaurantAPI.src.Contract.Acount.Response;
using RestaurantAPI.src.Data;
using RestaurantAPI.src.Exceptions;
using RestaurantAPI.src.Extensions;
using RestaurantAPI.src.Validator;
using System.Security.Claims;

namespace RestaurantAPI.src.Route {
    public static class AccountRoute {
        public static RouteGroupBuilder MapAccountRoute(this IEndpointRouteBuilder app) {
            var group = app.MapGroup("/api/accounts")
                .WithTags("Account");

            // 1. Chỉ Admin mới thấy hết tất cả account
            group.MapGet("/", GetAllAccounts)
                .RequireAuthorization("admin")
                .Produces<ApiResponse<IEnumerable<AccountResponse>>>(StatusCodes.Status200OK)
                .WithName("GetAllAccounts");

            // 2. Admin, Staff thấy mọi người. Customer CHỈ thấy chính mình.
            group.MapGet("/{id:long}", GetAccountById)
                .RequireAuthorization("customer")
                .Produces<ApiResponse<AccountResponse>>(StatusCodes.Status200OK)
                .WithName("GetAccountById");

            // 3. Admin tạo tài khoản
            group.MapPost("/", CreateAccount)
                .RequireAuthorization("admin")
                .Produces<ApiResponse<AccountResponse>>(StatusCodes.Status201Created)
                .WithName("CreateAccount");

            // 4. Admin sửa mọi người. Customer CHỈ sửa chính mình.
            group.MapPut("/{id:long}", UpdateAccount)
                .RequireAuthorization("customer")
                .Produces<ApiResponse<AccountResponse>>(StatusCodes.Status200OK)
                .WithName("UpdateAccount");

            // 5. Chỉ Admin mới được khóa/mở tài khoản
            group.MapPatch("/{id:long}/status", PatchAccount)
                .RequireAuthorization("admin")
                .Produces<ApiResponse<AccountResponse>>(StatusCodes.Status200OK)
                .WithName("UpdateStatusAccount");

            // 6. Chỉ Admin mới được xóa tài khoản
            group.MapDelete("/{id:long}", DeleteAccount)
                .RequireAuthorization("admin")
                .WithName("DeleteAccount");

            return group;
        }

        static async Task<IResult> GetAllAccounts(AppDbContext db, CancellationToken ct = default) {
            var accounts = await db.Accounts.OrderBy(a => a.Id).ToListAsync(ct);
            var response = accounts.Select(AccountResponse.FromEntity);
            return EndpointExtensions.Ok(response, "Lấy danh sách tài khoản thành công");
        }

        static async Task<IResult> GetAccountById(AppDbContext db, HttpContext context, long id, CancellationToken ct = default) {
            // Kiểm tra quyền "Chính chủ"
            CheckOwnerOrStaff(context, id);

            var account = await db.Accounts.FindAsync([id], ct);
            if (account == null) throw ApiException.NotFound($"Account với id {id} không tồn tại");

            return EndpointExtensions.Ok(AccountResponse.FromEntity(account), "Lấy tài khoản thành công");
        }

        static async Task<IResult> CreateAccount(AppDbContext db, CreateAccountRequest req, CancellationToken ct = default) {
            AccountValidator.ValidateCreateAccountRequest(req);

            var existingAccount = await db.Accounts.FirstOrDefaultAsync(a => a.Username == req.Username, ct);
            if (existingAccount != null) throw ApiException.Conflict("Username đã tồn tại");

            var newAccount = new Modal.Account {
                Username = req.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                Role = req.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            db.Accounts.Add(newAccount);
            await db.SaveChangesAsync(ct);
            return EndpointExtensions.Ok(AccountResponse.FromEntity(newAccount), "Tạo tài khoản thành công");
        }

        static async Task<IResult> UpdateAccount(AppDbContext db, HttpContext context, long id, UpdateAccountRequest req, CancellationToken ct = default) {
            // Chỉ chính chủ hoặc Admin mới được sửa
            CheckOwnerOrAdmin(context, id);

            var account = await db.Accounts.FindAsync([id], ct);
            if (account == null) throw ApiException.NotFound("Không tìm thấy tài khoản");

            AccountValidator.ValidateUpdateAccountRequest(req);

            if (!string.IsNullOrEmpty(req.Password))
                account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);

            // Chỉ ADMIN mới được sửa Role hoặc Status ở endpoint này
            var currentUserRole = context.User.FindFirst(ClaimTypes.Role)?.Value;
            if (currentUserRole == "ADMIN") {
                if (req.IsActive.HasValue) account.IsActive = req.IsActive.Value;
                if (req.Role.HasValue) account.Role = req.Role.Value;
            }

            await db.SaveChangesAsync(ct);
            return EndpointExtensions.Ok(AccountResponse.FromEntity(account), "Cập nhật thành công");
        }

        static async Task<IResult> DeleteAccount(AppDbContext db, long id, CancellationToken ct = default) {
            var account = await db.Accounts.FindAsync([id], ct);
            if (account == null) throw ApiException.NotFound("Không tìm thấy tài khoản");

            db.Accounts.Remove(account);
            await db.SaveChangesAsync(ct);
            return EndpointExtensions.Ok("Xóa account thành công");
        }

        static async Task<IResult> PatchAccount(AppDbContext db, long id, UpdateStatusAccountRequest req, CancellationToken ct = default) {
            var account = await db.Accounts.FindAsync([id], ct);
            if (account == null) throw ApiException.NotFound("Không tìm thấy tài khoản");

            AccountValidator.ValidateUpdateStatusAccountRequest(req);
            account.IsActive = req.IsActive;

            await db.SaveChangesAsync(ct);
            return EndpointExtensions.Ok(AccountResponse.FromEntity(account), "Cập nhật trạng thái thành công");
        }

        // --- HELPER CHECK QUYỀN ---

        private static void CheckOwnerOrStaff(HttpContext context, long targetId) {
            var currentUserId = long.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "ADMIN" || role == "STAFF") return; 

            if (currentUserId != targetId) {
                throw ApiException.Forbidden("Bạn không có quyền xem thông tin của người khác");
            }
        }

        private static void CheckOwnerOrAdmin(HttpContext context, long targetId) {
            var currentUserId = long.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

            if (role != "ADMIN" && currentUserId != targetId) {
                throw ApiException.Forbidden("Bạn không có quyền chỉnh sửa tài khoản này");
            }
        }
    }
}