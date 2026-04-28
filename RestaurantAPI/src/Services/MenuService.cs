using Microsoft.EntityFrameworkCore;
using RestaurantAPI.src.Contract.Menu.Request;
using RestaurantAPI.src.Data;
using RestaurantAPI.src.Exceptions;
using RestaurantAPI.src.Modal;
using RestaurantAPI.src.Services.Interfaces;

namespace RestaurantAPI.src.Services {
    public class MenuService : IMenuService {
        private readonly AppDbContext _context;
        public MenuService(AppDbContext context) {
            _context = context;
        }

        public async Task<MenuCategory> CreateMenuCategory(CreateMenuCategoryRequest request) {
            // Kiểm tra trùng tên Category
            var isExist = await _context.MenuCategories
                .AnyAsync(c => c.Name.ToLower() == request.name.Trim().ToLower());

            if (isExist)
                throw ApiException.Conflict($"Danh mục '{request.name}' đã tồn tại.");

            try {
                var item = new MenuCategory {
                    Name = request.name.Trim()
                };

                _context.MenuCategories.Add(item);
                await _context.SaveChangesAsync();
                return item;
            } catch (Exception) {
                throw ApiException.InternalServerError("Có lỗi xảy ra khi tạo danh mục.");
            }
        }

        public async Task<MenuItem> CreateMenuItem(CreateMenuItemRequest request) {
            // 1. Kiểm tra Category có tồn tại không
            var categoryExists = await _context.MenuCategories.AnyAsync(c => c.Id == request.CategoryId);
            if (!categoryExists)
                throw ApiException.BadRequest($"Không tìm thấy danh mục với ID {request.CategoryId}");

            // 2. Kiểm tra trùng tên món ăn trong cùng một danh mục (hoặc toàn hệ thống tùy bạn)
            // Thường thì món ăn không nên trùng tên trên toàn thực đơn
            var isItemExist = await _context.MenuItems
                .AnyAsync(m => m.Name.ToLower() == request.Name.Trim().ToLower());

            if (isItemExist)
                throw ApiException.Conflict($"Món ăn '{request.Name}' đã tồn tại trong thực đơn.");

            try {
                var item = new MenuItem {
                    CategoryId = request.CategoryId,
                    Name = request.Name.Trim(),
                    Price = request.Price,
                    ImageUrl = request.ImageUrl,
                    Description = request.Description,
                    Status = Modal.Enums.StatusMenuItem.AVAILABLE
                };

                _context.MenuItems.Add(item);
                await _context.SaveChangesAsync();

                return await _context.MenuItems
                    .Include(x => x.Category)
                    .FirstOrDefaultAsync(x => x.Id == item.Id);
            } catch (Exception) {
                throw ApiException.InternalServerError("Có lỗi xảy ra khi tạo món ăn.");
            }
        }

        public async Task<List<MenuCategory>> GetAllMenuCategory() {
            return await _context.MenuCategories.AsNoTracking().ToListAsync();
        }

        public async Task<List<MenuItem>> GetAllMenuItem() {
            return await _context.MenuItems
                .Include(x => x.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<MenuItem> UpdateMenuItem(long id, UpdateMenuItemRequest request) {
            var item = await _context.MenuItems.FindAsync(id);
            if (item == null) throw ApiException.NotFound($"Không tìm thấy món ăn ID {id}");

            // Kiểm tra trùng tên khi cập nhật (Trừ chính nó ra)
            var isExist = await _context.MenuItems
                .AnyAsync(m => m.Name.ToLower() == request.Name.Trim().ToLower() && m.Id != id);

            if (isExist)
                throw ApiException.Conflict($"Tên món ăn '{request.Name}' đã được sử dụng bởi món khác.");

            try {
                item.Name = request.Name.Trim();
                item.Price = request.Price;
                item.ImageUrl = request.ImageUrl;
                item.Description = request.Description;

                await _context.SaveChangesAsync();
                await _context.Entry(item).Reference(x => x.Category).LoadAsync();
                return item;
            } catch (Exception) {
                throw ApiException.InternalServerError("Lỗi khi cập nhật món ăn.");
            }
        }

        public async Task<bool> UpdateMenuItemStatus(long id, UpdateMenuItemStatusRequest request) {
            var item = await _context.MenuItems.FindAsync(id);
            if (item == null) throw ApiException.NotFound($"Không tìm thấy món ăn ID {id}");

            try {
                item.Status = request.Status;
                await _context.SaveChangesAsync();
                return true;
            } catch (Exception) {
                throw ApiException.InternalServerError("Lỗi khi cập nhật trạng thái.");
            }
        }
    }
}