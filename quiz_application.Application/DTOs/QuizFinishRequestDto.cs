namespace quiz_application.Application.DTOs
{
    public class QuizFinishRequestDto
    {
        public long ClientEndTimeTicks { get; set; } // Thời gian kết thúc từ client (Unix Milliseconds)
    }
}
