using Microsoft.EntityFrameworkCore;
using RestaurantAPI.src.Contract.Table.Request;
using RestaurantAPI.src.Contract.Table.Response;
using RestaurantAPI.src.Data;
using RestaurantAPI.src.Exceptions;
using RestaurantAPI.src.Modal;
using RestaurantAPI.src.Modal.Enums;
using RestaurantAPI.src.Services.Interfaces;

namespace RestaurantAPI.src.Services;

public class TableService : ITableService {
    private readonly AppDbContext _context;

    public TableService(AppDbContext context) {
        _context = context;
    }

    // CREATE
    public async Task<TableResponse> Create(CreateTableRequest request) {
        var exists = await _context.Tables
            .AnyAsync(t => t.TableCode == request.TableCode);

        if (exists)
            throw new BadRequestException("Mã bàn đã tồn tại");

        var table = new Table {
            TableCode = request.TableCode,
            SeatCount = request.SeatCount,
            Status = TableStatus.EMPTY
        };

        _context.Tables.Add(table);
        await _context.SaveChangesAsync();

        return TableResponse.MapToResponse(table);
    }

    // GET ALL
    public async Task<List<TableResponse>> GetAll() {
        var tables = await _context.Tables.ToListAsync();
        return tables.Select(TableResponse.MapToResponse).ToList();
    }

    // GET BY ID
    public async Task<TableResponse> GetById(long id) {
        var table = await _context.Tables.FindAsync(id);

        if (table == null)
            throw ApiException.NotFound("Không tìm thấy bàn");

        return TableResponse.MapToResponse(table);
    }

    // UPDATE
    public async Task<TableResponse> Update(long id, UpdateTableRequest request) {
        var table = await _context.Tables.FindAsync(id);
        if (table == null) throw ApiException.NotFound("Không tìm thấy bàn");

        var exists = await _context.Tables
            .AnyAsync(t => t.TableCode == request.TableCode && t.Id != id);
        if (exists) throw ApiException.BadRequest("Mã bàn đã tồn tại");

        // NGHIỆP VỤ: Nếu đang có khách ngồi, không cho phép đổi trạng thái thủ công sang EMPTY qua API này
        if (table.Status == TableStatus.OCCUPIED && request.Status == TableStatus.EMPTY) {
            var hasOrder = await _context.Orders.AnyAsync(o => o.TableId == id && o.Status == OrderStatus.OPEN);
            if (hasOrder) throw  ApiException.BadRequest("Bàn đang có khách và Order chưa đóng, không thể đổi trạng thái");
        }

        table.TableCode = request.TableCode;
        table.SeatCount = request.SeatCount;
        table.Status = request.Status;

        await _context.SaveChangesAsync();
        return TableResponse.MapToResponse(table);
    }

    // DELETE (SAFE)
    public async Task<bool> Delete(long id) {
        var table = await _context.Tables.FindAsync(id);

        if (table == null)
            throw ApiException.NotFound("Không tìm thấy bàn");

        var hasActiveOrder = await _context.Orders
            .AnyAsync(o => o.TableId == id && o.Status == OrderStatus.OPEN);

        if (hasActiveOrder)
            throw ApiException.BadRequest("Bàn đang có order, không thể xóa");

        _context.Tables.Remove(table);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<TableStatusResponse>> GetStatus() {
        return await _context.Tables
            .Select(t => new {
                Table = t,
                OpenOrder = _context.Orders
                    .Where(o => o.TableId == t.Id && o.Status == OrderStatus.OPEN)
                    .Select(o => new {
                        o.Id,
                        Total = _context.OrderItems
                            .Where(oi => oi.OrderId == o.Id)
                            .Sum(oi => oi.Price * oi.Quantity)
                    })
                    .FirstOrDefault()
            })
            .Select(x => new TableStatusResponse {
                TableId = x.Table.Id,
                TableCode = x.Table.TableCode,
                Status = x.Table.Status,
                OrderId = x.OpenOrder != null ? x.OpenOrder.Id : null,
                Total = x.OpenOrder != null ? x.OpenOrder.Total : 0
            })
            .ToListAsync();
    }

    public async Task<bool> Reset(long tableId) {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try {
            var table = await _context.Tables.FindAsync(tableId);
            if (table == null) throw ApiException.NotFound("Không tìm thấy bàn");

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.TableId == tableId && o.Status == OrderStatus.OPEN);

            if (order != null) {
                var hasItems = await _context.OrderItems.AnyAsync(i => i.OrderId == order.Id);
                if (hasItems)
                    throw ApiException.BadRequest("Bàn đã gọi món, phải thanh toán hoặc hủy món trước khi reset");

                order.Status = OrderStatus.CANCELLED;
            }

            table.Status = TableStatus.EMPTY;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        } catch {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task SetOccupied(long tableId) {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try {
            // 2. Tìm bàn và kiểm tra trạng thái hiện tại
            var table = await _context.Tables.FindAsync(tableId);

            if (table == null)
                throw ApiException.NotFound("Không tìm thấy bàn");

            // 3. QUAN TRỌNG: Nếu bàn không còn trống, báo lỗi ngay
            if (table.Status != TableStatus.EMPTY) {
                throw ApiException.BadRequest($"Bàn {table.TableCode} hiện đang {table.Status}, không thể mở.");
            }

            // 4. Tiến hành cập nhật
            table.Status = TableStatus.OCCUPIED;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        } catch {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task SetEmpty(long tableId) {
        var table = await _context.Tables.FindAsync(tableId);

        if (table == null)
            throw ApiException.NotFound("Không tìm thấy bàn");

        table.Status = TableStatus.EMPTY;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> MergeTable(long sourceTableId, long targetTableId) {
        var sourceOrder = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.TableId == sourceTableId && o.Status == OrderStatus.OPEN);

        var targetOrder = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.TableId == targetTableId && o.Status == OrderStatus.OPEN);

        if (sourceOrder == null)
            throw ApiException.BadRequest("Bàn nguồn không có khách để gộp.");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try {
            if (targetOrder == null) {
                // Chuyển bàn: Cập nhật trực tiếp TableId của Order
                sourceOrder.TableId = targetTableId;

                var targetTable = await _context.Tables.FindAsync(targetTableId);
                if (targetTable != null) targetTable.Status = TableStatus.OCCUPIED;
            } else {
                // Gộp món: Duyệt danh sách Items
                foreach (var item in sourceOrder.Items.ToList()) {
                    // Tìm món trùng dựa trên MenuItemId (thay vì MenuId)
                    var duplicate = targetOrder.Items
                        .FirstOrDefault(i => i.MenuItemId == item.MenuItemId && i.Price == item.Price);

                    if (duplicate != null) {
                        duplicate.Quantity += item.Quantity;
                        _context.OrderItems.Remove(item); // Xóa dòng cũ ở bàn nguồn
                    } else {
                        item.OrderId = targetOrder.Id; // Chuyển chủ sở hữu sang bàn đích
                    }
                }
                sourceOrder.Status = OrderStatus.CANCELLED;
            }

            var sourceTable = await _context.Tables.FindAsync(sourceTableId);
            if (sourceTable != null) sourceTable.Status = TableStatus.EMPTY;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        } catch {
            await transaction.RollbackAsync();
            throw;
        }
    }
}