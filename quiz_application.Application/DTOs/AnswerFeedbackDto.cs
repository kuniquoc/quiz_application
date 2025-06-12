namespace quiz_application.Application.DTOs
{
    public class AnswerFeedbackDto
    {
        public bool IsCorrect { get; set; }         // Server xác nhận đúng/sai
        public int? CorrectOptionId { get; set; }   // ID của đáp án đúng (để client highlight) - nullable
        public string? CorrectOptionText { get; set; } // Nội dung đáp án đúng (để client hiển thị) - nullable
    }
}
