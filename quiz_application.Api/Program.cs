using Microsoft.EntityFrameworkCore;
using quiz_application.Infrastructure.Data; // Namespace của DbContext
using quiz_application.Application.Services; // Namespace của Services
// using quiz_application.Domain.Entities; // Chỉ để SeedData (cần để SeedData.Initialize)
// using Microsoft.Extensions.Logging; // Thêm using này cho ILogger

var builder = WebApplication.CreateBuilder(args);

// --- 0. Cấu hình cổng mặc định ---
builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");

// --- 1. Đăng ký Services vào Dependency Injection container ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình DbContext cho Entity Framework Core
// Sử dụng SQLite (khuyến nghị cho bài test này vì đơn giản):
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký QuizService: AddScoped nghĩa là một instance mới được tạo cho mỗi HTTP request.
builder.Services.AddScoped<IQuizService, QuizService>();

// --- 2. Xây dựng ứng dụng ---
var app = builder.Build();

// --- 3. Cấu hình HTTP Request Pipeline (Middleware) ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Chỉ sử dụng HTTPS redirection trong production
    app.UseHttpsRedirection();
}

// app.UseAuthorization();
app.MapControllers();

// --- 4. Tùy chọn: Seed Data và áp dụng Migrations khi khởi động ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Áp dụng tất cả các migrations đang chờ xử lý vào database
        context.Database.Migrate();

        // Khởi tạo dữ liệu mẫu nếu database trống
        SeedData.Initialize(services); // Gọi phương thức seed data của bạn
    }
    catch (Exception ex)
    {
        // Log lỗi nếu có vấn đề khi seed database
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database migration or seeding.");
    }
}

// --- 5. Chạy ứng dụng ---
app.Run();
