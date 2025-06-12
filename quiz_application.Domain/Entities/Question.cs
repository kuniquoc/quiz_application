namespace quiz_application.Domain.Entities
{
    public class Question
    {
        public int QuestionId { get; set; }
        public int QuizId { get; set; } // Foreign Key        
        public string QuestionText { get; set; } = null!; // Added null-forgiving operator
        public string? QuestionImage { get; set; } // Nullable, URL or path to image
        public int OrderInQuiz { get; set; } // Default order of question in Quiz (used when Sequential)        

        // Navigation properties
        public Quiz Quiz { get; set; } = null!; // null-forgiving operator
        public ICollection<Option> Options { get; set; } = new List<Option>(); // Initialize to avoid null
    }
}
