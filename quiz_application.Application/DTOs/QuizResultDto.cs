namespace quiz_application.Application.DTOs
{
    public class QuizResultDto
    {
        public double TotalTimeTakenSeconds { get; set; } // Tổng thời gian làm bài
        public int CorrectAnswersCount { get; set; }       // Số câu trả lời đúng
        public int IncorrectAnswersCount { get; set; }     // Số câu trả lời sai
        public bool IsPassed { get; set; }                  // Kết quả Đỗ/Trượt
        public decimal PassPercentageRequired { get; set; } // Phần trăm điểm cần thiết để đỗ
        public int? TimeLimitSeconds { get; set; }        // Giới hạn thời gian của Quiz
        public List<ReviewQuestionDto> ReviewQuestions { get; set; } = new List<ReviewQuestionDto>(); // Danh sách câu hỏi để xem lại
    }
}
