using Microsoft.EntityFrameworkCore;
using quiz_application.Infrastructure.Data;
using quiz_application.Application.Services;
using quiz_application.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// --- 0. Configure default ports ---
builder.WebHost.UseUrls("http://+:5000");

// --- 1. Register Services into Dependency Injection container ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext for Entity Framework Core
// Use SQLite (recommended for this test because it's simple):
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register QuizService: AddScoped means a new instance is created for each HTTP request.
builder.Services.AddScoped<IQuizService, QuizService>();

// --- 2. Build the application ---
var app = builder.Build();

// --- 3. Configure HTTP Request Pipeline (Middleware) ---
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Quiz API v1");
    c.RoutePrefix = "swagger";
});

// Global error handling middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

// app.UseAuthorization();
app.MapControllers();

// --- 4. Database Recreation and Seed Data on startup ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var configuration = services.GetRequiredService<IConfiguration>();

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();        // Check RecreateOnStartup configuration (default is false if not present)
        bool recreateOnStartup = configuration.GetValue<bool>("DatabaseSettings:RecreateOnStartup", false);

        if (recreateOnStartup)
        {
            logger.LogInformation("RecreateOnStartup is enabled: Deleting existing database..."); context.Database.EnsureDeleted(); // Delete current database

            logger.LogInformation("Creating new database from current model...");
            context.Database.EnsureCreated(); // Create new database from current model

            logger.LogInformation("Seeding fresh data...");
            SeedData.Initialize(services); // Initialize sample data

            logger.LogInformation("Database recreation and seeding completed successfully.");
        }
        else
        {            // Normal mode: Apply migrations and seed data if needed
            logger.LogInformation("Normal mode: Applying pending migrations...");
            context.Database.Migrate();

            // Seed data if database is empty (with old logic)
            if (!context.Quizzes.Any())
            {
                logger.LogInformation("Database is empty, seeding initial data...");
                SeedData.Initialize(services);
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database setup or seeding.");
        throw; // Re-throw to prevent application startup if there's a database error
    }
}

// --- 5. Run the application ---
app.Run();
