namespace quiz_application.Application.DTOs
{
    public class QuizStartResponseDto
    {
        public int AttemptId { get; set; } // ID of the attempt created on the server
        public string QuizName { get; set; } = null!;
        public int? TimeLimitSeconds { get; set; } // Time limit for the Quiz
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>(); // List of questions already ordered/randomized
    }
}
