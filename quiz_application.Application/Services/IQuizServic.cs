using quiz_application.Application.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace quiz_application.Application.Services
{
    public interface IQuizService
    {
        Task<List<QuizSummaryDto>> GetAllQuizzesAsync();
        Task<QuizStartResponseDto> StartQuizAsync(QuizStartRequestDto request);
        Task<AnswerFeedbackDto> SubmitAnswerAsync(SubmitAnswerRequestDto request);
        Task<QuizResultDto> FinishQuizAndGetResultsAsync(int attemptId, QuizFinishRequestDto request);
    }
}