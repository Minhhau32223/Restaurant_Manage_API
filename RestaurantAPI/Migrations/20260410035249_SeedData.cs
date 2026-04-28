using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RestaurantAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    username = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    role = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "combos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_combos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ingredients",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Unit = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    stock_quantity = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    min_quantity = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingredients", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "menu_categories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu_categories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    table_id = table.Column<long>(type: "bigint", nullable: false),
                    account_id = table.Column<long>(type: "bigint", nullable: false),
                    customer_id = table.Column<long>(type: "bigint", nullable: true),
                    order_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    invoice_id = table.Column<long>(type: "bigint", nullable: false),
                    method = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    payment_time = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tables",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    table_code = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    seat_count = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    full_name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    points = table.Column<int>(type: "int", nullable: false),
                    account_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_customers_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "inventory_logs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ingredient_id = table.Column<long>(type: "bigint", nullable: false),
                    log_type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    expiry_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_inventory_logs_ingredients_ingredient_id",
                        column: x => x.ingredient_id,
                        principalTable: "ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "menu_items",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    category_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    image_url = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_menu_items_menu_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "menu_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "invoices",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    order_id = table.Column<long>(type: "bigint", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    discount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    vat = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    service_fee = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    total = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoices", x => x.id);
                    table.ForeignKey(
                        name: "FK_invoices_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "customer_vouchers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    customer_id = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    discount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    is_used = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_vouchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_customer_vouchers_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "reservations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    customer_id = table.Column<long>(type: "bigint", nullable: false),
                    table_id = table.Column<long>(type: "bigint", nullable: false),
                    reservation_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    guest_count = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_reservations_Tables_table_id",
                        column: x => x.table_id,
                        principalTable: "Tables",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reservations_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "combo_items",
                columns: table => new
                {
                    combo_id = table.Column<long>(type: "bigint", nullable: false),
                    menu_item_id = table.Column<long>(type: "bigint", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_combo_items", x => new { x.combo_id, x.menu_item_id });
                    table.ForeignKey(
                        name: "FK_combo_items_combos_combo_id",
                        column: x => x.combo_id,
                        principalTable: "combos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_combo_items_menu_items_menu_item_id",
                        column: x => x.menu_item_id,
                        principalTable: "menu_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "order_items",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    order_id = table.Column<long>(type: "bigint", nullable: false),
                    menu_item_id = table.Column<long>(type: "bigint", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_order_items_menu_items_menu_item_id",
                        column: x => x.menu_item_id,
                        principalTable: "menu_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_items_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "recipes",
                columns: table => new
                {
                    menu_item_id = table.Column<long>(type: "bigint", nullable: false),
                    ingredient_id = table.Column<long>(type: "bigint", nullable: false),
                    quantity = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipes", x => new { x.menu_item_id, x.ingredient_id });
                    table.ForeignKey(
                        name: "FK_recipes_ingredients_ingredient_id",
                        column: x => x.ingredient_id,
                        principalTable: "ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_recipes_menu_items_menu_item_id",
                        column: x => x.menu_item_id,
                        principalTable: "menu_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Tables",
                columns: new[] { "id", "seat_count", "status", "table_code" },
                values: new object[,]
                {
                    { 1L, 4, "EMPTY", "T01" },
                    { 2L, 4, "EMPTY", "T02" },
                    { 3L, 6, "OCCUPIED", "VIP-01" },
                    { 4L, 10, "RESERVED", "VIP-02" }
                });

            migrationBuilder.InsertData(
                table: "accounts",
                columns: new[] { "id", "created_at", "is_active", "password", "role", "username" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 4, 10, 10, 52, 47, 128, DateTimeKind.Local).AddTicks(4521), true, "$2a$11$1Ig2wqnOdBpqBuT.BAfsJe0Lxrte/gpsXvm6docROLgjldges2FXe", "ADMIN", "admin" },
                    { 2L, new DateTime(2026, 4, 10, 10, 52, 47, 128, DateTimeKind.Local).AddTicks(4538), true, "$2a$11$Jntx8qUE/6yFwqJnIOk7kO1jvIFjb6.MVr8sYfgB9GL/kmClusZsC", "STAFF", "staff" },
                    { 3L, new DateTime(2026, 4, 10, 10, 52, 47, 128, DateTimeKind.Local).AddTicks(4539), true, "$2a$11$lLfoCr9.x.gW.I43R3lTxutgMu6T8qJVAaID088sPAY2YWIo3sH6K", "CUSTOMER", "customer" }
                });

            migrationBuilder.InsertData(
                table: "combos",
                columns: new[] { "Id", "name", "price" },
                values: new object[,]
                {
                    { 1L, "Combo 2 người", 180000m },
                    { 2L, "Combo gia đình", 350000m }
                });

            migrationBuilder.InsertData(
                table: "customers",
                columns: new[] { "Id", "account_id", "email", "full_name", "phone", "points" },
                values: new object[,]
                {
                    { 2L, null, "tch@sv.ptit.edu.vn", "Trần Công Hậu", "01669127737", 80 },
                    { 3L, null, "ntp@sv.hub.edu.vn", "Nguyễn Tiến Phát", "0933681888", 200 },
                    { 4L, null, "ntn@sv.nttu.edu.vn", "Nguyễn Trọng Nghĩa", "0934567890", 50 },
                    { 5L, null, "nmt@sv.hcmurne.edu.vn", "Nguyễn Minh Thiện", "0945678901", 150 }
                });

            migrationBuilder.InsertData(
                table: "ingredients",
                columns: new[] { "Id", "min_quantity", "Name", "stock_quantity", "Unit" },
                values: new object[,]
                {
                    { 1L, 10m, "Gạo", 1000m, "kg" },
                    { 2L, 5m, "Thịt bò", 1000m, "kg" },
                    { 3L, 5m, "Tôm", 1000m, "kg" },
                    { 4L, 200m, "Trà", 1000m, "g" }
                });

            migrationBuilder.InsertData(
                table: "menu_categories",
                columns: new[] { "Id", "name" },
                values: new object[,]
                {
                    { 1L, "Món chính" },
                    { 2L, "Đồ uống" },
                    { 3L, "Tráng miệng" }
                });

            migrationBuilder.InsertData(
                table: "orders",
                columns: new[] { "id", "account_id", "customer_id", "order_time", "Status", "table_id" },
                values: new object[,]
                {
                    { 1L, 1L, 1L, new DateTime(2026, 4, 10, 10, 52, 47, 128, DateTimeKind.Local).AddTicks(4736), "OPEN", 1L },
                    { 2L, 2L, 2L, new DateTime(2026, 4, 10, 10, 52, 47, 128, DateTimeKind.Local).AddTicks(4738), "PAID", 2L }
                });

            migrationBuilder.InsertData(
                table: "payments",
                columns: new[] { "id", "amount", "invoice_id", "method", "payment_time", "status" },
                values: new object[] { 1L, 137000m, 1L, "CASH", new DateTime(2026, 4, 10, 10, 52, 47, 128, DateTimeKind.Local).AddTicks(4837), "SUCCESS" });

            migrationBuilder.InsertData(
                table: "customer_vouchers",
                columns: new[] { "Id", "Code", "customer_id", "discount", "expiry_date", "is_used" },
                values: new object[] { 2L, "VIP50", 2L, 50000m, new DateTime(2026, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), false });

            migrationBuilder.InsertData(
                table: "customers",
                columns: new[] { "Id", "account_id", "email", "full_name", "phone", "points" },
                values: new object[] { 1L, 3L, "ntmh@sv.sgu.edu.vn", "Nguyễn Thị Mỹ Hạnh", "0346685730", 1200 });

            migrationBuilder.InsertData(
                table: "inventory_logs",
                columns: new[] { "Id", "created_at", "expiry_date", "ingredient_id", "log_type", "Quantity" },
                values: new object[,]
                {
                    { 1L, null, null, 1L, "IN", 50m },
                    { 2L, null, null, 2L, "IN", 20m },
                    { 3L, null, null, 3L, "OUT", 5m }
                });

            migrationBuilder.InsertData(
                table: "invoices",
                columns: new[] { "id", "created_at", "discount", "order_id", "service_fee", "subtotal", "total", "vat" },
                values: new object[] { 1L, new DateTime(2026, 4, 10, 10, 52, 47, 128, DateTimeKind.Local).AddTicks(4817), 0m, 2L, 5000m, 120000m, 137000m, 12000m });

            migrationBuilder.InsertData(
                table: "menu_items",
                columns: new[] { "Id", "category_id", "description", "image_url", "name", "price", "status" },
                values: new object[,]
                {
                    { 1L, 1L, "Cơm chiên với tôm, mực", null, "Cơm chiên hải sản", 75000m, "AVAILABLE" },
                    { 2L, 1L, "Bò xào mềm", null, "Bò lúc lắc", 120000m, "AVAILABLE" },
                    { 3L, 2L, "Trà đào mát lạnh", null, "Trà đào", 30000m, "AVAILABLE" },
                    { 4L, 2L, "Cà phê truyền thống", null, "Cà phê sữa", 25000m, "AVAILABLE" },
                    { 5L, 3L, "Tráng miệng", null, "Bánh flan", 20000m, "AVAILABLE" }
                });

            migrationBuilder.InsertData(
                table: "reservations",
                columns: new[] { "Id", "customer_id", "guest_count", "reservation_time", "status", "table_id" },
                values: new object[] { 2L, 2L, 2, new DateTime(2026, 4, 12, 10, 52, 47, 128, DateTimeKind.Local).AddTicks(4932), "PENDING", 2L });

            migrationBuilder.InsertData(
                table: "combo_items",
                columns: new[] { "combo_id", "menu_item_id", "quantity" },
                values: new object[,]
                {
                    { 1L, 1L, 1 },
                    { 1L, 3L, 2 },
                    { 2L, 1L, 2 },
                    { 2L, 2L, 1 },
                    { 2L, 3L, 3 }
                });

            migrationBuilder.InsertData(
                table: "customer_vouchers",
                columns: new[] { "Id", "Code", "customer_id", "discount", "expiry_date", "is_used" },
                values: new object[] { 1L, "DISCOUNT10", 1L, 10000m, new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), false });

            migrationBuilder.InsertData(
                table: "order_items",
                columns: new[] { "id", "menu_item_id", "order_id", "price", "quantity" },
                values: new object[,]
                {
                    { 1L, 1L, 1L, 75000m, 2 },
                    { 2L, 3L, 1L, 30000m, 1 },
                    { 3L, 2L, 2L, 120000m, 1 }
                });

            migrationBuilder.InsertData(
                table: "recipes",
                columns: new[] { "ingredient_id", "menu_item_id", "quantity" },
                values: new object[,]
                {
                    { 1L, 1L, 5m },
                    { 3L, 1L, 5m },
                    { 2L, 2L, 5m }
                });

            migrationBuilder.InsertData(
                table: "reservations",
                columns: new[] { "Id", "customer_id", "guest_count", "reservation_time", "status", "table_id" },
                values: new object[] { 1L, 1L, 5, new DateTime(2026, 4, 11, 10, 52, 47, 128, DateTimeKind.Local).AddTicks(4926), "CONFIRMED", 4L });

            migrationBuilder.CreateIndex(
                name: "IX_combo_items_menu_item_id",
                table: "combo_items",
                column: "menu_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_vouchers_customer_id",
                table: "customer_vouchers",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_customers_account_id",
                table: "customers",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_logs_ingredient_id",
                table: "inventory_logs",
                column: "ingredient_id");

            migrationBuilder.CreateIndex(
                name: "IX_invoices_order_id",
                table: "invoices",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_menu_items_category_id",
                table: "menu_items",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_menu_item_id",
                table: "order_items",
                column: "menu_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_order_id",
                table: "order_items",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipes_ingredient_id",
                table: "recipes",
                column: "ingredient_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservations_customer_id",
                table: "reservations",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservations_table_id",
                table: "reservations",
                column: "table_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "combo_items");

            migrationBuilder.DropTable(
                name: "customer_vouchers");

            migrationBuilder.DropTable(
                name: "inventory_logs");

            migrationBuilder.DropTable(
                name: "invoices");

            migrationBuilder.DropTable(
                name: "order_items");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "recipes");

            migrationBuilder.DropTable(
                name: "reservations");

            migrationBuilder.DropTable(
                name: "combos");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "ingredients");

            migrationBuilder.DropTable(
                name: "menu_items");

            migrationBuilder.DropTable(
                name: "Tables");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "menu_categories");

            migrationBuilder.DropTable(
                name: "accounts");
        }
    }
}
