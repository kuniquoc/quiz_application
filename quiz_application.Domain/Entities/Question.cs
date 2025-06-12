namespace quiz_application.Domain.Entities
{
    public class Question
    {
        public int QuestionId { get; set; }
        public int QuizId { get; set; } // Foreign Key
        public string QuestionText { get; set; } = null!; // Thêm null-forgiving operator
        public string? QuestionImage { get; set; } // Nullable, URL hoặc path tới ảnh
        public int OrderInQuiz { get; set; } // Thứ tự mặc định của câu hỏi trong Quiz (dùng khi Sequential)

        // Navigation properties
        public Quiz Quiz { get; set; } = null!; // null-forgiving operator
        public ICollection<Option> Options { get; set; } = new List<Option>(); // Khởi tạo để tránh null
    }
}
