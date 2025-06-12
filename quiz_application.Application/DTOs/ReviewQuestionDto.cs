namespace quiz_application.Application.DTOs
{
    public class ReviewQuestionDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = null!;
        public string? YourAnswerText { get; set; } // Câu trả lời của người dùng
        public string? CorrectAnswerText { get; set; } // Đáp án đúng
        public bool WasCorrect { get; set; }        // True nếu người dùng trả lời đúng
    }
}
