using quiz_application.Application.DTOs;
using quiz_application.Application.Services;
using quiz_application.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;


namespace quiz_application.Api.Controllers
{
    [ApiController]
    [Route("api/quiz")] // Base route for APIs in this controller
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }
        /// <summary>
        /// Get the list of all available quizzes.
        /// </summary>
        /// <returns>List of quizzes with summary information.</returns>
        [HttpGet]
        public async Task<ActionResult<List<QuizSummaryDto>>> GetAllQuizzes()
        {
            var quizzes = await _quizService.GetAllQuizzesAsync();
            return Ok(quizzes);
        }
        /// <summary>
        /// Start a new Quiz and get the list of questions.
        /// </summary>
        /// <param name="quizId">ID of the Quiz to start.</param>
        /// <param name="request">Contains the Quiz start time from client.</param>
        /// <returns>Information about the Quiz and the sorted/randomized list of questions.</returns>
        [HttpPost("{quizId}/start")]
        public async Task<ActionResult<QuizStartResponseDto>> StartQuiz(int quizId, [FromBody] QuizStartRequestDto request)
        {
            // Check route parameter matches body
            if (quizId != request.QuizId)
            {
                throw new BadRequestException("QuizId in route and request body must match.");
            }

            var response = await _quizService.StartQuizAsync(request);
            return Ok(response);
        }
        /// <summary>
        /// Submit user's answer for a question.
        /// </summary>
        /// <param name="request">Contains AttemptId, QuestionId and SelectedOptionId.</param>
        /// <returns>Feedback about whether the answer is correct and the correct answer.</returns>
        [HttpPost("submit-answer")]
        public async Task<ActionResult<AnswerFeedbackDto>> SubmitAnswer([FromBody] SubmitAnswerRequestDto request)
        {
            var feedback = await _quizService.SubmitAnswerAsync(request);
            return Ok(feedback);
        }
        /// <summary>
        /// Finish Quiz and get detailed results.
        /// </summary>
        /// <param name="attemptId">ID of the attempt to finish.</param>
        /// <param name="request">Contains Quiz end time from client.</param>
        /// <returns>Quiz overview results and list of questions for review.</returns>
        [HttpPost("{attemptId}/finish")]
        public async Task<ActionResult<QuizResultDto>> FinishQuiz(int attemptId, [FromBody] QuizFinishRequestDto request)
        {
            // Validate attemptId from route
            if (attemptId <= 0)
            {
                throw new BadRequestException("AttemptId must be a positive integer.");
            }

            var result = await _quizService.FinishQuizAndGetResultsAsync(attemptId, request);
            return Ok(result);
        }
    }
}
