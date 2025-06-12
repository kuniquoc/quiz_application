using System;
using System.Collections.Generic;
using System.Linq;
using quiz_application.Domain.Entities;
using quiz_application.Domain.Enums;
using Xunit;

namespace quiz_application.Tests.Domain
{
    public class EntityTests
    {
        [Fact]
        public void Quiz_Navigation_Properties_Initialized()
        {
            // Arrange & Act
            var quiz = new Quiz();

            // Assert
            Assert.NotNull(quiz.Questions);
            Assert.NotNull(quiz.UserQuizAttempts);
            Assert.Empty(quiz.Questions);
            Assert.Empty(quiz.UserQuizAttempts);
        }

        [Fact]
        public void Question_Navigation_Properties_Initialized()
        {
            // Arrange & Act
            var question = new Question();

            // Assert
            Assert.NotNull(question.Options);
            Assert.Empty(question.Options);
        }

        [Fact]
        public void UserQuizAttempt_Navigation_Properties_Initialized()
        {
            // Arrange & Act
            var attempt = new UserQuizAttempt();

            // Assert
            Assert.NotNull(attempt.UserAnswers);
            Assert.Empty(attempt.UserAnswers);
        }

        [Fact]
        public void Quiz_Can_Add_Questions()
        {
            // Arrange
            var quiz = new Quiz
            {
                QuizId = 1,
                QuizName = "Test Quiz",
                Description = "Test Description",
                PassPercentage = 60.0M,
                TimeLimitSeconds = 300,
                QuestionOrderType = QuizQuestionOrderType.Sequential
            };

            var question1 = new Question
            {
                QuestionId = 1,
                QuestionText = "Question 1",
                OrderInQuiz = 1
            };

            var question2 = new Question
            {
                QuestionId = 2,
                QuestionText = "Question 2",
                OrderInQuiz = 2
            };

            // Act
            quiz.Questions.Add(question1);
            quiz.Questions.Add(question2);

            // Assert
            Assert.Equal(2, quiz.Questions.Count);
            Assert.Contains(quiz.Questions, q => q.QuestionId == 1);
            Assert.Contains(quiz.Questions, q => q.QuestionId == 2);
        }

        [Fact]
        public void Question_Can_Add_Options()
        {
            // Arrange
            var question = new Question
            {
                QuestionId = 1,
                QuestionText = "Test Question"
            };

            var option1 = new Option
            {
                OptionId = 1,
                OptionText = "Option 1",
                IsCorrect = true
            };

            var option2 = new Option
            {
                OptionId = 2,
                OptionText = "Option 2",
                IsCorrect = false
            };

            // Act
            question.Options.Add(option1);
            question.Options.Add(option2);

            // Assert
            Assert.Equal(2, question.Options.Count);
            Assert.Single(question.Options.Where(o => o.IsCorrect));
            Assert.Contains(question.Options, o => o.OptionId == 1 && o.IsCorrect);
            Assert.Contains(question.Options, o => o.OptionId == 2 && !o.IsCorrect);
        }

        [Fact]
        public void UserQuizAttempt_Can_Add_UserAnswers()
        {
            // Arrange
            var attempt = new UserQuizAttempt
            {
                AttemptId = 1,
                QuizId = 1,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddMinutes(10)
            };

            var answer1 = new UserAnswer
            {
                AttemptId = 1,
                QuestionId = 1,
                SelectedOptionId = 1,
                IsCorrect = true
            };

            var answer2 = new UserAnswer
            {
                AttemptId = 1,
                QuestionId = 2,
                SelectedOptionId = 4,
                IsCorrect = false
            };

            // Act
            attempt.UserAnswers.Add(answer1);
            attempt.UserAnswers.Add(answer2);

            // Assert
            Assert.Equal(2, attempt.UserAnswers.Count);
            Assert.Single(attempt.UserAnswers.Where(a => a.IsCorrect));
            Assert.Single(attempt.UserAnswers.Where(a => !a.IsCorrect));
        }

        [Fact]
        public void QuizQuestionOrderType_Enum_Values()
        {
            // Test that the enum has the expected values
            Assert.Equal(0, (int)QuizQuestionOrderType.Sequential);
            Assert.Equal(1, (int)QuizQuestionOrderType.Random);

            // Test enum conversion
            var sequential = QuizQuestionOrderType.Sequential;
            var random = QuizQuestionOrderType.Random;

            Assert.Equal("Sequential", sequential.ToString());
            Assert.Equal("Random", random.ToString());
        }
    }
}
