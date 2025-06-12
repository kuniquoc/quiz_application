namespace quiz_application.Domain.Entities
{
    public class UserAnswer
    {
        public int AttemptId { get; set; }    // Foreign Key và là một phần của khóa chính
        public int QuestionId { get; set; }   // Foreign Key và là một phần của khóa chính
        public int? SelectedOptionId { get; set; } // Foreign Key, nullable: người dùng có thể không chọn đáp án
        public bool IsCorrect { get; set; }      // Xác nhận đúng/sai từ server

        // Navigation properties
        public UserQuizAttempt Attempt { get; set; } = null!; // null-forgiving operator
        public Question Question { get; set; } = null!;     // null-forgiving operator
        public Option? SelectedOption { get; set; } // Nullable, nếu SelectedOptionId là null
    }
}
