using Microsoft.EntityFrameworkCore;
using RestaurantAPI.src.Contract.Payment.Request;
using RestaurantAPI.src.Contract.Payment.Response;
using RestaurantAPI.src.Contract.Pricing.Request;
using RestaurantAPI.src.Data;
using RestaurantAPI.src.Exceptions;
using RestaurantAPI.src.Modal;
using RestaurantAPI.src.Modal.Enums;
using RestaurantAPI.src.Services.Interfaces;
using RestaurantAPI.src.Validator;

namespace RestaurantAPI.src.Services {
    public class PaymentService : IPaymentService {
        private readonly AppDbContext _context;
        private readonly ITableService _tableService;
        private readonly IPricingService _pricingService;
        private readonly IInventoryService _inventoryService; // Thêm service kho

        public PaymentService(
            AppDbContext context,
            ITableService tableService,
            IPricingService pricingService,
            IInventoryService inventoryService) {
            _context = context;
            _tableService = tableService;
            _pricingService = pricingService;
            _inventoryService = inventoryService;
        }

        public async Task<PaymentResponse> Pay(CreatePaymentRequest request) {
            PaymentValidator.Validate(request);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try {
                // 1. LOAD ORDER + ITEMS
                var order = await _context.Orders
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(x => x.Id == request.OrderId);

                if (order == null) throw ApiException.NotFound("Order không tồn tại");
                if (order.Status != OrderStatus.OPEN) throw ApiException.BadRequest("Order này không ở trạng thái có thể thanh toán");
                if (!order.Items.Any()) throw ApiException.BadRequest("Order chưa có món ăn nào");

                // 2. TÍNH TOÁN GIÁ (PRICING)
                var pricing = await _pricingService.CalculateAsync(new PricingRequest {
                    OrderId = order.Id,
                    VoucherCode = request.VoucherCode,
                    CustomerId = request.CustomerId
                });

                // 3. TẠO HÓA ĐƠN (INVOICE)
                var invoice = new Invoice {
                    OrderId = order.Id,
                    Subtotal = pricing.Subtotal,
                    Vat = pricing.Vat,
                    ServiceFee = pricing.ServiceFee,
                    Discount = pricing.Discount,
                    Total = pricing.Total,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();

                // 4. TẠO THANH TOÁN (PAYMENT)
                var payment = new Payment {
                    InvoiceId = invoice.Id,
                    Method = request.Method,
                    Amount = pricing.Total,
                    Status = PaymentStatus.SUCCESS,
                    PaymentTime = DateTime.UtcNow
                };
                _context.Payments.Add(payment);

                // 5. CẬP NHẬT TRẠNG THÁI ORDER SANG PAID
                order.Status = OrderStatus.PAID;

                // 6. QUAN TRỌNG: KHẤU TRỪ KHO THỰC TẾ
                // Hàm này sẽ trừ StockQuantity và ghi InventoryLogs dựa trên định lượng
                var deductSuccess = await _inventoryService.DeductStockFromOrderAsync(order.Id);
                if (!deductSuccess) throw ApiException.InternalServerError("Lỗi khi khấu trừ kho nguyên liệu");

                // 7. GIẢI PHÓNG BÀN
                await _tableService.SetEmpty(order.TableId);

                // 8. XỬ LÝ VOUCHER (NẾU CÓ)
                if (!string.IsNullOrEmpty(request.VoucherCode)) {
                    var voucher = await _context.CustomerVouchers
                        .FirstOrDefaultAsync(v => v.Code == request.VoucherCode && v.CustomerId == request.CustomerId);
                    if (voucher != null) voucher.IsUsed = true;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new PaymentResponse {
                    InvoiceId = invoice.Id,
                    Total = pricing.Total,
                    Method = payment.Method.ToString(),
                    PaymentTime = payment.PaymentTime
                };
            } catch (Exception ex) {
                await transaction.RollbackAsync();
                if (ex is ApiException) throw;
                throw ApiException.InternalServerError($"Lỗi thanh toán: {ex.Message}");
            }
        }
    }
}