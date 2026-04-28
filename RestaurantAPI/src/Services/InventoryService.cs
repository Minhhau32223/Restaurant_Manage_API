using Microsoft.EntityFrameworkCore;
using RestaurantAPI.src.Contract.Inventory.Request;
using RestaurantAPI.src.Contract.Inventory.Response;
using RestaurantAPI.src.Data;
using RestaurantAPI.src.Exceptions;
using RestaurantAPI.src.Modal;
using RestaurantAPI.src.Modal.Enums;
using RestaurantAPI.src.Services.Interfaces;
namespace RestaurantAPI.src.Services {
    public class InventoryService : IInventoryService {
        private readonly AppDbContext _context;
        public InventoryService(AppDbContext context) {
            _context = context;
        }
        public async Task<ApiResponse<List<InventoryLogResponse>>> GetInventoryLogsByIngredientAsync(long ingredientId) {
            var ingredient = await _context.Ingredients.FindAsync(ingredientId);
            
            if (ingredient == null) {
                throw new NotFoundException($"Không tìm thấy nguyên liệu với ID {ingredientId}.");
            }

            var logsQuery = _context.InventoryLogs
                .Where(log => log.IngredientId == ingredientId)
                .OrderByDescending(log => log.CreateAt);

            var logsEntities = await logsQuery.ToListAsync();

            var logs = logsEntities.Select(log => new InventoryLogResponse {
                Id = log.Id,
                IngredientId = log.IngredientId,
                IngredientName = ingredient.Name,
                LogType = log.LogType.ToString(),
                Quantity = log.Quantity,
                ExpiryDate = log.ExpiryDate,
                CreatedAt = log.CreateAt
            }).ToList();
            return ApiResponse<List<InventoryLogResponse>>.SuccessResponse(logs);
        }

        public async Task<ApiResponse<List<InventoryLogResponse>>> InventoryInAsync(InventoryInRequest request) {
            var ingredient = await _context.Ingredients.FindAsync(request.IngredientId);
            if (ingredient == null) {
                throw ApiException.NotFound($"Ingredient with ID {request.IngredientId} not found.");
            }

            ingredient.StockQuantity += request.Quantity;
            var log = new InventoryLogs {
                IngredientId = request.IngredientId,
                LogType = Modal.Enums.InventoryLogType.IN,
                Quantity = request.Quantity,
                ExpiryDate = request.ExpiryDate,
                CreateAt = DateTime.UtcNow
            };

            _context.InventoryLogs.Add(log);
            await _context.SaveChangesAsync();
            
            var response = new InventoryLogResponse {
                Id = log.Id,
                IngredientId = log.IngredientId,
                IngredientName = ingredient.Name,
                LogType = log.LogType.ToString(),
                Quantity = log.Quantity,
                ExpiryDate = log.ExpiryDate,
                CreatedAt = log.CreateAt
            }; 

            return ApiResponse<List<InventoryLogResponse>>.SuccessResponse(new List<InventoryLogResponse> { response });
        }

        public async Task<ApiResponse<List<InventoryLogResponse>>> InventoryOutAsync(InventoryOutRequest request) {
            var ingredient = await _context.Ingredients.FindAsync(request.IngredientId);
            if (ingredient == null) {
                throw ApiException.NotFound($"Ingredient with ID {request.IngredientId} not found.");
            }
            if (ingredient.StockQuantity < request.Quantity) {
                throw ApiException.BadRequest($"Not enough stock for ingredient ID {request.IngredientId}. Current stock: {ingredient.StockQuantity}");
            }

            ingredient.StockQuantity -= request.Quantity;
            var log = new InventoryLogs {
                IngredientId = request.IngredientId,
                LogType = Modal.Enums.InventoryLogType.OUT,
                Quantity = request.Quantity,
                CreateAt = DateTime.UtcNow
            };

            _context.InventoryLogs.Add(log);
            await _context.SaveChangesAsync();

            var response = new InventoryLogResponse {
                Id = log.Id,
                IngredientId = log.IngredientId,
                IngredientName = ingredient.Name,
                LogType = log.LogType.ToString(),
                Quantity = log.Quantity,
                ExpiryDate = log.ExpiryDate,
                CreatedAt = DateTime.UtcNow
            };
            return ApiResponse<List<InventoryLogResponse>>.SuccessResponse(new List<InventoryLogResponse> { response });
        }

        public async Task<bool> DeductStockFromOrderAsync(long orderId) {
            // 1. Tìm đơn hàng
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) throw ApiException.NotFound($"Không tìm thấy đơn hàng #{orderId}");

            // 2. KIỂM TRA TRẠNG THÁI
            if (order.Status == OrderStatus.COMPLETED) {
                throw ApiException.BadRequest($"Đơn hàng #{orderId} đã được khấu trừ kho và hoàn tất trước đó.");
            }

            // Lưu ý: Khi gọi từ PaymentService, Status lúc này đã là PAID nên sẽ pass qua check này
            if (order.Status != OrderStatus.PAID) {
                throw ApiException.BadRequest("Chỉ đơn hàng ở trạng thái 'Đã thanh toán' (PAID) mới có thể khấu trừ kho.");
            }

            // --- BỎ DÒNG KHỞI TẠO TRANSACTION TẠI ĐÂY ---

            try {
                // 3. Lấy danh sách món ăn trong đơn
                var orderDetails = await _context.OrderItems
                    .Where(od => od.OrderId == orderId)
                    .ToListAsync();

                if (!orderDetails.Any()) throw ApiException.BadRequest("Đơn hàng không có món ăn nào.");

                foreach (var detail in orderDetails) {
                    // 4. Tìm công thức (Recipe) của từng món
                    var recipes = await _context.Recipes
                        .Where(r => r.MenuItemId == detail.MenuItemId)
                        .ToListAsync();

                    foreach (var recipe in recipes) {
                        // Tính toán lượng trừ: (Định lượng món) x (Số lượng khách đặt)
                        decimal amountToDeduct = recipe.Quantity * (decimal)detail.Quantity;

                        var ingredient = await _context.Ingredients.FindAsync(recipe.IngredientId);
                        if (ingredient != null) {
                            // 5. Cập nhật số dư kho của nguyên liệu
                            ingredient.StockQuantity -= amountToDeduct;

                            // 6. Ghi Log xuất kho (LogType = OUT)
                            _context.InventoryLogs.Add(new InventoryLogs {
                                IngredientId = ingredient.Id,
                                LogType = InventoryLogType.OUT,
                                Quantity = amountToDeduct,
                                CreateAt = DateTime.UtcNow
                            });
                        }
                    }
                }

                // 7. Chuyển trạng thái đơn hàng sang COMPLETED
                order.Status = OrderStatus.COMPLETED;

                // Lưu thay đổi vào ChangeTracker (Transaction bên ngoài sẽ thực thi commit sau)
                await _context.SaveChangesAsync();

                // --- BỎ DÒNG COMMIT TRANSACTION TẠI ĐÂY ---
                return true;
            } catch (Exception) {
                // --- BỎ DÒNG ROLLBACK TRANSACTION TẠI ĐÂY ---
                // Quăng lỗi ra ngoài để Transaction ở PaymentService tự động Rollback
                throw;
            }
        }
    }
}
