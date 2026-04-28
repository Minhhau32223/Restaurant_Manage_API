using Microsoft.EntityFrameworkCore;
using RestaurantAPI.src.Contract.Order.Request;
using RestaurantAPI.src.Contract.Order.Response;
using RestaurantAPI.src.Data;
using RestaurantAPI.src.Exceptions;
using RestaurantAPI.src.Modal;
using RestaurantAPI.src.Modal.Enums;
using RestaurantAPI.src.Services.Interfaces;
using RestaurantAPI.src.Validator;

namespace RestaurantAPI.src.Services;

public class OrderService : IOrderService {
    private readonly AppDbContext _context;
    private readonly ITableService _tableService;

    public OrderService(AppDbContext context, ITableService tableService) {
        _context = context;
        _tableService = tableService;
    }

    // Kiểm tra kho ảo (Chỉ kiểm tra, chưa trừ)
    private async Task CheckStockAvailable(long? menuItemId, long? comboId, int quantity) {
        var itemsToCheck = new List<(long Id, int Qty)>();

        if (menuItemId.HasValue) {
            itemsToCheck.Add((menuItemId.Value, quantity));
        } else if (comboId.HasValue) {
            var comboItems = await _context.ComboItems
                .Where(ci => ci.ComboId == comboId.Value)
                .ToListAsync();
            foreach (var ci in comboItems) {
                itemsToCheck.Add((ci.MenuItemId, ci.Quantity * quantity));
            }
        }

        foreach (var item in itemsToCheck) {
            var recipes = await _context.Recipes.Where(r => r.MenuItemId == item.Id).ToListAsync();
            foreach (var r in recipes) {
                var ing = await _context.Ingredients.FindAsync(r.IngredientId);
                if (ing != null && ing.StockQuantity < (r.Quantity * item.Qty)) {
                    throw ApiException.BadRequest($"Nguyên liệu '{ing.Name}' không đủ cho đơn hàng.");
                }
            }
        }
    }

    public async Task<OrderResponse> Create(CreateOrderRequest request) {
        OrderValidator.ValidateCreate(request);
        var table = await _context.Tables.FindAsync(request.TableId) ?? throw ApiException.NotFound("Bàn không tồn tại");
        if (table.Status == TableStatus.OCCUPIED) throw ApiException.BadRequest("Bàn đang có khách");

        var order = new Order {
            TableId = request.TableId,
            AccountId = request.AccountId,
            CustomerId = request.CustomerId,
            Status = OrderStatus.OPEN,
            OrderTime = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        await _tableService.SetOccupied(request.TableId);
        await _context.SaveChangesAsync();
        return OrderResponse.Map(order);
    }

    public async Task<OrderDetailResponse> AddItem(long orderId, AddOrderItemRequest request) {
        OrderValidator.ValidateAddItem(request);

        using var transaction = await _context.Database.BeginTransactionAsync();
        try {
            var order = await _context.Orders.FindAsync(orderId)
                ?? throw ApiException.NotFound("Đơn hàng không tồn tại");

            if (order.Status != OrderStatus.OPEN)
                throw ApiException.BadRequest("Đơn hàng đã đóng, không thể thêm món.");

            // 1. Kiểm tra tồn kho (Virtual Check)
            await CheckStockAvailable(request.MenuItemId, request.ComboId, request.Quantity);

            // 2. Xử lý logic thêm món
            if (request.MenuItemId.HasValue) {
                // Trường hợp thêm món lẻ
                await ProcessAddMenuItem(orderId, request.MenuItemId.Value, request.Quantity);
            } else if (request.ComboId.HasValue) {
                // Trường hợp thêm Combo: Duyệt từng món trong combo để thêm vào đơn
                var comboItems = await _context.ComboItems
                    .Where(ci => ci.ComboId == request.ComboId.Value)
                    .ToListAsync();

                if (!comboItems.Any()) throw ApiException.BadRequest("Combo này không có món ăn nào.");

                foreach (var ci in comboItems) {
                    await ProcessAddMenuItem(orderId, ci.MenuItemId, ci.Quantity * request.Quantity);
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return await GetById(orderId);
        } catch (Exception) {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // Hàm phụ để xử lý thêm/cập nhật OrderItem (Tái sử dụng code)
    private async Task ProcessAddMenuItem(long orderId, long menuItemId, int quantity) {
        var menu = await _context.MenuItems.FindAsync(menuItemId)
            ?? throw ApiException.NotFound($"Món ăn ID {menuItemId} không tồn tại");

        var existing = await _context.OrderItems
            .FirstOrDefaultAsync(x => x.OrderId == orderId && x.MenuItemId == menuItemId);

        if (existing != null) {
            existing.Quantity += quantity;
        } else {
            _context.OrderItems.Add(new OrderItem {
                OrderId = orderId,
                MenuItemId = menuItemId,
                Quantity = quantity,
                Price = menu.Price
            });
        }
    }

    public async Task<OrderDetailResponse> GetById(long id) {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(oi => oi.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == id) ?? throw ApiException.NotFound("Không thấy đơn hàng");

        return new OrderDetailResponse {
            Id = order.Id,
            TableId = order.TableId,
            Status = order.Status,
            Items = order.Items.Select(oi => new OrderItemResponse {
                Id = oi.Id,
                MenuItemId = oi.MenuItemId,
                Name = oi.MenuItem?.Name ?? "N/A",
                Price = oi.Price,
                Quantity = oi.Quantity,
                SubTotal = oi.Price * oi.Quantity
            }).ToList(),
            Total = order.Items.Sum(x => x.Price * x.Quantity)
        };
    }

    public async Task<OrderDetailResponse> UpdateItem(long orderId, long itemId, int quantity) {
        var item = await _context.OrderItems.FindAsync(itemId) ?? throw ApiException.NotFound("Món không tồn tại trong đơn");
        await CheckStockAvailable(item.MenuItemId, null, quantity - item.Quantity);

        if (quantity <= 0) _context.OrderItems.Remove(item);
        else item.Quantity = quantity;

        await _context.SaveChangesAsync();
        return await GetById(orderId);
    }

    public async Task<OrderDetailResponse> DeleteItem(long orderId, long itemId) {
        var item = await _context.OrderItems.FindAsync(itemId) ?? throw ApiException.NotFound("Món không tồn tại");
        _context.OrderItems.Remove(item);
        await _context.SaveChangesAsync();
        return await GetById(orderId);
    }
}