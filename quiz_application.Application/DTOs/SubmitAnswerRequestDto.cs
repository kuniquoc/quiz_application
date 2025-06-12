namespace quiz_application.Application.DTOs
{
    public class SubmitAnswerRequestDto
    {
        public int AttemptId { get; set; }      // ID của lần làm bài
        public int QuestionId { get; set; }     // ID của câu hỏi đã trả lời
        public int? SelectedOptionId { get; set; } // ID của lựa chọn người dùng đã chọn (nullable)
    }
}
