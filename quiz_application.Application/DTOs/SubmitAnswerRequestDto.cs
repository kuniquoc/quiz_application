namespace quiz_application.Application.DTOs
{
    public class SubmitAnswerRequestDto
    {
        public int AttemptId { get; set; }      // ID of the quiz attempt
        public int QuestionId { get; set; }     // ID of the answered question
        public int? SelectedOptionId { get; set; } // ID of the user's selected option (nullable)
    }
}
