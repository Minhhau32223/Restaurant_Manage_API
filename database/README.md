# Database - Restaurant Management

## Kết nối từ RestaurantAPI

Đây là database mà **RestaurantAPI** sử dụng.

- **Connection string**: `appsettings.json` → `ConnectionStrings:DefaultConnection`
- **Database**: `restaurant_management`
- **Ví dụ**: `Server=localhost;Database=restaurant_management;User=root;Password=;Port=3306`

## Cài đặt

1. Đảm bảo MySQL đang chạy
2. Chạy script schema:
   ```bash
   mysql -u root -p < database/schema.sql
   ```
   Hoặc mở `schema.sql` trong MySQL Workbench và Execute

## Tài khoản mẫu (seed)

Script đã chèn 3 tài khoản với **password: 123456** (đã hash BCrypt):

| Username | Password | Role |
|----------|----------|------|
| Minhhau | 123456   | ADMIN |



**Nếu đăng nhập thất bại**: Hash có thể không khớp. Tạo user mới qua API:
1. Chạy API
2. Swagger → POST `/api/account` (cần JWT - tạm thêm `[AllowAnonymous]` ở `AccountRoute` để tạo user đầu tiên)
3. Hoặc dùng tool tạo BCrypt hash rồi INSERT thủ công

## Cấu trúc bảng

| Bảng | Mô tả |
|------|-------|
| accounts | Tài khoản đăng nhập |
| customers | Khách hàng |
| tables | Bàn ăn |
| menu_categories | Danh mục món |
| menu_items | Món ăn/đồ uống |
| combos, combo_items | Combo món |
| orders, order_items | Đơn hàng |
| invoices, payments | Hóa đơn, thanh toán |
| ingredients, inventory_logs | Nguyên liệu, tồn kho |
| recipes | Công thức món |
| reservations | Đặt bàn |
| customer_vouchers | Voucher khách |
