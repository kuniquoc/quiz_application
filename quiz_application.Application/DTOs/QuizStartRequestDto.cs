namespace quiz_application.Application.DTOs
{
    public class QuizStartRequestDto
    {
        public int QuizId { get; set; }
        public long ClientStartTimeTicks { get; set; } // Thời gian bắt đầu từ client (Unix Milliseconds)
    }
}
