using Microsoft.EntityFrameworkCore;
using RestaurantAPI.src.Contract.Invoice.Response;
using RestaurantAPI.src.Contract.Pricing.Request;
using RestaurantAPI.src.Data;
using RestaurantAPI.src.Exceptions;
using RestaurantAPI.src.Services.Interfaces;

namespace RestaurantAPI.src.Services {
    public class InvoiceService : IInvoiceService {
        private readonly AppDbContext _context;
        private readonly IPricingService _pricingService;

        public InvoiceService(AppDbContext context, IPricingService pricingService) {
            _context = context;
            _pricingService = pricingService;
        }

        public async Task<InvoiceDetailResponse> GetByOrderId(long orderId) {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) throw ApiException.NotFound("Order không tồn tại");

            // 1. Lấy danh sách món thực tế từ OrderItems
            var items = await _context.OrderItems
                .Where(i => i.OrderId == orderId)
                .Include(i => i.MenuItem)
                .Select(i => new InvoiceItemResponse {
                    Name = i.MenuItem.Name,
                    Quantity = i.Quantity,
                    Price = i.Price
                })
                .ToListAsync();

            if (!items.Any()) throw ApiException.BadRequest("Order chưa có món");

            // 2. 🔥 SỬA: Dùng PricingService để tính tiền thay vì tự tính tay
            // Điều này đảm bảo Voucher, VAT, ServiceFee luôn chuẩn xác
            var pricing = await _pricingService.CalculateAsync(new PricingRequest {
                OrderId = orderId,
                VoucherCode = null, // Preview chưa có voucher hoặc lấy từ Order nếu bạn lưu Code vào Order
                CustomerId = order.CustomerId
            });

            return new InvoiceDetailResponse {
                InvoiceId = 0,
                OrderId = orderId,
                Subtotal = pricing.Subtotal,
                Vat = pricing.Vat,
                ServiceFee = pricing.ServiceFee,
                Discount = pricing.Discount,
                Total = pricing.Total,
                CreatedAt = DateTime.UtcNow,
                Items = items
            };
        }

        public async Task<InvoiceDetailResponse> GetById(long invoiceId) {
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(x => x.Id == invoiceId)
                ?? throw ApiException.NotFound("Không tìm thấy hóa đơn");

            var items = await _context.OrderItems
                .Where(i => i.OrderId == invoice.OrderId)
                .Include(i => i.MenuItem)
                .Select(i => new InvoiceItemResponse {
                    Name = i.MenuItem.Name,
                    Quantity = i.Quantity,
                    Price = i.Price
                })
                .ToListAsync();

            return new InvoiceDetailResponse {
                InvoiceId = invoice.Id,
                OrderId = invoice.OrderId,
                Subtotal = invoice.Subtotal,
                Vat = invoice.Vat,
                ServiceFee = invoice.ServiceFee,
                Discount = invoice.Discount,
                Total = invoice.Total,
                CreatedAt = invoice.CreatedAt,
                Items = items
            };
        }
    }
}