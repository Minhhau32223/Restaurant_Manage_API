using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RestaurantAPI.src.Modal;
using RestaurantAPI.src.Modal.Enums;

namespace RestaurantAPI.src.Data
{
    public class AppDbContext:DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions) {
            
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Table> Tables { get; set; }

        public DbSet<Combo> Combos { get; set; }
        public DbSet<ComboItem> ComboItems { get; set; }

        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<MenuCategory> MenuCategories { get; set; }

        public DbSet<Recipe> Recipes { get; set; }

        public DbSet<Ingredients> Ingredients { get; set; }
        public DbSet<InventoryLogs> InventoryLogs { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerVoucher> CustomerVouchers { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes()) {
                foreach (var property in entityType.GetProperties()) {
                    if (property.ClrType.IsEnum) {
                        var converterType = typeof(EnumToStringConverter<>)
                            .MakeGenericType(property.ClrType);

                        var converter = (ValueConverter)Activator.CreateInstance(converterType)!;

                        property.SetValueConverter(converter);
                    }
                }
            }

            modelBuilder.Entity<ComboItem>()
                .HasKey(ci => new { ci.ComboId, ci.MenuItemId });

            modelBuilder.Entity<Recipe>()
                .HasKey(r => new { r.MenuItemId, r.IngredientId });

            // 1. Seed Accounts (Sửa Password -> PasswordHash, ép kiểu long 'L', dùng Enum Role)
            modelBuilder.Entity<Account>().HasData(
                new { Id = 1L, Username = "admin", PasswordHash = "$2a$11$1Ig2wqnOdBpqBuT.BAfsJe0Lxrte/gpsXvm6docROLgjldges2FXe", Role = Role.ADMIN, IsActive = true, CreatedAt = DateTime.Now },
                new { Id = 2L, Username = "staff", PasswordHash = "$2a$11$Jntx8qUE/6yFwqJnIOk7kO1jvIFjb6.MVr8sYfgB9GL/kmClusZsC", Role = Role.STAFF, IsActive = true, CreatedAt = DateTime.Now },
                new { Id = 3L, Username = "customer", PasswordHash = "$2a$11$lLfoCr9.x.gW.I43R3lTxutgMu6T8qJVAaID088sPAY2YWIo3sH6K", Role = Role.CUSTOMER, IsActive = true, CreatedAt = DateTime.Now }
            );

            // 2. Seed Customers
            modelBuilder.Entity<Customer>().HasData(
                new { Id = 1L, FullName = "Nguyễn Thị Mỹ Hạnh", Phone = "0346685730", Email = "ntmh@sv.sgu.edu.vn", Points = 1200, AccountId = 3L },
                new { Id = 2L, FullName = "Trần Công Hậu", Phone = "01669127737", Email = "tch@sv.ptit.edu.vn", Points = 80 },
                new { Id = 3L, FullName = "Nguyễn Tiến Phát", Phone = "0933681888", Email = "ntp@sv.hub.edu.vn", Points = 200 },
                new { Id = 4L, FullName = "Nguyễn Trọng Nghĩa", Phone = "0934567890", Email = "ntn@sv.nttu.edu.vn", Points = 50 },
                new { Id = 5L, FullName = "Nguyễn Minh Thiện", Phone = "0945678901", Email = "nmt@sv.hcmurne.edu.vn", Points = 150 }
            );

            // 3. Seed Tables (Dùng Enum TableStatus nếu có, nếu là String thì giữ nguyên)
            modelBuilder.Entity<Table>().HasData(
                new { Id = 1L, TableCode = "T01", SeatCount = 4, Status = TableStatus.EMPTY },
                new { Id = 2L, TableCode = "T02", SeatCount = 4, Status = TableStatus.EMPTY },
                new { Id = 3L, TableCode = "VIP-01", SeatCount = 6, Status = TableStatus.OCCUPIED },
                new { Id = 4L, TableCode = "VIP-02", SeatCount = 10, Status = TableStatus.RESERVED }
            );

            // 4. Seed Categories
            modelBuilder.Entity<MenuCategory>().HasData(
                new { Id = 1L, Name = "Món chính" },
                new { Id = 2L, Name = "Đồ uống" },
                new { Id = 3L, Name = "Tráng miệng" }
            );

            // 5. Seed MenuItems (Ép kiểu m cho decimal, L cho long, dùng Enum ItemStatus)
            modelBuilder.Entity<MenuItem>().HasData(
                new { Id = 1L, CategoryId = 1L, Name = "Cơm chiên hải sản", Price = 75000m, Description = "Cơm chiên với tôm, mực", Status = StatusMenuItem.AVAILABLE },
                new { Id = 2L, CategoryId = 1L, Name = "Bò lúc lắc", Price = 120000m, Description = "Bò xào mềm", Status = StatusMenuItem.AVAILABLE },
                new { Id = 3L, CategoryId = 2L, Name = "Trà đào", Price = 30000m, Description = "Trà đào mát lạnh", Status = StatusMenuItem.AVAILABLE },
                new { Id = 4L, CategoryId = 2L, Name = "Cà phê sữa", Price = 25000m, Description = "Cà phê truyền thống", Status = StatusMenuItem.AVAILABLE },
                new { Id = 5L, CategoryId = 3L, Name = "Bánh flan", Price = 20000m, Description = "Tráng miệng", Status = StatusMenuItem.AVAILABLE }
            );

            // 6. Seed Combos
            modelBuilder.Entity<Combo>().HasData(
                new { Id = 1L, Name = "Combo 2 người", Price = 180000m },
                new { Id = 2L, Name = "Combo gia đình", Price = 350000m }
            );

            // 7. Seed ComboItems (Khóa chính phức hợp cần chính xác kiểu long 'L')
            modelBuilder.Entity<ComboItem>().HasData(
                new { ComboId = 1L, MenuItemId = 1L, Quantity = 1 },
                new { ComboId = 1L, MenuItemId = 3L, Quantity = 2 },
                new { ComboId = 2L, MenuItemId = 1L, Quantity = 2 },
                new { ComboId = 2L, MenuItemId = 2L, Quantity = 1 },
                new { ComboId = 2L, MenuItemId = 3L, Quantity = 3 }
            );

            // 8. Seed Orders (Dùng Enum OrderStatus)
            modelBuilder.Entity<Order>().HasData(
                new { Id = 1L, TableId = 1L, AccountId = 1L, CustomerId = 1L, OrderTime = DateTime.Now, Status = OrderStatus.OPEN },
                new { Id = 2L, TableId = 2L, AccountId = 2L, CustomerId = 2L, OrderTime = DateTime.Now, Status = OrderStatus.PAID }
            );

            // 9. Seed OrderItems
            modelBuilder.Entity<OrderItem>().HasData(
                new { Id = 1L, OrderId = 1L, MenuItemId = 1L, Quantity = 2, Price = 75000m },
                new { Id = 2L, OrderId = 1L, MenuItemId = 3L, Quantity = 1, Price = 30000m },
                new { Id = 3L, OrderId = 2L, MenuItemId = 2L, Quantity = 1, Price = 120000m }
            );

            // 10. Seed Invoices
            modelBuilder.Entity<Invoice>().HasData(
                new { Id = 1L, OrderId = 2L, Subtotal = 120000m, Discount = 0m, Vat = 12000m, ServiceFee = 5000m, Total = 137000m, CreatedAt = DateTime.Now }
            );

            // 11. Seed Payments (Dùng Enum PaymentMethod và PaymentStatus)
            modelBuilder.Entity<Payment>().HasData(
                new { Id = 1L, InvoiceId = 1L, Method = PaymentMethod.CASH, Amount = 137000m, Status = PaymentStatus.SUCCESS, PaymentTime = DateTime.Now }
            );

            // 12. Seed Ingredients (Sửa decimal 'm' cho StockQuantity và MinQuantity)
            modelBuilder.Entity<Ingredients>().HasData(
                new { Id = 1L, Name = "Gạo", Unit = "kg", StockQuantity = 1000m, MinQuantity = 10m },
                new { Id = 2L, Name = "Thịt bò", Unit = "kg", StockQuantity = 1000m, MinQuantity = 5m },
                new { Id = 3L, Name = "Tôm", Unit = "kg", StockQuantity = 1000m, MinQuantity = 5m },
                new { Id = 4L, Name = "Trà", Unit = "g", StockQuantity = 1000m, MinQuantity = 200m }
            );

            // 13. Seed InventoryLogs (Dùng Enum LogType và decimal 'm')
            modelBuilder.Entity<InventoryLogs>().HasData(
                new { Id = 1L, IngredientId = 1L, LogType = InventoryLogType.IN, Quantity = 50m, CreatedAt = DateTime.Now },
                new { Id = 2L, IngredientId = 2L, LogType = InventoryLogType.IN, Quantity = 20m, CreatedAt = DateTime.Now },
                new { Id = 3L, IngredientId = 3L, LogType = InventoryLogType.OUT, Quantity = 5m, CreatedAt = DateTime.Now }
            );

            // 14. Seed Recipes (decimal 'm')
            modelBuilder.Entity<Recipe>().HasData(
                new { MenuItemId = 1L, IngredientId = 1L, Quantity = 5m },
                new { MenuItemId = 1L, IngredientId = 3L, Quantity = 5m },
                new { MenuItemId = 2L, IngredientId = 2L, Quantity = 5m }
            );

            // 15. Seed Reservations (Dùng Enum ReservationStatus)
            modelBuilder.Entity<Reservation>().HasData(
                new { Id = 1L, CustomerId = 1L, TableId = 4L, ReservationTime = DateTime.Now.AddDays(1), GuestCount = 5, Status = ReservationStatus.CONFIRMED },
                new { Id = 2L, CustomerId = 2L, TableId = 2L, ReservationTime = DateTime.Now.AddDays(2), GuestCount = 2, Status = ReservationStatus.PENDING }
            );

            // 16. Seed Vouchers (decimal 'm')
            modelBuilder.Entity<CustomerVoucher>().HasData(
                new { Id = 1L, CustomerId = 1L, Code = "DISCOUNT10", Discount = 10000m, ExpiryDate = new DateTime(2026, 12, 31), IsUsed = false },
                new { Id = 2L, CustomerId = 2L, Code = "VIP50", Discount = 50000m, ExpiryDate = new DateTime(2026, 6, 30), IsUsed = false }
            );
        }
    }
}
