namespace quiz_application.Application.DTOs
{
    public class QuestionDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = null!;
        public string? QuestionImage { get; set; } // Nullable
        public List<OptionDto> Options { get; set; } = new List<OptionDto>();
    }
}
