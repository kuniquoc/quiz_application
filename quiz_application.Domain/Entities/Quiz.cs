using quiz_application.Domain.Enums; // Thêm using để sử dụng enum

namespace quiz_application.Domain.Entities
{
    public class Quiz
    {
        public int QuizId { get; set; }
        public string QuizName { get; set; } = null!; // Thêm null-forgiving operator
        public string Description { get; set; } = null!; // Thêm null-forgiving operator
        public decimal PassPercentage { get; set; }
        public int? TimeLimitSeconds { get; set; } // Nullable: Không có giới hạn thời gian nếu null
        public QuizQuestionOrderType QuestionOrderType { get; set; } // Loại thứ tự câu hỏi

        // Navigation properties
        public ICollection<Question> Questions { get; set; } = new List<Question>(); // Khởi tạo để tránh null
        public ICollection<UserQuizAttempt> UserQuizAttempts { get; set; } = new List<UserQuizAttempt>(); // THÊM DÒNG NÀY (để hỗ trợ cấu hình DBContext)
    }
}
