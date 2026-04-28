-- ============================================================================
-- RESTAURANT MANAGEMENT - DATABASE SCHEMA
-- ============================================================================
-- Đây là database mà RestaurantAPI sẽ kết nối.
-- Connection string trong appsettings.json: "DefaultConnection"
-- Database: restaurant_management
-- ============================================================================

-- Xóa database cũ (cẩn thận: mất toàn bộ dữ liệu)
DROP DATABASE IF EXISTS restaurant_management;

-- Tạo database mới với UTF-8 (hỗ trợ tiếng Việt)
CREATE DATABASE restaurant_management
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE restaurant_management;

-- ============================================================================
-- 1. ACCOUNTS - Tài khoản đăng nhập (nhân viên, quản lý)
-- ============================================================================
-- Bảng này được AuthController và AccountController sử dụng.
-- Cột password lưu BCrypt hash (không lưu plain text).
-- ============================================================================
CREATE TABLE accounts (
    id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT 'PK, tự tăng',
    username VARCHAR(50) NOT NULL UNIQUE COMMENT 'Tên đăng nhập, không trùng',
    password VARCHAR(255) NOT NULL COMMENT 'BCrypt hash - không lưu plain text',
    role ENUM('ADMIN','STAFF','CUSTOMER') NOT NULL DEFAULT 'CUSTOMER' COMMENT 'ADMIN=Quản lý, STAFF=Nhân viên',
    is_active BOOLEAN DEFAULT TRUE COMMENT 'TRUE=Hoạt động, FALSE=Khóa',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời gian tạo'
) COMMENT 'Tài khoản đăng nhập hệ thống';

-- ============================================================================
-- 2. CUSTOMERS - Khách hàng
-- ============================================================================
CREATE TABLE customers (
    id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT 'PK',
    full_name VARCHAR(150) COMMENT 'Họ tên',
    phone VARCHAR(20) UNIQUE COMMENT 'SĐT (dùng tra cứu)',
    email VARCHAR(100) COMMENT 'Email',
    points INT DEFAULT 0 COMMENT 'Điểm tích lũy',
    account_id BIGINT COMMENT 'Liên kết với tài khoản'
) COMMENT 'Thông tin khách hàng';

-- ============================================================================
-- 3. TABLES - Bàn ăn
-- ============================================================================
CREATE TABLE tables (
    id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT 'PK',
    table_code VARCHAR(50) NOT NULL UNIQUE COMMENT 'Mã bàn (VD: T01, VIP-1)',
    seat_count INT NOT NULL COMMENT 'Số chỗ ngồi',
    status ENUM('EMPTY','OCCUPIED','RESERVED') DEFAULT 'EMPTY' COMMENT 'Trống/Đang dùng/Đặt trước'
) COMMENT 'Bàn trong nhà hàng';

-- ============================================================================
-- 4. MENU - Thực đơn
-- ============================================================================
CREATE TABLE menu_categories (
    id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT 'PK',
    name VARCHAR(100) NOT NULL UNIQUE COMMENT 'Tên danh mục (Món chính, Đồ uống...)'
) COMMENT 'Danh mục món ăn';

CREATE TABLE menu_items (
    id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT 'PK',
    category_id BIGINT NOT NULL COMMENT 'FK -> menu_categories',
    name VARCHAR(150) NOT NULL COMMENT 'Tên món',
    price DECIMAL(12,2) NOT NULL COMMENT 'Giá bán',
    image_url VARCHAR(255) COMMENT 'URL hình ảnh',
    description TEXT COMMENT 'Mô tả món',
    status ENUM('AVAILABLE','OUT_OF_STOCK') DEFAULT 'AVAILABLE' COMMENT 'Còn hàng/Hết hàng',
    CONSTRAINT fk_menu_category FOREIGN KEY (category_id) REFERENCES menu_categories(id)
) COMMENT 'Món ăn/đồ uống';

-- ============================================================================
-- 5. COMBO / SET MENU - Combo món
-- ============================================================================
CREATE TABLE combos (
    id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT 'PK',
    name VARCHAR(150) NOT NULL COMMENT 'Tên combo',
    price DECIMAL(12,2) NOT NULL COMMENT 'Giá combo'
) COMMENT 'Combo/set món';

CREATE TABLE combo_items (
    combo_id BIGINT NOT NULL COMMENT 'FK -> combos',
    menu_item_id BIGINT NOT NULL COMMENT 'FK -> menu_items',
    quantity INT NOT NULL COMMENT 'Số lượng món trong combo',
    PRIMARY KEY (combo_id, menu_item_id),
    FOREIGN KEY (combo_id) REFERENCES combos(id),
    FOREIGN KEY (menu_item_id) REFERENCES menu_items(id)
) COMMENT 'Chi tiết món trong combo';

-- ============================================================================
-- 6. ORDERS - Đơn hàng
-- ============================================================================
CREATE TABLE orders (
    id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT 'PK',
    table_id BIGINT NOT NULL COMMENT 'FK -> tables',
    account_id BIGINT NOT NULL COMMENT 'FK -> accounts (nhân viên tạo đơn)',
    customer_id BIGINT NULL COMMENT 'FK -> customers (nếu có KH)',
    order_time DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời gian tạo đơn',
    status ENUM('OPEN','PAID', 'COMPLETED', 'CANCELLED') DEFAULT 'OPEN' COMMENT 'Mở/Đã thanh toán/Hủy',
    FOREIGN KEY (table_id) REFERENCES tables(id),
    FOREIGN KEY (account_id) REFERENCES accounts(id),
    FOREIGN KEY (customer_id) REFERENCES customers(id)
) COMMENT 'Đơn hàng';

CREATE TABLE order_items (
    id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT 'PK',
    order_id BIGINT NOT NULL COMMENT 'FK -> orders',
    menu_item_id BIGINT NOT NULL COMMENT 'FK -> menu_items',
    quantity INT NOT NULL COMMENT 'Số lượng',
    price DECIMAL(12,2) NOT NULL COMMENT 'Giá tại thời điểm order',
    FOREIGN KEY (order_id) REFERENCES orders(id),
    FOREIGN KEY (menu_item_id) REFERENCES menu_items(id)
) COMMENT 'Chi tiết món trong đơn';

-- ============================================================================
-- 7. INVOICE & PAYMENT - Hóa đơn và thanh toán
-- ============================================================================
CREATE TABLE invoices (
    id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT 'PK',
    order_id BIGINT NOT NULL UNIQUE COMMENT 'FK -> orders (1-1)',
    subtotal DECIMAL(12,2) NOT NULL COMMENT 'Tổng tiền món',
    discount DECIMAL(12,2) DEFAULT 0 COMMENT 'Giảm giá',
    vat DECIMAL(12,2) DEFAULT 0 COMMENT 'Thuế VAT',
    service_fee DECIMAL(12,2) DEFAULT 0 COMMENT 'Phí phục vụ',
    total DECIMAL(12,2) NOT NULL COMMENT 'Tổng thanh toán',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (order_id) REFERENCES orders(id)
) COMMENT 'Hóa đơn';

CREATE TABLE payments (
    id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT 'PK',
    invoice_id BIGINT NOT NULL COMMENT 'FK -> invoices',
    method ENUM('CASH','QR','TRANSFER') NOT NULL COMMENT 'Tiền mặt/QR/Chuyển khoản',
    amount DECIMAL(12,2) NOT NULL COMMENT 'Số tiền',
    status ENUM('SUCCESS','FAILED','PENDING') DEFAULT 'SUCCESS',
    payment_time DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (invoice_id) REFERENCES invoices(id)
) COMMENT 'Thanh toán';

-- ============================================================================
-- 8. INGREDIENTS & INVENTORY - Nguyên liệu và tồn kho
-- ============================================================================
CREATE TABLE ingredients (
    id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT 'PK',
    name VARCHAR(150) NOT NULL UNIQUE COMMENT 'Tên nguyên liệu',
    unit VARCHAR(50) NOT NULL COMMENT 'Đơn vị (kg, g, lít...)',
    stock_quantity DECIMAL(10,2) NOT NULL DEFAULT 0 COMMENT 'Tồn kho hiện tại',
    min_quantity DECIMAL(10,2) DEFAULT 0 COMMENT 'Mức tối thiểu (cảnh báo)'
) COMMENT 'Nguyên liệu';

CREATE TABLE inventory_logs (
    id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT 'PK',
    ingredient_id BIGINT NOT NULL COMMENT 'FK -> ingredients',
    log_type ENUM('IN','OUT') NOT NULL COMMENT 'Nhập/Xuất kho',
    quantity DECIMAL(10,2) NOT NULL COMMENT 'Số lượng',
    expiry_date DATE COMMENT 'Hạn sử dụng',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ingredient_id) REFERENCES ingredients(id)
) COMMENT 'Nhật ký nhập/xuất kho';

-- ============================================================================
-- 9. RECIPES - Công thức (món cần nguyên liệu gì)
-- ============================================================================
CREATE TABLE recipes (
    menu_item_id BIGINT NOT NULL COMMENT 'FK -> menu_items',
    ingredient_id BIGINT NOT NULL COMMENT 'FK -> ingredients',
    quantity DECIMAL(10,2) NOT NULL COMMENT 'Số lượng nguyên liệu cho 1 phần',
    PRIMARY KEY (menu_item_id, ingredient_id),
    FOREIGN KEY (menu_item_id) REFERENCES menu_items(id),
    FOREIGN KEY (ingredient_id) REFERENCES ingredients(id)
) COMMENT 'Công thức món ăn';

-- ============================================================================
-- 10. RESERVATIONS - Đặt bàn
-- ============================================================================
CREATE TABLE reservations (
    id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT 'PK',
    customer_id BIGINT NOT NULL COMMENT 'FK -> customers',
    table_id BIGINT NOT NULL COMMENT 'FK -> tables',
    reservation_time DATETIME NOT NULL COMMENT 'Thời gian đặt',
    guest_count INT NOT NULL COMMENT 'Số khách',
    status ENUM('PENDING','CONFIRMED','CANCELLED') DEFAULT 'PENDING',
    FOREIGN KEY (customer_id) REFERENCES customers(id),
    FOREIGN KEY (table_id) REFERENCES tables(id)
) COMMENT 'Đặt bàn trước';

-- ============================================================================
-- 11. CUSTOMER VOUCHERS - Phiếu giảm giá của khách
-- ============================================================================
CREATE TABLE customer_vouchers (
    id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT 'PK',
    customer_id BIGINT NOT NULL COMMENT 'FK -> customers',
    code VARCHAR(50) NOT NULL UNIQUE COMMENT 'Mã voucher',
    discount DECIMAL(10,2) NOT NULL COMMENT 'Giá trị giảm',
    expiry_date DATE NOT NULL COMMENT 'Hạn dùng',
    is_used BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (customer_id) REFERENCES customers(id)
) COMMENT 'Voucher khách hàng';

-- ============================================================================
-- SEED DATA - Dữ liệu mẫu cho accounts
-- ============================================================================
-- LƯU Ý: API dùng BCrypt hash - KHÔNG lưu password dạng plain text.
--
-- Cách 1 (khuyên dùng): Chạy API -> Swagger -> POST /api/auth/login với user đã tạo
--                      trước tiên tạo user: POST /api/account (cần JWT - đăng nhập admin trước)
--                      HOẶC tạm bỏ [Authorize] ở AccountController.Create để tạo user đầu tiên.
--
-- Cách 2: Chạy INSERT dưới đây - hash BCrypt cho password "123456"
--         Nếu đăng nhập thất bại: xóa INSERT, chạy API, dùng Swagger POST /api/account
--         để tạo user (password tự hash).
-- ============================================================================

-- BCrypt hash cho "123456" (cost 10)
INSERT INTO accounts (username, password, role, is_active, created_at)
VALUES
    ('admin', '$2a$11$1Ig2wqnOdBpqBuT.BAfsJe0Lxrte/gpsXvm6docROLgjldges2FXe', 'ADMIN', TRUE, NOW()), -- admin123
    ('staff', '$2a$11$Jntx8qUE/6yFwqJnIOk7kO1jvIFjb6.MVr8sYfgB9GL/kmClusZsC', 'STAFF', TRUE, NOW()), -- staff123
	 ('customer', '$2a$11$lLfoCr9.x.gW.I43R3lTxutgMu6T8qJVAaID088sPAY2YWIo3sH6K', 'CUSTOMER', TRUE, NOW()); -- customer123
   
INSERT INTO customers (full_name, phone, email, points) VALUES
('Nguyễn Thị Mỹ Hạnh', '0346685730', 'ntmh@sv.sgu.edu.vn', 1200),
('Trần Công Hậu', '01669127737', 'tch@sv.ptit.edu.vn', 80),
('Nguyễn Tiến Phát', '0933681888', 'ntp@sv.hub.edu.vn', 200),
('Nguyễn Trọng Nghĩa', '0934567890', 'ntn@sv.nttu.edu.vn', 50),
('Nguyễn Minh Thiện', '0945678901', 'nmt@sv.hcmurne.edu.vn', 150);

INSERT INTO tables (table_code, seat_count, status) VALUES
('T01', 4, 'EMPTY'),
('T02', 4, 'EMPTY'),
('T03', 6, 'OCCUPIED'),
('VIP-1', 10, 'RESERVED');

INSERT INTO menu_categories (name) VALUES
('Món chính'),
('Đồ uống'),
('Tráng miệng');

INSERT INTO menu_items (category_id, name, price, image_url, description, status) VALUES
(1, 'Cơm chiên hải sản', 75000, NULL, 'Cơm chiên với tôm, mực', 'AVAILABLE'),
(1, 'Bò lúc lắc', 120000, NULL, 'Bò xào mềm', 'AVAILABLE'),
(2, 'Trà đào', 30000, NULL, 'Trà đào mát lạnh', 'AVAILABLE'),
(2, 'Cà phê sữa', 25000, NULL, 'Cà phê truyền thống', 'AVAILABLE'),
(3, 'Bánh flan', 20000, NULL, 'Tráng miệng', 'AVAILABLE');

INSERT INTO combos (name, price) VALUES
('Combo 2 người', 180000),
('Combo gia đình', 350000);

INSERT INTO combo_items (combo_id, menu_item_id, quantity) VALUES
(1, 1, 1),
(1, 3, 2),
(2, 1, 2),
(2, 2, 1),
(2, 3, 3);

INSERT INTO orders (table_id, account_id, customer_id, status) VALUES
(1, 1, 1, 'OPEN'),
(2, 2, 2, 'PAID');

INSERT INTO order_items (order_id, menu_item_id, quantity, price) VALUES
(1, 1, 2, 75000),
(1, 3, 1, 30000),
(2, 2, 1, 120000);

INSERT INTO invoices (order_id, subtotal, discount, vat, service_fee, total) VALUES
(2, 120000, 0, 12000, 5000, 137000);

INSERT INTO payments (invoice_id, method, amount, status) VALUES
(1, 'CASH', 137000, 'SUCCESS');

INSERT INTO ingredients (name, unit, stock_quantity, min_quantity) VALUES
('Gạo', 'kg', 1000, 10),
('Thịt bò', 'kg', 1000, 5),
('Tôm', 'kg', 1000, 5),
('Trà', 'g', 1000, 200);

INSERT INTO inventory_logs (ingredient_id, log_type, quantity) VALUES
(1, 'IN', 50),
(2, 'IN', 20),
(3, 'OUT', 5);

INSERT INTO recipes (menu_item_id, ingredient_id, quantity) VALUES
(1, 1, 5),
(1, 3, 5),
(2, 2, 5);

INSERT INTO reservations (customer_id, table_id, reservation_time, guest_count, status) VALUES
(1, 4, NOW() + INTERVAL 1 DAY, 5, 'CONFIRMED'),
(2, 2, NOW() + INTERVAL 2 DAY, 2, 'PENDING');

INSERT INTO customer_vouchers (customer_id, code, discount, expiry_date, is_used) VALUES
(1, 'DISCOUNT10', 10000, '2026-12-31', false),
(2, 'VIP50', 50000, '2026-06-30', false);

