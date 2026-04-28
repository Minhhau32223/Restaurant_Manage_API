using Microsoft.EntityFrameworkCore;
using RestaurantAPI.src.Contract.Combo.Request;
using RestaurantAPI.src.Contract.Combo.Response;
using RestaurantAPI.src.Data;
using RestaurantAPI.src.Exceptions;
using RestaurantAPI.src.Modal;
using RestaurantAPI.src.Services.Interfaces;
using RestaurantAPI.src.Validator;

namespace RestaurantAPI.src.Services {
    public class ComboService : IComboService {
        private readonly AppDbContext _appDbContext;
        public ComboService(AppDbContext appDbContext) {
            _appDbContext = appDbContext;
        }
        public async Task<ComboResponse> CreateCombo(CreateComboRequest request) {
            ComboValidator.ValidateCreate(request);

            var menuItemIds = request.Items.Select(x => x.MenuItemId).ToList();

            var menuItems = await _appDbContext.MenuItems
                .Where(x => menuItemIds.Contains(x.Id))
                .ToListAsync();

            if (menuItems.Count != menuItemIds.Count)
                throw ApiException.BadRequest("Có món không tồn tại");

            var combo = new Combo {
                Name = request.Name,
            };

            decimal totalPrice = 0;

            foreach (var item in request.Items) {
                var menu = menuItems.First(x => x.Id == item.MenuItemId);

                totalPrice += menu.Price * item.Quantity;

                combo.Items.Add(new ComboItem {
                    MenuItemId = item.MenuItemId,
                    Quantity = item.Quantity
                });
            }

            combo.Price = totalPrice;

            _appDbContext.Combos.Add(combo);
            await _appDbContext.SaveChangesAsync();

            return MapToResponse(combo, menuItems);
        }

        public async Task<ComboResponse> AddItem(CreateComboItemRequest request) {
            var combo = await _appDbContext.Combos
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == request.ComboId);

            if (combo == null)
                throw ApiException.NotFound("Combo không tồn tại");

            var exist = combo.Items
                .FirstOrDefault(x => x.MenuItemId == request.MenuItemId);

            if (exist != null)
                throw ApiException.BadRequest("Món đã tồn tại trong combo");

            combo.Items.Add(new ComboItem {
                MenuItemId = request.MenuItemId,
                Quantity = request.Quantity
            });

            await RecalculatePrice(combo);

            await _appDbContext.SaveChangesAsync();

            return await GetComboId((int)combo.Id);
        }

        public async Task<ComboResponse> RemoveItem(long comboId, long menuItemId) {
            var combo = await _appDbContext.Combos
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == comboId);

            if (combo == null)
                throw ApiException.NotFound("Combo không tồn tại");

            var item = combo.Items
                .FirstOrDefault(x => x.MenuItemId == menuItemId);

            if (item == null)
                throw ApiException.NotFound("Item không tồn tại");

            combo.Items.Remove(item);

            await RecalculatePrice(combo);

            await _appDbContext.SaveChangesAsync();

            return await GetComboId((int)combo.Id);
        }

        public async Task<ComboResponse> UpdateItem(long comboId, long menuItemId, int quantity) {
            if (quantity <= 0)
                throw ApiException.BadRequest("Quantity phải > 0");

            var combo = await _appDbContext.Combos
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == comboId);

            var item = combo?.Items.FirstOrDefault(x => x.MenuItemId == menuItemId);

            if (item == null)
                throw ApiException.NotFound("Item không tồn tại");

            item.Quantity = quantity;

            await RecalculatePrice(combo);

            await _appDbContext.SaveChangesAsync();

            return await GetComboId((int)combo.Id);
        }

        public async Task<ComboItem> CreateComboItem(CreateComboItemRequest ComboItemRequest) {
            var exist = await _appDbContext.ComboItems
            .FirstOrDefaultAsync(x => x.ComboId == ComboItemRequest.ComboId
                               && x.MenuItemId == ComboItemRequest.MenuItemId);

            if (exist != null)
                return null;
            var item =  new ComboItem {
                ComboId = ComboItemRequest.ComboId,
                MenuItemId = ComboItemRequest.MenuItemId,
                Quantity = ComboItemRequest.Quantity
            };

            _appDbContext.ComboItems.Add(item);
            await _appDbContext.SaveChangesAsync();
            return item;

        }

        public async Task<ComboResponse> GetComboId(long id) {
            var combo = await _appDbContext.Combos
                .Include(c => c.Items)
                .ThenInclude(i => i.MenuItem)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (combo == null)
                throw ApiException.NotFound("Combo không tồn tại");

            return MapToResponse(combo);
        }

        private async Task RecalculatePrice(Combo combo) {
            var menuIds = combo.Items.Select(x => x.MenuItemId).ToList();

            var menus = await _appDbContext.MenuItems
                .Where(x => menuIds.Contains(x.Id))
                .ToListAsync();

            combo.Price = combo.Items.Sum(i =>
            {
                var menu = menus.First(x => x.Id == i.MenuItemId);
                return menu.Price * i.Quantity;
            });
        }

        private ComboResponse MapToResponse(Combo combo, List<MenuItem>? menuItems = null) {
            return new ComboResponse {
                Id = combo.Id,
                Name = combo.Name,
                Price = combo.Price,
                Items = combo.Items.Select(i => {
                    var menu = menuItems?.FirstOrDefault(x => x.Id == i.MenuItemId) ?? i.MenuItem;

                    return new ComboItemResponse {
                        MenuItemId = i.MenuItemId,
                        MenuItemName = menu?.Name,
                        Price = menu?.Price ?? 0,
                        Quantity = i.Quantity
                    };
                }).ToList()
            };
        }
        public async Task<List<ComboResponse>> GetAllCombos() {
            var combos = await _appDbContext.Combos
                .Include(c => c.Items)
                .ThenInclude(i => i.MenuItem)
                .AsNoTracking()
                .ToListAsync();

            return combos.Select(c => MapToResponse(c)).ToList();
        }
    }
}
