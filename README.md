# 1. Tiêu đề dự án

Restaurant API (ASP.NET Core 8 Minimal API)

## 2. Giới thiệu

`Restaurant API` là dự án backend quản lý nhà hàng xây dựng bằng `ASP.NET Core 8 Minimal API`. Hệ thống mô phỏng các module nghiệp vụ chính: xác thực/phân quyền, quản lý tài khoản, bàn ăn, menu/combo, đơn hàng, hóa đơn/thanh toán, kho/nguyên liệu, khách hàng/voucher và đặt bàn.

Luôn xử lý tổng quát:

`Route -> Validator -> Service / DbContext -> ApiResponse`

## 3. Demo / Preview

- Swagger UI (khi chạy Development): `http://localhost:5126/swagger`
- API base URL (local): `http://localhost:5126`
- Docker Compose: API vẫn map ra `http://localhost:5126` (service `api` publish `5126:8080`)

## 4. Công nghệ sử dụng (Tech Stack)

- Language/Framework: `C#`, `.NET 8`, `ASP.NET Core Minimal API`
- ORM/DB access: `Entity Framework Core 8`, `Pomelo.EntityFrameworkCore.MySql`
- Database: `MySQL 8` (schema: [database/schema.sql](/C:/Learning/demoCCNLTHD/Restaurant_api/database/schema.sql))
- Auth: `JWT Bearer Authentication`, password hashing `BCrypt.Net`
- API docs: `Swagger / OpenAPI` (Swashbuckle)
- Runtime: `Docker`, `Docker Compose`

## 5. Cấu trúc thư mục

```text
Restaurant_api/
|-- database/
|   |-- schema.sql
|   `-- README.md
|-- RestaurantAPI/
|   |-- Properties/
|   |-- src/
|   |   |-- Contract/
|   |   |-- Data/
|   |   |-- Exceptions/
|   |   |-- Extensions/
|   |   |-- Modal/
|   |   |-- Route/
|   |   |-- Services/
|   |   `-- Validator/
|   |-- Program.cs
|   |-- appsettings.json
|   |-- appsettings.Development.json
|   `-- Dockerfile
|-- docker-compose.yml
`-- README.md
```

Ý nghĩa các thư mục trong `RestaurantAPI/src`:

- `Contract/`: chứa request/response DTO cho từng module
- `Data/`: chứa `AppDbContext` và cấu hình làm việc với EF Core
- `Exceptions/`: xử lý lỗi và middleware trả response lỗi thống nhất
- `Extensions/`: helper dùng chung cho response
- `Modal/`: entity và enum nghiệp vụ
- `Route/`: định nghĩa các endpoint Minimal API
- `Services/`: xử lý business logic
- `Validator/`: kiểm tra dữ liệu đầu vào trước khi xử lý

## 6. Cài đặt (Installation)

### Cách 1: Chạy local (.NET + MySQL)

Yêu cầu:

- `.NET SDK 8`
- `MySQL 8`

Bước 1: Tạo database và import schema:

```bash
mysql -u root -p < database/schema.sql
```

Bước 2: Kiểm tra config:

- `RestaurantAPI/appsettings.json`
- `RestaurantAPI/appsettings.Development.json`

Connection string mặc định:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=restaurant_management;User=root;Password=;Port=3306"
}
```

Bước 3: Chạy API:

```bash
cd RestaurantAPI
dotnet restore
dotnet run
```

### Cách 2: Chạy bằng Docker Compose

```bash
docker compose up --build
```

Sau khi chạy:

- API: `http://localhost:5126`
- Swagger: `http://localhost:5126/swagger`
- MySQL: `localhost:3306`

