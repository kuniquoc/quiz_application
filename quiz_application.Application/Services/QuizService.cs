using quiz_application.Application.DTOs;
using quiz_application.Domain.Entities;
using quiz_application.Domain.Enums;
using quiz_application.Infrastructure.Data; // Để truy cập DbContext
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace quiz_application.Application.Services
{
    public class QuizService : IQuizService
    {
        private readonly ApplicationDbContext _dbContext;

        public QuizService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // API 0: Lấy danh sách tất cả Quiz
        public async Task<List<QuizSummaryDto>> GetAllQuizzesAsync()
        {
            var quizzes = await _dbContext.Quizzes
                .Include(q => q.Questions) // Bao gồm Questions để đếm số câu hỏi
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

        // API 1: Bắt đầu Quiz và Lấy danh sách câu hỏi
        public async Task<QuizStartResponseDto> StartQuizAsync(QuizStartRequestDto request)
        {
            var quiz = await _dbContext.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(ques => ques.Options)
                .FirstOrDefaultAsync(q => q.QuizId == request.QuizId);

            if (quiz == null)
            {
                throw new Exception("Quiz not found."); // Ném Exception để Controller bắt và trả về BadRequest
            }

            // Tạo một bản ghi mới cho lần làm bài của người dùng
            var newAttempt = new UserQuizAttempt
            {
                QuizId = request.QuizId,
                ClientStartTime = DateTimeOffset.FromUnixTimeMilliseconds(request.ClientStartTimeTicks).UtcDateTime,
                // ClientEndTime, Score, IsPassed sẽ được cập nhật sau
            };
            _dbContext.UserQuizAttempts.Add(newAttempt);
            await _dbContext.SaveChangesAsync(); // Lưu để lấy AttemptId

            // --- Logic điều chỉnh thứ tự câu hỏi ---
            // Lấy tất cả câu hỏi vào bộ nhớ để sắp xếp ngẫu nhiên (OrderBy(Guid.NewGuid()) không hoạt động trực tiếp trên DB)
            var questions = quiz.Questions.ToList();

            if (quiz.QuestionOrderType == QuizQuestionOrderType.Random)
            {
                questions = questions.OrderBy(q => Guid.NewGuid()).ToList(); // Random trong bộ nhớ
            }
            else // QuizQuestionOrderType.Sequential (mặc định)
            {
                questions = questions.OrderBy(q => q.OrderInQuiz).ToList();
            }

            // Map các câu hỏi và lựa chọn sang DTOs
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

        // API 2: Gửi câu trả lời và lấy phản hồi
        public async Task<AnswerFeedbackDto> SubmitAnswerAsync(SubmitAnswerRequestDto request)
        {
            // Kiểm tra tính hợp lệ của Quiz Attempt
            var attempt = await _dbContext.UserQuizAttempts.FirstOrDefaultAsync(a => a.AttemptId == request.AttemptId);
            if (attempt == null) throw new Exception("Invalid Quiz Attempt.");

            // Lấy câu hỏi và các lựa chọn của nó để xác thực đáp án
            var question = await _dbContext.Questions
                .Include(q => q.Options)
                .FirstOrDefaultAsync(q => q.QuestionId == request.QuestionId);

            if (question == null) throw new Exception("Question not found.");

            // Xử lý trường hợp người dùng không chọn option (SelectedOptionId là null)
            Option? selectedOption = null;
            if (request.SelectedOptionId.HasValue)
            {
                selectedOption = question.Options.FirstOrDefault(o => o.OptionId == request.SelectedOptionId.Value);
                if (selectedOption == null) throw new Exception("Selected option not found for this question.");
            }

            var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
            // Một câu trả lời là đúng nếu người dùng chọn đúng đáp án VÀ có đáp án đúng tồn tại.
            bool isCorrect = (selectedOption != null && correctOption != null && correctOption.OptionId == selectedOption.OptionId);

            // Kiểm tra nếu câu trả lời cho câu hỏi này đã tồn tại trong lần thử này
            // Sử dụng khóa tổng hợp (AttemptId, QuestionId) để tìm kiếm
            var existingAnswer = await _dbContext.UserAnswers
                .FirstOrDefaultAsync(ua => ua.AttemptId == request.AttemptId && ua.QuestionId == request.QuestionId);

            if (existingAnswer != null)
            {
                // Nếu đã tồn tại, cập nhật câu trả lời của người dùng
                existingAnswer.SelectedOptionId = request.SelectedOptionId;
                existingAnswer.IsCorrect = isCorrect;
            }
            else
            {
                // Nếu chưa, tạo mới bản ghi UserAnswer
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
                CorrectOptionId = correctOption?.OptionId, // Trả về null nếu không có đáp án đúng (lỗi dữ liệu)
                CorrectOptionText = correctOption?.OptionText
            };
        }

        // API 3: Kết thúc Quiz và Lấy kết quả chi tiết
        public async Task<QuizResultDto> FinishQuizAndGetResultsAsync(int attemptId, QuizFinishRequestDto request)
        {
            var attempt = await _dbContext.UserQuizAttempts
                .Include(a => a.Quiz) // Nạp Quiz để lấy PassPercentage và TimeLimitSeconds
                .Include(a => a.UserAnswers) // Nạp các câu trả lời của người dùng
                    .ThenInclude(ua => ua.Question) // Từ UserAnswer, nạp Question
                        .ThenInclude(q => q.Options) // Từ Question, nạp Options
                .FirstOrDefaultAsync(a => a.AttemptId == attemptId);

            if (attempt == null) throw new Exception("Quiz attempt not found.");

            // Cập nhật thời điểm kết thúc từ client
            attempt.ClientEndTime = DateTimeOffset.FromUnixTimeMilliseconds(request.ClientEndTimeTicks).UtcDateTime;

            // Tính toán tổng thời gian làm bài
            var totalTimeTaken = attempt.ClientEndTime - attempt.ClientStartTime;

            // Đếm số câu đúng/sai
            var correctAnswersCount = attempt.UserAnswers.Count(ua => ua.IsCorrect);
            var totalQuestionsInQuiz = await _dbContext.Questions.CountAsync(q => q.QuizId == attempt.QuizId);
            var incorrectAnswersCount = totalQuestionsInQuiz - correctAnswersCount;

            // Tính toán tiêu chí ĐỖ/TRƯỢT dựa trên điểm số
            var passPercentage = attempt.Quiz.PassPercentage;
            var userScorePercentage = (totalQuestionsInQuiz > 0) ? ((decimal)correctAnswersCount / totalQuestionsInQuiz * 100) : 0;
            bool isScorePassed = userScorePercentage >= passPercentage;

            // Tính toán tiêu chí ĐỖ/TRƯỢT dựa trên thời gian
            bool isTimePassed = true; // Mặc định là đỗ nếu không có giới hạn thời gian
            if (attempt.Quiz.TimeLimitSeconds.HasValue)
            {
                isTimePassed = totalTimeTaken.TotalSeconds <= attempt.Quiz.TimeLimitSeconds.Value;
            }

            // Cập nhật kết quả cuối cùng vào bản ghi Attempt
            attempt.Score = correctAnswersCount;
            attempt.IsPassed = isScorePassed && isTimePassed; // Đỗ khi ĐỦ ĐIỂM VÀ TRONG THỜI GIAN

            await _dbContext.SaveChangesAsync(); // Lưu kết quả cuối cùng của attempt vào DB

            // Chuẩn bị danh sách các câu hỏi để xem lại (ReviewQuestions)
            // Lấy TẤT CẢ các câu hỏi của quiz, sắp xếp theo thứ tự mặc định để dễ theo dõi trong phần review
            var allQuestionsForReview = await _dbContext.Questions
                                                    .Where(q => q.QuizId == attempt.QuizId)
                                                    .Include(q => q.Options)
                                                    .OrderBy(q => q.OrderInQuiz) // Luôn sắp xếp theo OrderInQuiz cho review
                                                    .ToListAsync();

            var reviewQuestions = new List<ReviewQuestionDto>();
            foreach (var question in allQuestionsForReview)
            {
                // Lấy câu trả lời của người dùng cho câu hỏi này
                var userAnswer = attempt.UserAnswers.FirstOrDefault(ua => ua.QuestionId == question.QuestionId);

                // Lấy thông tin về lựa chọn của người dùng (nếu có)
                var yourAnswerOption = userAnswer != null && userAnswer.SelectedOptionId.HasValue
                    ? question.Options.FirstOrDefault(o => o.OptionId == userAnswer.SelectedOptionId.Value)
                    : null;

                // Lấy thông tin về đáp án đúng
                var correctAnswerOption = question.Options.FirstOrDefault(o => o.IsCorrect);

                reviewQuestions.Add(new ReviewQuestionDto
                {
                    QuestionId = question.QuestionId,
                    QuestionText = question.QuestionText,
                    YourAnswerText = yourAnswerOption?.OptionText ?? "No answer", // "No answer" nếu người dùng không trả lời hoặc chọn null
                    CorrectAnswerText = correctAnswerOption?.OptionText, // Có thể là null nếu không có đáp án đúng
                    WasCorrect = userAnswer?.IsCorrect ?? false // False nếu không trả lời
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
