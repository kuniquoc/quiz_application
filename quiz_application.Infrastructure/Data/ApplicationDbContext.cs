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

        // Define DbSet for each Entity so EF Core can track and interact with the tables
        public DbSet<Quiz> Quizzes { get; set; } = null!;
        public DbSet<Question> Questions { get; set; } = null!;
        public DbSet<Option> Options { get; set; } = null!;
        public DbSet<UserQuizAttempt> UserQuizAttempts { get; set; } = null!;
        public DbSet<UserAnswer> UserAnswers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure primary key for UserQuizAttempt
            modelBuilder.Entity<UserQuizAttempt>()
                .HasKey(ua => ua.AttemptId);

            // Configure composite primary key for UserAnswer
            modelBuilder.Entity<UserAnswer>()
                .HasKey(ua => new { ua.AttemptId, ua.QuestionId });           
                
            // Configure relationship between UserAnswer and Option (SelectedOptionId)
            // SelectedOptionId is nullable, so this relationship is optional (IsRequired(false))
            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.SelectedOption)
                .WithMany()
                .HasForeignKey(ua => ua.SelectedOptionId)
                .IsRequired(false); // Allow SelectedOptionId to be NULL

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

            // UserQuizAttempt (1) --- n --- UserAnswer (Identifying Relationship - because it's part of composite key)
            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.Attempt)
                .WithMany(a => a.UserAnswers)
                .HasForeignKey(ua => ua.AttemptId)
                .OnDelete(DeleteBehavior.Cascade);

            // Question (1) --- n --- UserAnswer (Identifying Relationship - because it's part of composite key)
            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.Question)
                .WithMany()
                .HasForeignKey(ua => ua.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
// Note: DbSets are initialized with null! to avoid nullability warnings, as they will be initialized by EF Core when creating the database.