using quiz_application.Application.DTOs;
using quiz_application.Domain.Entities;
using quiz_application.Domain.Enums;
using quiz_application.Infrastructure.Data; // To access DbContext
using quiz_application.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace quiz_application.Application.Services
{
    public class QuizService : IQuizService
    {
        private readonly ApplicationDbContext _dbContext;

        public QuizService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // API 0: Get all quizzes
        public async Task<List<QuizSummaryDto>> GetAllQuizzesAsync()
        {
            var quizzes = await _dbContext.Quizzes
                .Include(q => q.Questions) // Include Questions to count the number of questions
                .Select(q => new QuizSummaryDto
                {
                    QuizId = q.QuizId,
                    QuizName = q.QuizName,
                    Description = q.Description,
                    PassPercentage = q.PassPercentage,
                    TimeLimitSeconds = q.TimeLimitSeconds,
                    TotalQuestions = q.Questions.Count()
                })
                .ToListAsync();
            return quizzes;
        }
        // API 1: Start Quiz and Get question list
        public async Task<QuizStartResponseDto> StartQuizAsync(int quizId)
        {
            // Validate QuizId
            if (quizId <= 0)
            {
                throw new BadRequestException("QuizId must be a positive integer.");
            }

            var quiz = await _dbContext.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(ques => ques.Options)
                .FirstOrDefaultAsync(q => q.QuizId == quizId);

            if (quiz == null)
            {
                throw new NotFoundException($"Quiz with ID {quizId} not found.");
            }
            if (quiz.Questions == null || !quiz.Questions.Any())
            {
                throw new BadRequestException("Quiz has no questions.");
            }            // Create a new record for the user's quiz attempt
            var newAttempt = new UserQuizAttempt
            {
                QuizId = quizId,
                StartTime = DateTime.UtcNow // Use server time instead of client time
            };

            _dbContext.UserQuizAttempts.Add(newAttempt);
            await _dbContext.SaveChangesAsync(); // Save to get AttemptId

            // --- Logic for adjusting question order ---
            // Load all questions into memory to sort randomly (OrderBy(Guid.NewGuid()) doesn't work directly on DB)
            var questions = quiz.Questions.ToList();
            if (quiz.QuestionOrderType == QuizQuestionOrderType.Random)
            {
                questions = questions.OrderBy(q => Guid.NewGuid()).ToList(); // Random in memory
            }
            else // QuizQuestionOrderType.Sequential (default)
            {
                questions = questions.OrderBy(q => q.OrderInQuiz).ToList();
            }

            // Map questions and options to DTOs
            var questionsDto = questions
                .Select(q => new QuestionDto
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    QuestionImage = q.QuestionImage,
                    Options = q.Options.Select(o => new OptionDto
                    {
                        OptionId = o.OptionId,
                        OptionText = o.OptionText
                    }).ToList()
                })
                .ToList();

            return new QuizStartResponseDto
            {
                AttemptId = newAttempt.AttemptId,
                QuizName = quiz.QuizName,
                TimeLimitSeconds = quiz.TimeLimitSeconds,
                Questions = questionsDto
            };
        }
        // API 2: Submit answer and get feedback
        public async Task<AnswerFeedbackDto> SubmitAnswerAsync(SubmitAnswerRequestDto request)
        {
            // Validate request object
            if (request == null)
            {
                throw new BadRequestException("Request cannot be null.");
            }

            // Validate AttemptId
            if (request.AttemptId <= 0)
            {
                throw new BadRequestException("AttemptId must be a positive integer.");
            }

            // Validate QuestionId
            if (request.QuestionId <= 0)
            {
                throw new BadRequestException("QuestionId must be a positive integer.");
            }

            // SelectedOptionId can be null (user skipped question), so no need to validate that

            // Check validity of Quiz Attempt
            var attempt = await _dbContext.UserQuizAttempts
                .FirstOrDefaultAsync(a => a.AttemptId == request.AttemptId);

            if (attempt == null)
            {
                throw new NotFoundException($"Quiz attempt with ID {request.AttemptId} not found.");
            }

            // Get the question and its options to validate the answer
            var question = await _dbContext.Questions
                .Include(q => q.Options)
                .FirstOrDefaultAsync(q => q.QuestionId == request.QuestionId);

            if (question == null)
            {
                throw new NotFoundException($"Question with ID {request.QuestionId} not found.");
            }

            // Verify the question belongs to the quiz in this attempt
            if (question.QuizId != attempt.QuizId)
            {
                throw new BadRequestException($"Question ID {request.QuestionId} does not belong to the quiz in attempt ID {request.AttemptId}.");
            }

            // Handle case where user does not select option (SelectedOptionId is null)
            Option? selectedOption = null;
            if (request.SelectedOptionId.HasValue)
            {
                selectedOption = question.Options.FirstOrDefault(o => o.OptionId == request.SelectedOptionId.Value);

                // Validate if the option exists and belongs to this question
                if (selectedOption == null)
                {
                    throw new BadRequestException($"Selected option with ID {request.SelectedOptionId.Value} not found for this question.");
                }

                // Double check that the option belongs to the specified question
                if (selectedOption.QuestionId != question.QuestionId)
                {
                    throw new BadRequestException($"Selected option with ID {request.SelectedOptionId.Value} does not belong to question ID {request.QuestionId}.");
                }
            }
            var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
            // An answer is correct if the user selected the correct option AND there is a correct option.
            bool isCorrect = (selectedOption != null && correctOption != null && correctOption.OptionId == selectedOption.OptionId);

            // Check if an answer for this question already exists in this attempt
            // Use the composite key (AttemptId, QuestionId) for search
            var existingAnswer = await _dbContext.UserAnswers
                .FirstOrDefaultAsync(ua => ua.AttemptId == request.AttemptId && ua.QuestionId == request.QuestionId);
            if (existingAnswer != null)
            {
                // If it exists, update the user's answer
                existingAnswer.SelectedOptionId = request.SelectedOptionId;
                existingAnswer.IsCorrect = isCorrect;
            }
            else
            {
                // If not, create a new UserAnswer record
                var userAnswer = new UserAnswer
                {
                    AttemptId = request.AttemptId,
                    QuestionId = request.QuestionId,
                    SelectedOptionId = request.SelectedOptionId,
                    IsCorrect = isCorrect
                };
                _dbContext.UserAnswers.Add(userAnswer);
            }

            await _dbContext.SaveChangesAsync();
            return new AnswerFeedbackDto
            {
                IsCorrect = isCorrect,
                CorrectOptionId = correctOption?.OptionId, // Return null if there is no correct option (data error)
                CorrectOptionText = correctOption?.OptionText
            };
        }

        // API 3: Finish Quiz and Get detailed results
        public async Task<QuizResultDto> FinishQuizAndGetResultsAsync(int attemptId)
        {
            // Validate attemptId parameter
            if (attemptId <= 0)
            {
                throw new BadRequestException("AttemptId must be a positive integer.");
            }

            var attempt = await _dbContext.UserQuizAttempts
                .Include(a => a.Quiz) // Load Quiz to get PassPercentage and TimeLimitSeconds
                .Include(a => a.UserAnswers) // Load user's answers
                    .ThenInclude(ua => ua.Question) // From UserAnswer, load Question
                        .ThenInclude(q => q.Options) // From Question, load Options
                .FirstOrDefaultAsync(a => a.AttemptId == attemptId);

            if (attempt == null) throw new NotFoundException($"Quiz attempt with ID {attemptId} not found.");

            // Set end time to current server time
            attempt.EndTime = DateTime.UtcNow;

            // Calculate total time spent on the quiz
            var totalTimeTaken = attempt.EndTime - attempt.StartTime;
            // Count correct/incorrect answers
            var correctAnswersCount = attempt.UserAnswers.Count(ua => ua.IsCorrect);
            var totalQuestionsInQuiz = await _dbContext.Questions.CountAsync(q => q.QuizId == attempt.QuizId);
            var incorrectAnswersCount = totalQuestionsInQuiz - correctAnswersCount;

            // Calculate PASS/FAIL criteria based on score
            var passPercentage = attempt.Quiz.PassPercentage;
            var userScorePercentage = (totalQuestionsInQuiz > 0) ? ((decimal)correctAnswersCount / totalQuestionsInQuiz * 100) : 0;
            bool isScorePassed = userScorePercentage >= passPercentage;
            // Calculate PASS/FAIL criteria based on time
            bool isTimePassed = true; // Default is pass if there's no time limit
            if (attempt.Quiz.TimeLimitSeconds.HasValue)
            {
                var timeDifference = totalTimeTaken.TotalSeconds - attempt.Quiz.TimeLimitSeconds.Value;
                // If time taken exceeds the limit, mark as failed
                // Buffer the time difference to check if it exceeds 1 second (to avoid issues with rounding or small delays)
                if (timeDifference > 1)
                {
                    isTimePassed = false; // Failed due to time limit
                }
                else
                {
                    isTimePassed = true; // Passed within time limit
                }
            }
            // Update final results in the Attempt record
            attempt.Score = correctAnswersCount;
            attempt.IsPassed = isScorePassed && isTimePassed; // Pass when SUFFICIENT SCORE AND WITHIN TIME LIMIT

            await _dbContext.SaveChangesAsync(); // Save final results of the attempt to DB            

            // Prepare list of questions for review (ReviewQuestions)
            // Get ALL questions from the quiz, sort by default order for easier tracking in review
            var allQuestionsForReview = await _dbContext.Questions
                                                    .Where(q => q.QuizId == attempt.QuizId)
                                                    .Include(q => q.Options)
                                                    .OrderBy(q => q.OrderInQuiz) // Always sort by OrderInQuiz for review
                                                    .ToListAsync();

            var reviewQuestions = new List<ReviewQuestionDto>();
            foreach (var question in allQuestionsForReview)
            {
                // Get user's answer for this question
                var userAnswer = attempt.UserAnswers.FirstOrDefault(ua => ua.QuestionId == question.QuestionId);

                // Get information about user's selection (if any)
                var yourAnswerOption = userAnswer != null && userAnswer.SelectedOptionId.HasValue
                    ? question.Options.FirstOrDefault(o => o.OptionId == userAnswer.SelectedOptionId.Value)
                    : null;

                // Get information about the correct answer
                var correctAnswerOption = question.Options.FirstOrDefault(o => o.IsCorrect);

                reviewQuestions.Add(new ReviewQuestionDto
                {
                    QuestionId = question.QuestionId,
                    QuestionText = question.QuestionText,
                    YourAnswerText = yourAnswerOption?.OptionText ?? "No answer", // "No answer" if user didn't answer or selected null
                    CorrectAnswerText = correctAnswerOption?.OptionText, // Can be null if there's no correct answer
                    WasCorrect = userAnswer?.IsCorrect ?? false // False if no answer
                });
            }

            return new QuizResultDto
            {
                TotalTimeTakenSeconds = totalTimeTaken.TotalSeconds,
                CorrectAnswersCount = correctAnswersCount,
                IncorrectAnswersCount = incorrectAnswersCount,
                IsPassed = attempt.IsPassed,
                PassPercentageRequired = passPercentage,
                TimeLimitSeconds = attempt.Quiz.TimeLimitSeconds,
                ReviewQuestions = reviewQuestions
            };
        }
    }
}
