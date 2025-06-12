using System;
using System.Collections.Generic;
using quiz_application.Application.DTOs;
using Xunit;

namespace quiz_application.Tests.DTOs
{
    public class DtoTests
    {
        [Fact]
        public void QuizSummaryDto_Properties()
        {
            // Arrange
            var dto = new QuizSummaryDto
            {
                QuizId = 1,
                QuizName = "Test Quiz",
                Description = "Test Description",
                PassPercentage = 70.0M,
                TimeLimitSeconds = 300,
                TotalQuestions = 5
            };

            // Assert
            Assert.Equal(1, dto.QuizId);
            Assert.Equal("Test Quiz", dto.QuizName);
            Assert.Equal("Test Description", dto.Description);
            Assert.Equal(70.0M, dto.PassPercentage);
            Assert.Equal(300, dto.TimeLimitSeconds);
            Assert.Equal(5, dto.TotalQuestions);
        }

        [Fact]
        public void QuizStartResponseDto_Properties()
        {
            // Arrange
            var dto = new QuizStartResponseDto
            {
                AttemptId = 1,
                QuizName = "Test Quiz",
                TimeLimitSeconds = 300,
                Questions = new List<QuestionDto>
                {
                    new QuestionDto
                    {
                        QuestionId = 1,
                        QuestionText = "Question 1",
                        Options = new List<OptionDto>
                        {
                            new OptionDto { OptionId = 1, OptionText = "Option 1" },
                            new OptionDto { OptionId = 2, OptionText = "Option 2" }
                        }
                    },
                    new QuestionDto
                    {
                        QuestionId = 2,
                        QuestionText = "Question 2",
                        QuestionImage = "image.jpg",
                        Options = new List<OptionDto>
                        {
                            new OptionDto { OptionId = 3, OptionText = "Option 3" },
                            new OptionDto { OptionId = 4, OptionText = "Option 4" }
                        }
                    }
                }
            };

            // Assert
            Assert.Equal(1, dto.AttemptId);
            Assert.Equal("Test Quiz", dto.QuizName);
            Assert.Equal(300, dto.TimeLimitSeconds);
            Assert.Equal(2, dto.Questions.Count);

            // Check first question
            Assert.Equal(1, dto.Questions[0].QuestionId);
            Assert.Equal("Question 1", dto.Questions[0].QuestionText);
            Assert.Null(dto.Questions[0].QuestionImage);
            Assert.Equal(2, dto.Questions[0].Options.Count);

            // Check second question
            Assert.Equal(2, dto.Questions[1].QuestionId);
            Assert.Equal("Question 2", dto.Questions[1].QuestionText);
            Assert.Equal("image.jpg", dto.Questions[1].QuestionImage);
            Assert.Equal(2, dto.Questions[1].Options.Count);
        }

        [Fact]
        public void SubmitAnswerRequestDto_Properties()
        {
            // Arrange
            var dto = new SubmitAnswerRequestDto
            {
                AttemptId = 1,
                QuestionId = 2,
                SelectedOptionId = 3
            };

            // Assert
            Assert.Equal(1, dto.AttemptId);
            Assert.Equal(2, dto.QuestionId);
            Assert.Equal(3, dto.SelectedOptionId);

            // Test with null SelectedOptionId (skipping question)
            var skipDto = new SubmitAnswerRequestDto
            {
                AttemptId = 1,
                QuestionId = 2,
                SelectedOptionId = null
            };

            Assert.Equal(1, skipDto.AttemptId);
            Assert.Equal(2, skipDto.QuestionId);
            Assert.Null(skipDto.SelectedOptionId);
        }

        [Fact]
        public void AnswerFeedbackDto_Properties()
        {
            // Arrange
            var dto = new AnswerFeedbackDto
            {
                IsCorrect = true,
                CorrectOptionId = 5,
                CorrectOptionText = "Correct Option"
            };

            // Assert
            Assert.True(dto.IsCorrect);
            Assert.Equal(5, dto.CorrectOptionId);
            Assert.Equal("Correct Option", dto.CorrectOptionText);

            // Test with incorrect answer
            var incorrectDto = new AnswerFeedbackDto
            {
                IsCorrect = false,
                CorrectOptionId = 5,
                CorrectOptionText = "Correct Option"
            };

            Assert.False(incorrectDto.IsCorrect);
            Assert.Equal(5, incorrectDto.CorrectOptionId);
            Assert.Equal("Correct Option", incorrectDto.CorrectOptionText);
        }

        [Fact]
        public void QuizResultDto_Properties()
        {
            // Arrange
            var dto = new QuizResultDto
            {
                TotalTimeTakenSeconds = 180.5,
                CorrectAnswersCount = 7,
                IncorrectAnswersCount = 3,
                IsPassed = true,
                PassPercentageRequired = 60.0M,
                TimeLimitSeconds = 300,
                ReviewQuestions = new List<ReviewQuestionDto>
                {
                    new ReviewQuestionDto
                    {
                        QuestionId = 1,
                        QuestionText = "Question 1",
                        YourAnswerText = "Your Answer",
                        CorrectAnswerText = "Correct Answer",
                        WasCorrect = false
                    },
                    new ReviewQuestionDto
                    {
                        QuestionId = 2,
                        QuestionText = "Question 2",
                        YourAnswerText = "Correct Answer",
                        CorrectAnswerText = "Correct Answer",
                        WasCorrect = true
                    }
                }
            };

            // Assert
            Assert.Equal(180.5, dto.TotalTimeTakenSeconds);
            Assert.Equal(7, dto.CorrectAnswersCount);
            Assert.Equal(3, dto.IncorrectAnswersCount);
            Assert.True(dto.IsPassed);
            Assert.Equal(60.0M, dto.PassPercentageRequired);
            Assert.Equal(300, dto.TimeLimitSeconds);
            Assert.Equal(2, dto.ReviewQuestions.Count);

            // Check review questions
            Assert.Equal(1, dto.ReviewQuestions[0].QuestionId);
            Assert.Equal("Question 1", dto.ReviewQuestions[0].QuestionText);
            Assert.Equal("Your Answer", dto.ReviewQuestions[0].YourAnswerText);
            Assert.Equal("Correct Answer", dto.ReviewQuestions[0].CorrectAnswerText);
            Assert.False(dto.ReviewQuestions[0].WasCorrect);

            Assert.Equal(2, dto.ReviewQuestions[1].QuestionId);
            Assert.Equal("Question 2", dto.ReviewQuestions[1].QuestionText);
            Assert.Equal("Correct Answer", dto.ReviewQuestions[1].YourAnswerText);
            Assert.Equal("Correct Answer", dto.ReviewQuestions[1].CorrectAnswerText);
            Assert.True(dto.ReviewQuestions[1].WasCorrect);
        }

        [Fact]
        public void OptionDto_Properties()
        {
            // Arrange & Act
            var dto = new OptionDto
            {
                OptionId = 1,
                OptionText = "Test Option"
            };

            // Assert
            Assert.Equal(1, dto.OptionId);
            Assert.Equal("Test Option", dto.OptionText);
        }

        [Fact]
        public void QuestionDto_Properties()
        {
            // Arrange & Act
            var dto = new QuestionDto
            {
                QuestionId = 1,
                QuestionText = "Test Question",
                QuestionImage = "test.jpg",
                Options = new List<OptionDto>
                {
                    new OptionDto { OptionId = 1, OptionText = "Option 1" },
                    new OptionDto { OptionId = 2, OptionText = "Option 2" }
                }
            };

            // Assert
            Assert.Equal(1, dto.QuestionId);
            Assert.Equal("Test Question", dto.QuestionText);
            Assert.Equal("test.jpg", dto.QuestionImage);
            Assert.Equal(2, dto.Options.Count);
        }

        [Fact]
        public void ReviewQuestionDto_Properties()
        {
            // Arrange & Act
            var dto = new ReviewQuestionDto
            {
                QuestionId = 1,
                QuestionText = "Test Question",
                YourAnswerText = "Your Answer",
                CorrectAnswerText = "Correct Answer",
                WasCorrect = true
            };

            // Assert
            Assert.Equal(1, dto.QuestionId);
            Assert.Equal("Test Question", dto.QuestionText);
            Assert.Equal("Your Answer", dto.YourAnswerText);
            Assert.Equal("Correct Answer", dto.CorrectAnswerText);
            Assert.True(dto.WasCorrect);
        }
    }
}
