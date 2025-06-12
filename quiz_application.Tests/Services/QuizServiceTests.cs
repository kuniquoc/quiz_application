using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using quiz_application.Application.DTOs;
using quiz_application.Application.Exceptions;
using quiz_application.Application.Services;
using quiz_application.Domain.Entities;
using quiz_application.Domain.Enums;
using quiz_application.Infrastructure.Data;
using Xunit;
using Xunit.Abstractions;

namespace quiz_application.Tests.Services
{
    public class QuizServiceTests
    {
        private readonly ITestOutputHelper _output;

        public QuizServiceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging() // Thêm dòng này để xem chi tiết lỗi khi xảy ra
                .Options;

            var dbContext = new ApplicationDbContext(options);
            return dbContext;
        }

        #region GetAllQuizzesAsync Tests

        [Fact]
        public async Task GetAllQuizzesAsync_ReturnsListOfQuizzes()
        {
            // Arrange
            var dbContext = CreateDbContext();

            // Add test data
            var quiz1 = new Quiz
            {
                QuizName = "Test Quiz 1",
                Description = "Test Description 1",
                PassPercentage = 60.0M,
                TimeLimitSeconds = 300,
                QuestionOrderType = QuizQuestionOrderType.Sequential
            };
            var quiz2 = new Quiz
            {
                QuizName = "Test Quiz 2",
                Description = "Test Description 2",
                PassPercentage = 70.0M,
                TimeLimitSeconds = null,
                QuestionOrderType = QuizQuestionOrderType.Random
            };

            // Add questions to quiz1
            var question1 = new Question
            {
                QuestionText = "Question 1",
                OrderInQuiz = 1,
                Options = new List<Option>
                {
                    new Option { OptionText = "Option 1", IsCorrect = true },
                    new Option { OptionText = "Option 2", IsCorrect = false }
                }
            };

            // Add questions to quiz2
            var question2 = new Question
            {
                QuestionText = "Question 2",
                OrderInQuiz = 1,
                Options = new List<Option>
                {
                    new Option { OptionText = "Option 1", IsCorrect = false },
                    new Option { OptionText = "Option 2", IsCorrect = true }
                }
            };

            quiz1.Questions.Add(question1);
            quiz2.Questions.Add(question2);

            dbContext.Quizzes.Add(quiz1);
            dbContext.Quizzes.Add(quiz2);
            await dbContext.SaveChangesAsync();

            var quizService = new QuizService(dbContext);

            // Act
            var result = await quizService.GetAllQuizzesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            // Verify quiz 1 details
            var resultQuiz1 = result.FirstOrDefault(q => q.QuizName == "Test Quiz 1");
            Assert.NotNull(resultQuiz1);
            Assert.Equal("Test Description 1", resultQuiz1.Description);
            Assert.Equal(60.0M, resultQuiz1.PassPercentage);
            Assert.Equal(300, resultQuiz1.TimeLimitSeconds);
            Assert.Equal(1, resultQuiz1.TotalQuestions);

            // Verify quiz 2 details
            var resultQuiz2 = result.FirstOrDefault(q => q.QuizName == "Test Quiz 2");
            Assert.NotNull(resultQuiz2);
            Assert.Equal("Test Description 2", resultQuiz2.Description);
            Assert.Equal(70.0M, resultQuiz2.PassPercentage);
            Assert.Null(resultQuiz2.TimeLimitSeconds);
            Assert.Equal(1, resultQuiz2.TotalQuestions);
        }

        [Fact]
        public async Task GetAllQuizzesAsync_ReturnsEmptyList_WhenNoQuizzes()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var quizService = new QuizService(dbContext);

            // Act
            var result = await quizService.GetAllQuizzesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region StartQuizAsync Tests

        [Fact]
        public async Task StartQuizAsync_CreatesAttemptAndReturnsQuestions_Sequential()
        {
            // Arrange
            var dbContext = CreateDbContext();

            // Add test data
            var quiz = new Quiz
            {
                QuizId = 1,
                QuizName = "Sequential Quiz",
                Description = "Quiz with sequential questions",
                PassPercentage = 60.0M,
                TimeLimitSeconds = 300,
                QuestionOrderType = QuizQuestionOrderType.Sequential
            };

            var question1 = new Question
            {
                QuestionId = 1,
                QuizId = 1,
                QuestionText = "Question 1",
                OrderInQuiz = 2,
                Options = new List<Option>
                {
                    new Option { OptionId = 1, OptionText = "Option 1.1", IsCorrect = true },
                    new Option { OptionId = 2, OptionText = "Option 1.2", IsCorrect = false }
                }
            };

            var question2 = new Question
            {
                QuestionId = 2,
                QuizId = 1,
                QuestionText = "Question 2",
                OrderInQuiz = 1,
                Options = new List<Option>
                {
                    new Option { OptionId = 3, OptionText = "Option 2.1", IsCorrect = false },
                    new Option { OptionId = 4, OptionText = "Option 2.2", IsCorrect = true }
                }
            };

            quiz.Questions.Add(question1);
            quiz.Questions.Add(question2);

            dbContext.Quizzes.Add(quiz);
            await dbContext.SaveChangesAsync();

            var quizService = new QuizService(dbContext);

            // Act
            var result = await quizService.StartQuizAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Sequential Quiz", result.QuizName);
            Assert.Equal(300, result.TimeLimitSeconds);
            Assert.Equal(2, result.Questions.Count);

            // Questions should be in order of OrderInQuiz for Sequential
            Assert.Equal(2, result.Questions[0].QuestionId); // Question with OrderInQuiz = 1
            Assert.Equal(1, result.Questions[1].QuestionId); // Question with OrderInQuiz = 2

            // Verify attempt was created
            var attempt = await dbContext.UserQuizAttempts.FirstOrDefaultAsync();
            Assert.NotNull(attempt);
            Assert.Equal(1, attempt.QuizId);
        }

        [Fact]
        public async Task StartQuizAsync_ThrowsNotFoundException_WhenQuizNotFound()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var quizService = new QuizService(dbContext);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => quizService.StartQuizAsync(999));
            Assert.Equal("Quiz with ID 999 not found.", exception.Message);
        }

        [Fact]
        public async Task StartQuizAsync_ThrowsBadRequestException_WhenQuizHasNoQuestions()
        {
            // Arrange
            var dbContext = CreateDbContext();

            // Add test data with no questions
            var quiz = new Quiz
            {
                QuizId = 1,
                QuizName = "Empty Quiz",
                Description = "Quiz with no questions",
                PassPercentage = 60.0M,
                TimeLimitSeconds = 300,
                QuestionOrderType = QuizQuestionOrderType.Sequential
            };

            dbContext.Quizzes.Add(quiz);
            await dbContext.SaveChangesAsync();

            var quizService = new QuizService(dbContext);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() => quizService.StartQuizAsync(1));
            Assert.Equal("Quiz has no questions.", exception.Message);
        }

        #endregion

        #region SubmitAnswerAsync Tests
        [Fact]
        public async Task SubmitAnswerAsync_ReturnsCorrectFeedback_WhenAnswerIsCorrect()
        {
            // Arrange
            var dbContext = CreateDbContext();

            // Add test data - Quiz with questions and options
            var quiz = new Quiz { QuizId = 1, QuizName = "Test Quiz", Description = "Test Quiz Description" };
            dbContext.Quizzes.Add(quiz);
            await dbContext.SaveChangesAsync();

            var question = new Question { QuizId = 1, QuestionText = "Test Question" };
            dbContext.Questions.Add(question);
            await dbContext.SaveChangesAsync();

            var option1 = new Option { QuestionId = question.QuestionId, OptionText = "Correct Option", IsCorrect = true };
            var option2 = new Option { QuestionId = question.QuestionId, OptionText = "Wrong Option", IsCorrect = false };
            dbContext.Options.Add(option1);
            dbContext.Options.Add(option2);
            await dbContext.SaveChangesAsync();

            // Add user attempt
            var attempt = new UserQuizAttempt { QuizId = 1, StartTime = DateTime.UtcNow };
            dbContext.UserQuizAttempts.Add(attempt);
            await dbContext.SaveChangesAsync();

            var quizService = new QuizService(dbContext);

            // Create submission request
            var request = new SubmitAnswerRequestDto
            {
                AttemptId = attempt.AttemptId,
                QuestionId = question.QuestionId,
                SelectedOptionId = option1.OptionId // correct option
            };

            // Act
            var result = await quizService.SubmitAnswerAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsCorrect);
            Assert.Equal(option1.OptionId, result.CorrectOptionId);
            Assert.Equal("Correct Option", result.CorrectOptionText);

            // Verify answer was recorded
            var answer = await dbContext.UserAnswers.FirstOrDefaultAsync();
            Assert.NotNull(answer);
            Assert.Equal(attempt.AttemptId, answer.AttemptId);
            Assert.Equal(question.QuestionId, answer.QuestionId);
            Assert.Equal(option1.OptionId, answer.SelectedOptionId);
            Assert.True(answer.IsCorrect);
        }
        [Fact]
        public async Task SubmitAnswerAsync_ReturnsIncorrectFeedback_WhenAnswerIsWrong()
        {
            // Arrange
            var dbContext = CreateDbContext();

            // Add test data - Quiz with questions and options
            var quiz = new Quiz { QuizName = "Test Quiz", Description = "Test Quiz Description" };
            dbContext.Quizzes.Add(quiz);
            await dbContext.SaveChangesAsync();

            var question = new Question { QuizId = quiz.QuizId, QuestionText = "Test Question" };
            dbContext.Questions.Add(question);
            await dbContext.SaveChangesAsync();

            var option1 = new Option { QuestionId = question.QuestionId, OptionText = "Correct Option", IsCorrect = true };
            var option2 = new Option { QuestionId = question.QuestionId, OptionText = "Wrong Option", IsCorrect = false };
            dbContext.Options.Add(option1);
            dbContext.Options.Add(option2);
            await dbContext.SaveChangesAsync();

            // Add user attempt
            var attempt = new UserQuizAttempt { QuizId = quiz.QuizId, StartTime = DateTime.UtcNow };
            dbContext.UserQuizAttempts.Add(attempt);
            await dbContext.SaveChangesAsync();

            var quizService = new QuizService(dbContext);

            // Create submission request
            var request = new SubmitAnswerRequestDto
            {
                AttemptId = attempt.AttemptId,
                QuestionId = question.QuestionId,
                SelectedOptionId = option2.OptionId // wrong option
            };

            // Act
            var result = await quizService.SubmitAnswerAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsCorrect);
            Assert.Equal(option1.OptionId, result.CorrectOptionId);
            Assert.Equal("Correct Option", result.CorrectOptionText);

            // Verify answer was recorded
            var answer = await dbContext.UserAnswers.FirstOrDefaultAsync();
            Assert.NotNull(answer);
            Assert.Equal(attempt.AttemptId, answer.AttemptId);
            Assert.Equal(question.QuestionId, answer.QuestionId);
            Assert.Equal(option2.OptionId, answer.SelectedOptionId);
            Assert.False(answer.IsCorrect);
        }
        [Fact]
        public async Task SubmitAnswerAsync_UpdatesExistingAnswer_WhenSubmittingMultipleTimes()
        {
            // Arrange
            var dbContext = CreateDbContext();

            // Add test data - Quiz with questions and options
            var quiz = new Quiz { QuizName = "Test Quiz", Description = "Test Quiz Description" };
            dbContext.Quizzes.Add(quiz);
            await dbContext.SaveChangesAsync();

            var question = new Question { QuizId = quiz.QuizId, QuestionText = "Test Question" };
            dbContext.Questions.Add(question);
            await dbContext.SaveChangesAsync();

            var option1 = new Option { QuestionId = question.QuestionId, OptionText = "Correct Option", IsCorrect = true };
            var option2 = new Option { QuestionId = question.QuestionId, OptionText = "Wrong Option", IsCorrect = false };
            dbContext.Options.Add(option1);
            dbContext.Options.Add(option2);
            await dbContext.SaveChangesAsync();

            // Add user attempt
            var attempt = new UserQuizAttempt { QuizId = quiz.QuizId, StartTime = DateTime.UtcNow };
            dbContext.UserQuizAttempts.Add(attempt);
            await dbContext.SaveChangesAsync();

            // Add existing answer (initially wrong)
            var existingAnswer = new UserAnswer
            {
                AttemptId = attempt.AttemptId,
                QuestionId = question.QuestionId,
                SelectedOptionId = option2.OptionId, // bắt đầu với câu trả lời sai
                IsCorrect = false
            };
            dbContext.UserAnswers.Add(existingAnswer);
            await dbContext.SaveChangesAsync();

            var quizService = new QuizService(dbContext);

            // Create submission request (now with correct answer)
            var request = new SubmitAnswerRequestDto
            {
                AttemptId = attempt.AttemptId,
                QuestionId = question.QuestionId,
                SelectedOptionId = option1.OptionId // chuyển sang đáp án đúng
            };

            // Act
            var result = await quizService.SubmitAnswerAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsCorrect);

            // Verify answer was updated, not duplicated
            var answers = await dbContext.UserAnswers.ToListAsync();
            Assert.Single(answers);

            var answer = answers.First();
            Assert.Equal(attempt.AttemptId, answer.AttemptId);
            Assert.Equal(question.QuestionId, answer.QuestionId);
            Assert.Equal(option1.OptionId, answer.SelectedOptionId);
            Assert.True(answer.IsCorrect);
        }

        [Theory]
        [InlineData(0, 1, 1, "AttemptId must be a positive integer.")]
        [InlineData(1, 0, 1, "QuestionId must be a positive integer.")]
        public async Task SubmitAnswerAsync_ThrowsBadRequestException_WithInvalidInputs(
            int attemptId, int questionId, int? optionId, string expectedMessage)
        {
            // Arrange
            var dbContext = CreateDbContext();
            var quizService = new QuizService(dbContext);

            var request = new SubmitAnswerRequestDto
            {
                AttemptId = attemptId,
                QuestionId = questionId,
                SelectedOptionId = optionId
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() => quizService.SubmitAnswerAsync(request));
            Assert.Equal(expectedMessage, exception.Message);
        }

        #endregion

        #region FinishQuizAndGetResultsAsync Tests
        [Fact]
        public async Task FinishQuizAndGetResultsAsync_CalculatesCorrectResults_WhenAllAnswersCorrect()
        {
            // Arrange
            var dbContext = CreateDbContext();

            // Add quiz first
            var quiz = new Quiz
            {
                QuizName = "Test Quiz",
                Description = "Test Quiz Description",
                PassPercentage = 60.0M,
                TimeLimitSeconds = 300
            };
            dbContext.Quizzes.Add(quiz);
            await dbContext.SaveChangesAsync();

            // Add questions with correct references
            var question1 = new Question
            {
                QuizId = quiz.QuizId, // Use actual generated ID
                QuestionText = "Q1",
                OrderInQuiz = 1
            };
            var question2 = new Question
            {
                QuizId = quiz.QuizId, // Use actual generated ID
                QuestionText = "Q2",
                OrderInQuiz = 2
            };
            dbContext.Questions.AddRange(question1, question2);
            await dbContext.SaveChangesAsync();

            // Add options with correct references
            var q1Option1 = new Option
            {
                QuestionId = question1.QuestionId, // Use actual generated ID
                OptionText = "Q1 Correct",
                IsCorrect = true
            };
            var q1Option2 = new Option
            {
                QuestionId = question1.QuestionId,
                OptionText = "Q1 Wrong",
                IsCorrect = false
            };
            var q2Option1 = new Option
            {
                QuestionId = question2.QuestionId,
                OptionText = "Q2 Correct",
                IsCorrect = true
            };
            var q2Option2 = new Option
            {
                QuestionId = question2.QuestionId,
                OptionText = "Q2 Wrong",
                IsCorrect = false
            };
            dbContext.Options.AddRange(q1Option1, q1Option2, q2Option1, q2Option2);
            await dbContext.SaveChangesAsync();

            // Add user attempt with correct references
            var startTime = DateTime.UtcNow.AddMinutes(-5); // Started 5 minutes ago
            var attempt = new UserQuizAttempt
            {
                QuizId = quiz.QuizId,
                StartTime = startTime,
                EndTime = startTime // This will be updated by the method
            };
            dbContext.UserQuizAttempts.Add(attempt);
            await dbContext.SaveChangesAsync();

            // Add correct answers with correct references
            var answer1 = new UserAnswer
            {
                AttemptId = attempt.AttemptId,
                QuestionId = question1.QuestionId,
                SelectedOptionId = q1Option1.OptionId, // correct option
                IsCorrect = true
            };

            var answer2 = new UserAnswer
            {
                AttemptId = attempt.AttemptId,
                QuestionId = question2.QuestionId,
                SelectedOptionId = q2Option1.OptionId, // correct option
                IsCorrect = true
            };

            dbContext.UserAnswers.AddRange(answer1, answer2);
            await dbContext.SaveChangesAsync();

            var quizService = new QuizService(dbContext);

            // Act
            var result = await quizService.FinishQuizAndGetResultsAsync(attempt.AttemptId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.CorrectAnswersCount);
            Assert.Equal(0, result.IncorrectAnswersCount);
            Assert.True(result.IsPassed);
            Assert.Equal(60.0M, result.PassPercentageRequired);
            Assert.Equal(300, result.TimeLimitSeconds);

            // Check that the attempt was updated
            var updatedAttempt = await dbContext.UserQuizAttempts.FindAsync(attempt.AttemptId);
            Assert.NotNull(updatedAttempt);
            Assert.NotEqual(startTime, updatedAttempt.EndTime); // End time should be updated
            Assert.Equal(2, updatedAttempt.Score);
            Assert.True(updatedAttempt.IsPassed);

            // Check review questions
            Assert.Equal(2, result.ReviewQuestions.Count);

            var reviewQ1 = result.ReviewQuestions.FirstOrDefault(q => q.QuestionId == question1.QuestionId);
            Assert.NotNull(reviewQ1);
            Assert.Equal("Q1", reviewQ1.QuestionText);
            Assert.Equal("Q1 Correct", reviewQ1.YourAnswerText);
            Assert.Equal("Q1 Correct", reviewQ1.CorrectAnswerText);
            Assert.True(reviewQ1.WasCorrect);

            var reviewQ2 = result.ReviewQuestions.FirstOrDefault(q => q.QuestionId == question2.QuestionId);
            Assert.NotNull(reviewQ2);
            Assert.Equal("Q2", reviewQ2.QuestionText);
            Assert.Equal("Q2 Correct", reviewQ2.YourAnswerText);
            Assert.Equal("Q2 Correct", reviewQ2.CorrectAnswerText);
            Assert.True(reviewQ2.WasCorrect);
        }
        [Fact]
        public async Task FinishQuizAndGetResultsAsync_FailsResult_WhenScoreIsBelowPassingPercentage()
        {
            // Arrange
            var dbContext = CreateDbContext();

            // Add test data - first add the quiz
            var quiz = new Quiz
            {
                QuizName = "Test Quiz",
                Description = "Test Quiz Description",
                PassPercentage = 60.0M, // Need 60% to pass
                TimeLimitSeconds = 300
            };
            dbContext.Quizzes.Add(quiz);
            await dbContext.SaveChangesAsync();

            // Create a list to track questions and options for consistent referencing
            var questions = new List<Question>();
            var correctOptions = new List<Option>();
            var wrongOptions = new List<Option>();

            // Add 5 questions separately with proper references
            for (int i = 1; i <= 5; i++)
            {
                var question = new Question
                {
                    QuizId = quiz.QuizId,
                    QuestionText = $"Q{i}",
                    OrderInQuiz = i
                };
                dbContext.Questions.Add(question);
                await dbContext.SaveChangesAsync();
                questions.Add(question);

                // Add options for each question
                var correctOption = new Option
                {
                    QuestionId = question.QuestionId,
                    OptionText = $"Q{i} Correct",
                    IsCorrect = true
                };
                var wrongOption = new Option
                {
                    QuestionId = question.QuestionId,
                    OptionText = $"Q{i} Wrong",
                    IsCorrect = false
                };

                dbContext.Options.AddRange(correctOption, wrongOption);
                await dbContext.SaveChangesAsync();

                correctOptions.Add(correctOption);
                wrongOptions.Add(wrongOption);
            }

            // Add user attempt with proper reference
            var attempt = new UserQuizAttempt
            {
                QuizId = quiz.QuizId,
                StartTime = DateTime.UtcNow.AddMinutes(-10)
            };
            dbContext.UserQuizAttempts.Add(attempt);
            await dbContext.SaveChangesAsync();

            // Add answers - only 2 out of 5 correct (40%)
            var userAnswers = new List<UserAnswer>
            {
                new UserAnswer { AttemptId = attempt.AttemptId, QuestionId = questions[0].QuestionId, SelectedOptionId = correctOptions[0].OptionId, IsCorrect = true },
                new UserAnswer { AttemptId = attempt.AttemptId, QuestionId = questions[1].QuestionId, SelectedOptionId = wrongOptions[1].OptionId, IsCorrect = false },
                new UserAnswer { AttemptId = attempt.AttemptId, QuestionId = questions[2].QuestionId, SelectedOptionId = correctOptions[2].OptionId, IsCorrect = true },
                new UserAnswer { AttemptId = attempt.AttemptId, QuestionId = questions[3].QuestionId, SelectedOptionId = wrongOptions[3].OptionId, IsCorrect = false },
                new UserAnswer { AttemptId = attempt.AttemptId, QuestionId = questions[4].QuestionId, SelectedOptionId = wrongOptions[4].OptionId, IsCorrect = false }
            };
            dbContext.UserAnswers.AddRange(userAnswers);
            await dbContext.SaveChangesAsync();

            var quizService = new QuizService(dbContext);

            // Act
            var result = await quizService.FinishQuizAndGetResultsAsync(attempt.AttemptId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.CorrectAnswersCount);
            Assert.Equal(3, result.IncorrectAnswersCount);
            Assert.False(result.IsPassed); // Should fail (40% < 60%)
            Assert.Equal(60.0M, result.PassPercentageRequired);

            // Check that the attempt was updated
            var updatedAttempt = await dbContext.UserQuizAttempts.FindAsync(attempt.AttemptId);
            Assert.NotNull(updatedAttempt);
            Assert.Equal(2, updatedAttempt.Score);
            Assert.False(updatedAttempt.IsPassed);
        }
        [Fact]
        public async Task FinishQuizAndGetResultsAsync_FailsResult_WhenTimeLimitExceeded()
        {
            // Arrange
            var dbContext = CreateDbContext();

            // Add test data - first add the quiz
            var quiz = new Quiz
            {
                QuizName = "Test Quiz",
                Description = "Test Quiz Description",
                PassPercentage = 60.0M,
                TimeLimitSeconds = 60 // 1 minute time limit
            };
            dbContext.Quizzes.Add(quiz);
            await dbContext.SaveChangesAsync();

            // Add a question with proper reference
            var question = new Question
            {
                QuizId = quiz.QuizId,
                QuestionText = "Q1",
                OrderInQuiz = 1
            };
            dbContext.Questions.Add(question);
            await dbContext.SaveChangesAsync();

            // Add option with proper reference
            var correctOption = new Option
            {
                QuestionId = question.QuestionId,
                OptionText = "Correct",
                IsCorrect = true
            };
            dbContext.Options.Add(correctOption);
            await dbContext.SaveChangesAsync();

            // Add user attempt with proper reference and that started well over the time limit
            var attempt = new UserQuizAttempt
            {
                QuizId = quiz.QuizId,
                StartTime = DateTime.UtcNow.AddMinutes(-2) // Started 2 minutes ago (> 1 minute limit)
            };
            dbContext.UserQuizAttempts.Add(attempt);
            await dbContext.SaveChangesAsync();

            // Add a correct answer with proper references
            var answer = new UserAnswer
            {
                AttemptId = attempt.AttemptId,
                QuestionId = question.QuestionId,
                SelectedOptionId = correctOption.OptionId,
                IsCorrect = true
            };
            dbContext.UserAnswers.Add(answer);
            await dbContext.SaveChangesAsync();

            var quizService = new QuizService(dbContext);

            // Act
            var result = await quizService.FinishQuizAndGetResultsAsync(attempt.AttemptId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.CorrectAnswersCount);
            Assert.Equal(0, result.IncorrectAnswersCount);
            Assert.False(result.IsPassed); // Should fail due to time limit
            Assert.Equal(60, result.TimeLimitSeconds);

            // Verify attempt was updated with failure
            var updatedAttempt = await dbContext.UserQuizAttempts.FindAsync(attempt.AttemptId);
            Assert.NotNull(updatedAttempt);
            Assert.False(updatedAttempt.IsPassed);
        }

        [Fact]
        public async Task FinishQuizAndGetResultsAsync_HandlesUnansweredQuestions()
        {
            // Arrange
            var dbContext = CreateDbContext();

            // Add quiz first
            var quiz = new Quiz
            {
                QuizName = "Test Quiz",
                Description = "Test Quiz Description",
                PassPercentage = 50.0M
            };
            dbContext.Quizzes.Add(quiz);
            await dbContext.SaveChangesAsync();

            var questions = new List<Question>();
            var options = new List<Option>();

            // Add 3 questions with proper references
            for (int i = 1; i <= 3; i++)
            {
                var question = new Question
                {
                    QuizId = quiz.QuizId,
                    QuestionText = $"Q{i}",
                    OrderInQuiz = i
                };
                dbContext.Questions.Add(question);
                await dbContext.SaveChangesAsync();
                questions.Add(question);

                var correctOption = new Option
                {
                    QuestionId = question.QuestionId,
                    OptionText = $"Q{i} Correct",
                    IsCorrect = true
                };
                dbContext.Options.Add(correctOption);
                await dbContext.SaveChangesAsync();
                options.Add(correctOption);
            }

            // Add user attempt with proper reference
            var attempt = new UserQuizAttempt
            {
                QuizId = quiz.QuizId,
                StartTime = DateTime.UtcNow.AddMinutes(-5)
            };
            dbContext.UserQuizAttempts.Add(attempt);
            await dbContext.SaveChangesAsync();

            // Only answer question 1
            var answer = new UserAnswer
            {
                AttemptId = attempt.AttemptId,
                QuestionId = questions[0].QuestionId,
                SelectedOptionId = options[0].OptionId,
                IsCorrect = true
            };
            dbContext.UserAnswers.Add(answer);
            await dbContext.SaveChangesAsync();

            var quizService = new QuizService(dbContext);

            // Act
            var result = await quizService.FinishQuizAndGetResultsAsync(attempt.AttemptId);

            // Assert
            Assert.Equal(1, result.CorrectAnswersCount);
            Assert.Equal(2, result.IncorrectAnswersCount); // Unanswered questions count as incorrect
            Assert.False(result.IsPassed); // 33% correct < 50% passing score

            // Check review questions
            Assert.Equal(3, result.ReviewQuestions.Count);

            // Answered question should show the answer
            var reviewQ1 = result.ReviewQuestions.First(q => q.QuestionId == questions[0].QuestionId);
            Assert.Equal("Q1 Correct", reviewQ1.YourAnswerText);
            Assert.True(reviewQ1.WasCorrect);

            // Unanswered questions should show "No answer"
            var reviewQ2 = result.ReviewQuestions.First(q => q.QuestionId == questions[1].QuestionId);
            Assert.Equal("No answer", reviewQ2.YourAnswerText);
            Assert.False(reviewQ2.WasCorrect);

            var reviewQ3 = result.ReviewQuestions.First(q => q.QuestionId == questions[2].QuestionId);
            Assert.Equal("No answer", reviewQ3.YourAnswerText);
            Assert.False(reviewQ3.WasCorrect);
        }

        #endregion
    }
}
