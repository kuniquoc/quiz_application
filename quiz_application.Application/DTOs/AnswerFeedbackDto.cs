namespace quiz_application.Application.DTOs
{
    public class AnswerFeedbackDto
    {
        public bool IsCorrect { get; set; }         // Server confirmation of right/wrong
        public int? CorrectOptionId { get; set; }   // ID of the correct option (for client highlighting) - nullable
        public string? CorrectOptionText { get; set; } // Content of the correct option (for client display) - nullable
    }
}
