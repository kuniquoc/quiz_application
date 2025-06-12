using quiz_application.Domain.Entities;
using quiz_application.Domain.Enums; // Thêm using để sử dụng enum
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic;

namespace quiz_application.Infrastructure.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ApplicationDbContext>>()))
            {
                // Kiểm tra xem đã có dữ liệu Quiz nào chưa
                if (context.Quizzes.Any())
                {
                    return;   // DB đã có dữ liệu, không cần seed nữa
                }

                // Tạo Quiz 1 (Random Order)
                var quiz1 = new Quiz
                {
                    QuizName = "General Knowledge Quiz (Random)",
                    Description = "Test your general knowledge with random questions.",
                    PassPercentage = 70.0M, // Cần 70% đúng để đỗ
                    TimeLimitSeconds = 600, // 10 phút
                    QuestionOrderType = QuizQuestionOrderType.Random
                };
                context.Quizzes.Add(quiz1);
                context.SaveChanges(); // Lưu Quiz để có QuizId cho các câu hỏi

                var q1_1 = new Question { QuizId = quiz1.QuizId, QuestionText = "What is the capital of France?", OrderInQuiz = 1 };
                q1_1.Options = new List<Option>
                {
                    new Option { OptionText = "Berlin", IsCorrect = false },
                    new Option { OptionText = "Paris", IsCorrect = true },
                    new Option { OptionText = "Rome", IsCorrect = false },
                    new Option { OptionText = "Madrid", IsCorrect = false }
                };
                context.Questions.Add(q1_1);

                var q1_2 = new Question { QuizId = quiz1.QuizId, QuestionText = "Which planet is known as the Red Planet?", OrderInQuiz = 2 };
                q1_2.Options = new List<Option>
                {
                    new Option { OptionText = "Earth", IsCorrect = false },
                    new Option { OptionText = "Mars", IsCorrect = true },
                    new Option { OptionText = "Jupiter", IsCorrect = false },
                    new Option { OptionText = "Venus", IsCorrect = false }
                };
                context.Questions.Add(q1_2);

                var q1_3 = new Question { QuizId = quiz1.QuizId, QuestionText = "What is the largest ocean on Earth?", OrderInQuiz = 3 };
                q1_3.Options = new List<Option>
                {
                    new Option { OptionText = "Atlantic Ocean", IsCorrect = false },
                    new Option { OptionText = "Indian Ocean", IsCorrect = false },
                    new Option { OptionText = "Arctic Ocean", IsCorrect = false },
                    new Option { OptionText = "Pacific Ocean", IsCorrect = true }
                };
                context.Questions.Add(q1_3);

                context.SaveChanges(); // Lưu các câu hỏi và lựa chọn

                // Tạo Quiz 2 (Sequential Order)
                var quiz2 = new Quiz
                {
                    QuizName = "Science Fundamentals (Sequential)",
                    Description = "Basic science questions in a fixed order.",
                    PassPercentage = 60.0M,
                    TimeLimitSeconds = 300, // 5 phút
                    QuestionOrderType = QuizQuestionOrderType.Sequential
                };
                context.Quizzes.Add(quiz2);
                context.SaveChanges(); // Lưu Quiz để có QuizId cho các câu hỏi

                var q2_1 = new Question { QuizId = quiz2.QuizId, QuestionText = "What is H2O commonly known as?", OrderInQuiz = 1 };
                q2_1.Options = new List<Option>
                {
                    new Option { OptionText = "Salt", IsCorrect = false },
                    new Option { OptionText = "Sugar", IsCorrect = false },
                    new Option { OptionText = "Water", IsCorrect = true },
                    new Option { OptionText = "Oxygen", IsCorrect = false }
                };
                context.Questions.Add(q2_1);

                var q2_2 = new Question { QuizId = quiz2.QuizId, QuestionText = "What is the chemical symbol for Gold?", OrderInQuiz = 2 };
                q2_2.Options = new List<Option>
                {
                    new Option { OptionText = "Ag", IsCorrect = false },
                    new Option { OptionText = "Au", IsCorrect = true },
                    new Option { OptionText = "Fe", IsCorrect = false },
                    new Option { OptionText = "Hg", IsCorrect = false }
                };
                context.Questions.Add(q2_2);

                context.SaveChanges(); // Lưu các câu hỏi và lựa chọn
            }
        }
    }
}
