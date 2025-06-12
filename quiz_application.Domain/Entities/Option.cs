namespace quiz_application.Domain.Entities
{
    public class Option
    {
        public int OptionId { get; set; }
        public int QuestionId { get; set; } // Foreign Key
        public string OptionText { get; set; } = null!; // null-forgiving operator
        public bool IsCorrect { get; set; }

        // Navigation property
        public Question Question { get; set; } = null!; // null-forgiving operator
    }
}
