using Microsoft.EntityFrameworkCore;
using RestaurantAPI.src.Contract.Recipe.Request;
using RestaurantAPI.src.Data;
using RestaurantAPI.src.Exceptions;
using RestaurantAPI.src.Modal;
using RestaurantAPI.src.Services.Interfaces;

namespace RestaurantAPI.src.Services {
    public class RecipeService : IRecipeService {
        private readonly AppDbContext _appDbContext;
        public RecipeService(AppDbContext appDbContext) {
            _appDbContext = appDbContext;
        }

        public async Task<List<Recipe>> CreateBulkRecipe(CreateBulkRecipeRequest request) {
            // 1. Kiểm tra món ăn có tồn tại không
            var menuItem = await _appDbContext.MenuItems.FindAsync(request.MenuItemId);
            if (menuItem == null) throw ApiException.NotFound("Món ăn không tồn tại");

            using var transaction = await _appDbContext.Database.BeginTransactionAsync();
            try {
                // 2. Lấy danh sách ID nguyên liệu mới từ request để so sánh
                var newIngredientIds = request.Ingredients.Select(i => i.IngredientId).ToList();

                // 3. XÓA: Những Recipe cũ của món này mà IngredientId KHÔNG nằm trong danh sách mới
                var recipesToRemove = await _appDbContext.Recipes
                    .Where(r => r.MenuItemId == request.MenuItemId && !newIngredientIds.Contains(r.IngredientId))
                    .ToListAsync();

                if (recipesToRemove.Any()) {
                    _appDbContext.Recipes.RemoveRange(recipesToRemove);
                }

                // 4. XỬ LÝ THÊM/SỬA: Duyệt qua danh sách nguyên liệu mới
                foreach (var item in request.Ingredients) {
                    // Kiểm tra nguyên liệu có tồn tại trong bảng Ingredients không
                    var ingExists = await _appDbContext.Ingredients.AnyAsync(i => i.Id == item.IngredientId);
                    if (!ingExists) throw ApiException.NotFound($"Nguyên liệu ID {item.IngredientId} không tồn tại");

                    // Kiểm tra xem đã có Recipe này chưa
                    var existingRecipe = await _appDbContext.Recipes
                        .FirstOrDefaultAsync(r => r.MenuItemId == request.MenuItemId && r.IngredientId == item.IngredientId);

                    if (existingRecipe != null) {
                        // Cập nhật số lượng mới (Ghi đè hoàn toàn số lượng cũ)
                        existingRecipe.Quantity = item.Quantity;
                    } else {
                        // Nếu chưa có thì thêm mới
                        var newRecipe = new Recipe {
                            MenuItemId = request.MenuItemId,
                            IngredientId = item.IngredientId,
                            Quantity = item.Quantity
                        };
                        _appDbContext.Recipes.Add(newRecipe);
                    }
                }

                // 5. Lưu thay đổi và xác nhận Transaction
                await _appDbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                // 6. Trả về danh sách mới nhất (gọi hàm GetRecipeId để lấy đầy đủ Include tránh lỗi Null)
                return await GetRecipeId((int)request.MenuItemId);
            } catch {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Recipe>> GetRecipeId(int menuItemId) {
            var recipes = await _appDbContext.Recipes
                 .AsNoTracking() // Tối ưu hiệu năng cho truy vấn Read-only
                 .Where(x => x.MenuItemId == menuItemId)
                 .Include(x => x.Ingredients)
                 .Include(x => x.MenuItem)
                 .ThenInclude(m => m.Category)
                 .ToListAsync();

            if (recipes == null || !recipes.Any())
                throw ApiException.NotFound("Món ăn này chưa được thiết lập công thức.");

            return recipes;
        }

        public async Task<bool> DeleteRecipe(long menuItemId, long ingredientId) {
            var recipe = await _appDbContext.Recipes
                .FirstOrDefaultAsync(r => r.MenuItemId == menuItemId && r.IngredientId == ingredientId);

            if (recipe == null) throw ApiException.NotFound("Không tìm thấy thành phần này trong công thức.");

            _appDbContext.Recipes.Remove(recipe);
            await _appDbContext.SaveChangesAsync();
            return true;
        }
    }
}