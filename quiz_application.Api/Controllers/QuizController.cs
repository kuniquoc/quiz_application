using quiz_application.Application.DTOs;
using quiz_application.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace quiz_application.Api.Controllers
{
    [ApiController]
    [Route("api/quiz")] // Base route cho các API trong controller này
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        /// <summary>
        /// Lấy danh sách tất cả các quiz có sẵn.
        /// </summary>
        /// <returns>Danh sách các quiz với thông tin tóm tắt.</returns>
        [HttpGet]
        public async Task<ActionResult<List<QuizSummaryDto>>> GetAllQuizzes()
        {
            try
            {
                var quizzes = await _quizService.GetAllQuizzesAsync();
                return Ok(quizzes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Bắt đầu một Quiz mới và lấy danh sách câu hỏi.
        /// </summary>
        /// <param name="quizId">ID của Quiz muốn bắt đầu.</param>
        /// <param name="request">Chứa thời điểm bắt đầu Quiz từ client.</param>
        /// <returns>Thông tin về Quiz và danh sách câu hỏi đã được sắp xếp/ngẫu nhiên.</returns>
        [HttpPost("{quizId}/start")]
        public async Task<ActionResult<QuizStartResponseDto>> StartQuiz(int quizId, [FromBody] QuizStartRequestDto request)
        {
            if (quizId != request.QuizId)
            {
                return BadRequest("QuizId in route and request body must match.");
            }
            try
            {
                var response = await _quizService.StartQuizAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi: Trong môi trường production, bạn nên log lỗi chi tiết
                // và trả về một mã lỗi chung (ví dụ: 500 Internal Server Error)
                // thay vì thông báo lỗi chi tiết của exception.
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Gửi câu trả lời của người dùng cho một câu hỏi.
        /// </summary>
        /// <param name="request">Chứa AttemptId, QuestionId và SelectedOptionId.</param>
        /// <returns>Phản hồi về việc câu trả lời có đúng hay không và đáp án đúng.</returns>
        [HttpPost("submit-answer")]
        public async Task<ActionResult<AnswerFeedbackDto>> SubmitAnswer([FromBody] SubmitAnswerRequestDto request)
        {
            try
            {
                var feedback = await _quizService.SubmitAnswerAsync(request);
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Kết thúc Quiz và lấy kết quả chi tiết.
        /// </summary>
        /// <param name="attemptId">ID của lần làm bài muốn kết thúc.</param>
        /// <param name="request">Chứa thời điểm kết thúc Quiz từ client.</param>
        /// <returns>Kết quả tổng quan của Quiz và danh sách câu hỏi để xem lại.</returns>
        [HttpPost("{attemptId}/finish")]
        public async Task<ActionResult<QuizResultDto>> FinishQuiz(int attemptId, [FromBody] QuizFinishRequestDto request)
        {
            try
            {
                var result = await _quizService.FinishQuizAndGetResultsAsync(attemptId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
