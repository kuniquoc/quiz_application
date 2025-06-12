namespace quiz_application.Application.DTOs
{
    public class ReviewQuestionDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = null!;
        public string? YourAnswerText { get; set; } // User's answer
        public string? CorrectAnswerText { get; set; } // Correct answer
        public bool WasCorrect { get; set; }        // True if the user answered correctly
    }
}
