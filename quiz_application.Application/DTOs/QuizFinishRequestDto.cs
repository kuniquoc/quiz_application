namespace quiz_application.Application.DTOs
{
    public class QuizFinishRequestDto
    {
        public long ClientEndTimeTicks { get; set; } // End time from client (Unix Milliseconds)
    }
}
