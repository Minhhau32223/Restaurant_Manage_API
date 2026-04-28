using Microsoft.EntityFrameworkCore;
using RestaurantAPI.src.Contract.Ingredients.Request;
using RestaurantAPI.src.Contract.Ingredients.Response;
using RestaurantAPI.src.Data;
using RestaurantAPI.src.Exceptions;
using RestaurantAPI.src.Modal;
using RestaurantAPI.src.Services.Interfaces;
namespace RestaurantAPI.src.Services
{
    public class IngredientService : IIngredientServices
    {
        private readonly AppDbContext _dbContext;
        public IngredientService(AppDbContext dbContext) {
            _dbContext = dbContext;
        }
        public async Task<ApiResponse<IngredientResponse>> CreateIngredientAsync(CreateIngredientRequest request) {
            var exists = await _dbContext.Ingredients.AnyAsync(i => i.Name == request.Name);
            if(exists) {
                throw ApiException.Conflict($"Nguyên liệu với tên '{request.Name}' đã tồn tại.");
            }
            var ingredients = new Ingredients {
                Name = request.Name,
                Unit = request.Unit,
                StockQuantity = request.StockQuantity,
                MinQuantity = request.MinQuantity
            };

            _dbContext.Ingredients.Add(ingredients);
            await _dbContext.SaveChangesAsync();
             var response = new IngredientResponse {
                Id = ingredients.Id,
                Name = ingredients.Name,
                Unit = ingredients.Unit,
                StockQuantity = ingredients.StockQuantity,
                MinQuantity = ingredients.MinQuantity
            };

            return ApiResponse<IngredientResponse>.SuccessResponse(response, "Nguyên liệu đã được tạo thành công.");
        }

        public async Task<ApiResponse<List<IngredientResponse>>> GetAllIngredientAsync() {
            var ingredients = await _dbContext.Ingredients.Select(i => new IngredientResponse {
                Id = i.Id,
                Name = i.Name,
                Unit = i.Unit,
                StockQuantity = i.StockQuantity,
                MinQuantity= i.MinQuantity
            }).ToListAsync();
            return ApiResponse<List<IngredientResponse>>.SuccessResponse(ingredients, "Danh sách nguyên liệu đã được lấy thành công.");
        }

        public async Task<ApiResponse<IngredientResponse>> GetIngredientByIdAsync(long id) {
            var ingredient = await _dbContext.Ingredients.FindAsync(id);

            if (ingredient == null) {
                throw ApiException.NotFound($"Không tìm thấy nguyên liệu với ID: {id}");
            }

            var response = new IngredientResponse {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Unit = ingredient.Unit,
                StockQuantity = ingredient.StockQuantity,
                MinQuantity = ingredient.MinQuantity
            };

            return ApiResponse<IngredientResponse>.SuccessResponse(response);
        }
        

        public async Task<ApiResponse<IngredientResponse>> UpdateIngredientAsync(long id, UpdateIngredientRequest request) {
            var ingredients = await _dbContext.Ingredients.FindAsync(request.Id);
            if (ingredients == null) {
                throw ApiException.NotFound($"Nguyên liệu với ID '{request.Id}' không tồn tại.");
            }

            var exists = await _dbContext.Ingredients.AnyAsync(i => i.Name == request.Name && i.Id != id);
            if (exists) {
                throw ApiException.Conflict($"Nguyên liệu với tên '{request.Name}' đã tồn tại.");
            }

            ingredients.Name = request.Name;
            ingredients.Unit = request.Unit;
            ingredients.MinQuantity = request.MinQuantity;
            await _dbContext.SaveChangesAsync();
            var response = new IngredientResponse {
                Id = ingredients.Id,
                Name = ingredients.Name,
                Unit = ingredients.Unit,
                StockQuantity = ingredients.StockQuantity,
                MinQuantity = ingredients.MinQuantity
            };

            return ApiResponse<IngredientResponse>.SuccessResponse(response, "Nguyên liệu đã được cập nhật thành công.");
        }
    }
}
