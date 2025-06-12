using quiz_application.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace quiz_application.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Định nghĩa DbSet cho mỗi Entity để EF Core có thể theo dõi và tương tác với các bảng
        public DbSet<Quiz> Quizzes { get; set; } = null!;
        public DbSet<Question> Questions { get; set; } = null!;
        public DbSet<Option> Options { get; set; } = null!;
        public DbSet<UserQuizAttempt> UserQuizAttempts { get; set; } = null!;
        public DbSet<UserAnswer> UserAnswers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình khóa chính cho UserQuizAttempt
            modelBuilder.Entity<UserQuizAttempt>()
                .HasKey(ua => ua.AttemptId);

            // Cấu hình khóa chính tổng hợp cho UserAnswer
            modelBuilder.Entity<UserAnswer>()
                .HasKey(ua => new { ua.AttemptId, ua.QuestionId });

            // Cấu hình mối quan hệ giữa UserAnswer và Option (SelectedOptionId)
            // SelectedOptionId là nullable, nên mối quan hệ này là tùy chọn (IsRequired(false))
            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.SelectedOption)
                .WithMany()
                .HasForeignKey(ua => ua.SelectedOptionId)
                .IsRequired(false); // Cho phép SelectedOptionId là NULL

            // --- Cấu hình các mối quan hệ và ràng buộc DeleteBehavior ---

            // Quiz (1) --- n --- Question (Non-Identifying)
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(quiz => quiz.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            // Question (1) --- n --- Option (Non-Identifying)
            modelBuilder.Entity<Option>()
                .HasOne(o => o.Question)
                .WithMany(q => q.Options)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quiz (1) --- n --- UserQuizAttempt (Non-Identifying)
            modelBuilder.Entity<UserQuizAttempt>()
                .HasOne(a => a.Quiz)
                .WithMany(q => q.UserQuizAttempts)
                .HasForeignKey(a => a.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserQuizAttempt (1) --- n --- UserAnswer (Identifying Relationship - vì là khóa tổng hợp)
            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.Attempt)
                .WithMany(a => a.UserAnswers)
                .HasForeignKey(ua => ua.AttemptId)
                .OnDelete(DeleteBehavior.Cascade);

            // Question (1) --- n --- UserAnswer (Identifying Relationship - vì là khóa tổng hợp)
            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.Question)
                .WithMany()
                .HasForeignKey(ua => ua.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
// Lưu ý: Các DbSet được khởi tạo với null! để tránh cảnh báo nullability, vì chúng sẽ được khởi tạo bởi EF Core khi tạo cơ sở dữ liệu.