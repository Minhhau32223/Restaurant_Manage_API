using Microsoft.EntityFrameworkCore;
using RestaurantAPI.src.Contract.Pricing.Request;
using RestaurantAPI.src.Contract.Pricing.Response;
using RestaurantAPI.src.Data;
using RestaurantAPI.src.Exceptions;
using RestaurantAPI.src.Services.Interfaces;

namespace RestaurantAPI.src.Services {
    public class PricingService : IPricingService {
        private readonly AppDbContext _context;

        private const decimal DEFAULT_VAT = 0.1m;
        private const decimal DEFAULT_SERVICE = 0.05m;

        public PricingService(AppDbContext context) {
            _context = context;
        }

        // 🔥 PAYMENT (DB - AN TOÀN)
        public async Task<PricingResponse> CalculateAsync(PricingRequest request) {
            var subtotal = await _context.OrderItems
                .Where(i => i.OrderId == request.OrderId)
                .SumAsync(i => i.Price * i.Quantity);

            if (subtotal <= 0)
                throw ApiException.BadRequest("Order chưa có món");

            return await BuildPricing(subtotal, request.VoucherCode, request.CustomerId, DEFAULT_VAT, DEFAULT_SERVICE);
        }

        // 🔥 PREVIEW (CLIENT)
        public async Task<PricingResponse> PreviewAsync(PricingPreviewRequest request) {
            if (request.Items == null || !request.Items.Any())
                throw ApiException.BadRequest("Danh sách món trống");

            var subtotal = request.Items
                .Sum(i => i.Price * i.Quantity);

            return await BuildPricing(
                subtotal,
                request.VoucherCode,
                request.CustomerId,
                request.VatRate,
                request.ServiceRate
            );
        }

        // 🔥 APPLY VOUCHER
        private async Task<decimal> ApplyVoucher(string? code, long? customerId) {
            if (string.IsNullOrWhiteSpace(code))
                return 0m;

            if (customerId == null)
                throw ApiException.BadRequest("ID khách hàng không được để trống");

            var voucher = await _context.CustomerVouchers
                .FirstOrDefaultAsync(v =>
                    v.Code == code &&
                    v.CustomerId == customerId);

            if (voucher == null)
                throw new NotFoundException("Voucher không tồn tại");

            if (voucher.ExpiryDate < DateTime.UtcNow)
                throw ApiException.BadRequest("Voucher đã hết hạn");

            if (voucher.IsUsed)
                throw ApiException.BadRequest("Voucher đã được sử dụng");

            return voucher.Discount;
        }

        // 🔥 CORE LOGIC
        private async Task<PricingResponse> BuildPricing(decimal subtotal, string? voucherCode, long? customerId, decimal? vatRate, decimal? serviceRate) {
            var discount = await ApplyVoucher(voucherCode, customerId);

            var baseAmount = subtotal - discount;

            if (baseAmount < 0)
                baseAmount = 0;

            var vat = baseAmount * (vatRate ?? DEFAULT_VAT);
            var serviceFee = baseAmount * (serviceRate ?? DEFAULT_SERVICE);

            var total = baseAmount + vat + serviceFee;

            return new PricingResponse {
                Subtotal = subtotal,
                Discount = discount,
                Vat = vat,
                ServiceFee = serviceFee,
                Total = total
            };
        }
    }
}