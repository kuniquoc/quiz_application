namespace quiz_application.Domain.Entities
{
    public class UserQuizAttempt
    {
        public int AttemptId { get; set; }
        public int QuizId { get; set; } // Foreign Key
        public DateTime ClientStartTime { get; set; } // Thời điểm bắt đầu Quiz (UTC), gửi từ client
        public DateTime ClientEndTime { get; set; }   // Thời điểm kết thúc Quiz (UTC), gửi từ client
        public int Score { get; set; }               // Tổng số câu trả lời đúng
        public bool IsPassed { get; set; }            // Kết quả Đỗ/Trượt

        // Navigation properties
        public Quiz Quiz { get; set; } = null!; // null-forgiving operator
        public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>(); // Khởi tạo để tránh null
    }
}
