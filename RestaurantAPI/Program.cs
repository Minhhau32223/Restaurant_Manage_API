using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.src.Data;
using RestaurantAPI.src.Exceptions;
using RestaurantAPI.src.Route;
using RestaurantAPI.src.Services;
using RestaurantAPI.src.Services.Interfaces;
using RestaurantAPI.src.Validator.Converters;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RestaurantAPI
{
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
     
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContextPool<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // JWT Authentication
            var jwtSecret = builder.Configuration["Jwt:Secret"]
                ?? throw new InvalidOperationException("JWT Secret chưa được cấu hình trong appsettings.json");
            var key = Encoding.UTF8.GetBytes(jwtSecret);

            builder.Services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents {
                    OnChallenge = async context => {
                        // Ngừng các xử lý mặc định để tránh ghi đè Header
                        context.HandleResponse();

                        if (!context.Response.HasStarted) {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

                            var response = ApiResponse<object>.ErrorResponse("Token không hợp lệ hoặc đã hết hạn.");
                            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                            });
                            await context.Response.WriteAsync(json);
                        }
                    },
                    OnForbidden = async context => {
                        if (!context.Response.HasStarted) {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";

                            var response = ApiResponse<object>.ErrorResponse("Bạn không có quyền truy cập vào chức năng này.");
                            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                            });
                            await context.Response.WriteAsync(json);
                        }
                    }
                };  
            });

            builder.Services.AddScoped<ITokenService, TokenService>();
            //builder.Services.AddScoped<IComboService, ComboService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IInvoiceService, InvoiceService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<ITableService, TableService>();

            builder.Services.AddScoped<IPricingService, PricingService>();

            builder.Services.AddScoped<IIngredientServices, IngredientService>();
            builder.Services.AddScoped<IInventoryService, InventoryService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<ICustomerVoucherService, VoucherService>();
            builder.Services.AddScoped<IReservationService, ReservationService>();

            builder.Services.AddScoped<IComboService, ComboService>();
            builder.Services.AddScoped<IMenuService, MenuService>();
            builder.Services.AddScoped<IRecipeService, RecipeService>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo {
                    Title = "Restaurant API",
                    Version = "v1",
                    Description = "Restaurant Management Backend REST API"
                });
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Nhập JWT token nhận được từ /api/auth/login"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                options.SerializerOptions.Converters.Add(new DecimalFormatConverter());
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            builder.Services.AddAuthorization
                (options => {
                    options.AddPolicy("admin", policy =>
                        policy.RequireRole("ADMIN"));

                    options.AddPolicy("staff", policy =>
                        policy.RequireRole("ADMIN", "STAFF"));

                    options.AddPolicy("customer", policy =>
                        policy.RequireRole("ADMIN", "STAFF", "CUSTOMER"));
                });

            var app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API v1");
                });
            }

            //app.UseHttpsRedirection();
            //app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapAccountRoute();
            app.MapAuthRoute();
            //app.MapCombosRoute();
            app.MapTableRoute();
            app.MapPricingRoute();
            app.MapInvoiceRoute();
            app.MapOrderRoute();
            app.MapPaymentRoute();
            app.MapIngredientRoute();
            app.MapInventoryRoute();
            app.MapCustomerRoute();
            app.MapVoucherRoute();
            app.MapReservationRoute();

            //app.MapInventoryCustomerRoutes();
            app.MapCombosRoute();
            app.MapMenuRoute();
            app.MapRecipiesRoute();

            app.Run();
        }
    }
}
