namespace quiz_application.Domain.Entities
{
    public class UserAnswer
    {
        public int AttemptId { get; set; }    // Foreign Key and part of the primary key
        public int QuestionId { get; set; }   // Foreign Key and part of the primary key
        public int? SelectedOptionId { get; set; } // Foreign Key, nullable: user may not select an answer
        public bool IsCorrect { get; set; }      // Right/wrong confirmation from server        

        // Navigation properties
        public UserQuizAttempt Attempt { get; set; } = null!; // null-forgiving operator
        public Question Question { get; set; } = null!;     // null-forgiving operator
        public Option? SelectedOption { get; set; } // Nullable, if SelectedOptionId is null
    }
}
