namespace quiz_application.Application.DTOs
{
    public class QuizStartResponseDto
    {
        public int AttemptId { get; set; } // ID của lần làm bài được tạo trên server
        public string QuizName { get; set; } = null!;
        public int? TimeLimitSeconds { get; set; } // Thời gian giới hạn cho Quiz
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>(); // Danh sách câu hỏi đã được sắp xếp/ngẫu nhiên
    }
}
