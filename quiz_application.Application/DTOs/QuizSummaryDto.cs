namespace quiz_application.Application.DTOs
{
    public class QuizSummaryDto
    {
        public int QuizId { get; set; }
        public string QuizName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal PassPercentage { get; set; }
        public int? TimeLimitSeconds { get; set; }
        public int TotalQuestions { get; set; }
    }
}
