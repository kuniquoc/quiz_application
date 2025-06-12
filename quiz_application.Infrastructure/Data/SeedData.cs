using quiz_application.Domain.Entities;
using quiz_application.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
                // Seed data logic - always runs when called
                // (Logic to check for empty database has been moved to Program.cs)

                // Create Quiz 1 (Random Order)
                var quiz1 = new Quiz
                {
                    QuizName = "General Knowledge Quiz (Random)",
                    Description = "Test your general knowledge with random questions.",
                    PassPercentage = 70.0M, // 70% correct answers required to pass
                    TimeLimitSeconds = 600, // 10 minutes

                    QuestionOrderType = QuizQuestionOrderType.Random
                };
                context.Quizzes.Add(quiz1);
                context.SaveChanges(); // Save Quiz to get QuizId for the questions

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

                context.SaveChanges(); // Save questions and options

                // Create Quiz 2 (Sequential Order)
                var quiz2 = new Quiz
                {
                    QuizName = "Science Fundamentals (Sequential)",
                    Description = "Basic science questions in a fixed order.",
                    PassPercentage = 60.0M,
                    TimeLimitSeconds = 300, // 5 minutes
                    QuestionOrderType = QuizQuestionOrderType.Sequential
                };
                context.Quizzes.Add(quiz2);
                context.SaveChanges(); // Save Quiz to get QuizId for the questions

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
                context.Questions.Add(q2_2); context.SaveChanges(); // Save questions and options

                // Create Quiz 3 (No time limit + with images)
                var quiz3 = new Quiz
                {
                    QuizName = "Visual Learning Quiz (No Time Limit)",
                    Description = "A quiz with images and no time pressure. Take your time!",
                    PassPercentage = 80.0M,
                    TimeLimitSeconds = null, // No time limit
                    QuestionOrderType = QuizQuestionOrderType.Random
                };
                context.Quizzes.Add(quiz3);
                context.SaveChanges();

                var q3_1 = new Question
                {
                    QuizId = quiz3.QuizId,
                    QuestionText = "What landmark is shown in this image?",
                    QuestionImage = "https://example.com/images/eiffel-tower.jpg",
                    OrderInQuiz = 1
                };
                q3_1.Options = new List<Option>
                {
                    new Option { OptionText = "Eiffel Tower", IsCorrect = true },
                    new Option { OptionText = "Big Ben", IsCorrect = false },
                    new Option { OptionText = "Statue of Liberty", IsCorrect = false },
                    new Option { OptionText = "Leaning Tower of Pisa", IsCorrect = false }
                };
                context.Questions.Add(q3_1);

                var q3_2 = new Question
                {
                    QuizId = quiz3.QuizId,
                    QuestionText = "Identify the animal species in this photo:",
                    QuestionImage = "https://example.com/images/tiger.jpg",
                    OrderInQuiz = 2
                };
                q3_2.Options = new List<Option>
                {
                    new Option { OptionText = "Lion", IsCorrect = false },
                    new Option { OptionText = "Tiger", IsCorrect = true },
                    new Option { OptionText = "Leopard", IsCorrect = false },
                    new Option { OptionText = "Cheetah", IsCorrect = false }
                };
                context.Questions.Add(q3_2);

                // Create Quiz 4 (Short time limit + No images + Sequential)
                var quiz4 = new Quiz
                {
                    QuizName = "Quick Math Challenge",
                    Description = "Fast-paced math questions. You have only 2 minutes!",
                    PassPercentage = 50.0M,
                    TimeLimitSeconds = 120, // 2 minutes - very short
                    QuestionOrderType = QuizQuestionOrderType.Sequential
                };
                context.Quizzes.Add(quiz4);
                context.SaveChanges();

                var q4_1 = new Question
                {
                    QuizId = quiz4.QuizId,
                    QuestionText = "What is 15 + 27?",
                    QuestionImage = null, // No image
                    OrderInQuiz = 1
                };
                q4_1.Options = new List<Option>
                {
                    new Option { OptionText = "42", IsCorrect = true },
                    new Option { OptionText = "41", IsCorrect = false },
                    new Option { OptionText = "43", IsCorrect = false },
                    new Option { OptionText = "40", IsCorrect = false }
                };
                context.Questions.Add(q4_1);

                var q4_2 = new Question
                {
                    QuizId = quiz4.QuizId,
                    QuestionText = "What is 8 × 7?",
                    QuestionImage = null,
                    OrderInQuiz = 2
                };
                q4_2.Options = new List<Option>
                {
                    new Option { OptionText = "54", IsCorrect = false },
                    new Option { OptionText = "56", IsCorrect = true },
                    new Option { OptionText = "58", IsCorrect = false },
                    new Option { OptionText = "52", IsCorrect = false }
                };
                context.Questions.Add(q4_2);

                var q4_3 = new Question
                {
                    QuizId = quiz4.QuizId,
                    QuestionText = "What is the square root of 64?",
                    QuestionImage = null,
                    OrderInQuiz = 3
                };
                q4_3.Options = new List<Option>
                {
                    new Option { OptionText = "6", IsCorrect = false },
                    new Option { OptionText = "7", IsCorrect = false },
                    new Option { OptionText = "8", IsCorrect = true },
                    new Option { OptionText = "9", IsCorrect = false }
                };
                context.Questions.Add(q4_3);

                // Create Quiz 5 (Long time limit + Mix of questions with and without images)
                var quiz5 = new Quiz
                {
                    QuizName = "Comprehensive Knowledge Test",
                    Description = "A thorough test covering various topics with mixed question types.",
                    PassPercentage = 75.0M,
                    TimeLimitSeconds = 1800, // 30 minutes - long duration
                    QuestionOrderType = QuizQuestionOrderType.Random
                };
                context.Quizzes.Add(quiz5);
                context.SaveChanges();

                var q5_1 = new Question
                {
                    QuizId = quiz5.QuizId,
                    QuestionText = "What programming language logo is this?",
                    QuestionImage = "https://example.com/images/python-logo.png",
                    OrderInQuiz = 1
                };
                q5_1.Options = new List<Option>
                {
                    new Option { OptionText = "Java", IsCorrect = false },
                    new Option { OptionText = "Python", IsCorrect = true },
                    new Option { OptionText = "JavaScript", IsCorrect = false },
                    new Option { OptionText = "C#", IsCorrect = false }
                };
                context.Questions.Add(q5_1);

                var q5_2 = new Question
                {
                    QuizId = quiz5.QuizId,
                    QuestionText = "Who wrote the novel '1984'?",
                    QuestionImage = null, // Literature question doesn't need an image
                    OrderInQuiz = 2
                };
                q5_2.Options = new List<Option>
                {
                    new Option { OptionText = "Aldous Huxley", IsCorrect = false },
                    new Option { OptionText = "George Orwell", IsCorrect = true },
                    new Option { OptionText = "Ray Bradbury", IsCorrect = false },
                    new Option { OptionText = "Philip K. Dick", IsCorrect = false }
                };
                context.Questions.Add(q5_2);

                var q5_3 = new Question
                {
                    QuizId = quiz5.QuizId,
                    QuestionText = "What country's flag is shown?",
                    QuestionImage = "https://example.com/images/japan-flag.jpg",
                    OrderInQuiz = 3
                };
                q5_3.Options = new List<Option>
                {
                    new Option { OptionText = "China", IsCorrect = false },
                    new Option { OptionText = "South Korea", IsCorrect = false },
                    new Option { OptionText = "Japan", IsCorrect = true },
                    new Option { OptionText = "Vietnam", IsCorrect = false }
                };
                context.Questions.Add(q5_3);

                var q5_4 = new Question
                {
                    QuizId = quiz5.QuizId,
                    QuestionText = "What is the process by which plants make their own food?",
                    QuestionImage = null,
                    OrderInQuiz = 4
                };
                q5_4.Options = new List<Option>
                {
                    new Option { OptionText = "Respiration", IsCorrect = false },
                    new Option { OptionText = "Photosynthesis", IsCorrect = true },
                    new Option { OptionText = "Digestion", IsCorrect = false },
                    new Option { OptionText = "Fermentation", IsCorrect = false }
                };
                context.Questions.Add(q5_4);

                // Create Quiz 6 (No time limit + Image-only questions)
                var quiz6 = new Quiz
                {
                    QuizName = "Visual Recognition Challenge",
                    Description = "All questions are image-based. Perfect for visual learners!",
                    PassPercentage = 65.0M,
                    TimeLimitSeconds = null, // No time limit
                    QuestionOrderType = QuizQuestionOrderType.Sequential
                };
                context.Quizzes.Add(quiz6);
                context.SaveChanges();

                var q6_1 = new Question
                {
                    QuizId = quiz6.QuizId,
                    QuestionText = "What architectural style is this building?",
                    QuestionImage = "https://example.com/images/gothic-cathedral.jpg",
                    OrderInQuiz = 1
                };
                q6_1.Options = new List<Option>
                {
                    new Option { OptionText = "Baroque", IsCorrect = false },
                    new Option { OptionText = "Gothic", IsCorrect = true },
                    new Option { OptionText = "Renaissance", IsCorrect = false },
                    new Option { OptionText = "Modern", IsCorrect = false }
                };
                context.Questions.Add(q6_1);

                var q6_2 = new Question
                {
                    QuizId = quiz6.QuizId,
                    QuestionText = "What type of cloud formation is this?",
                    QuestionImage = "https://example.com/images/cumulus-clouds.jpg",
                    OrderInQuiz = 2
                };
                q6_2.Options = new List<Option>
                {
                    new Option { OptionText = "Stratus", IsCorrect = false },
                    new Option { OptionText = "Cumulus", IsCorrect = true },
                    new Option { OptionText = "Cirrus", IsCorrect = false },
                    new Option { OptionText = "Nimbus", IsCorrect = false }
                };
                context.Questions.Add(q6_2);

                context.SaveChanges(); // Save all new data
            }
        }
    }
}
