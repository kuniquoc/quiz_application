using quiz_application.Domain.Enums; // Added using directive for enum usage

namespace quiz_application.Domain.Entities
{
    public class Quiz
    {
        public int QuizId { get; set; }
        public string QuizName { get; set; } = null!; // Added null-forgiving operator
        public string Description { get; set; } = null!; // Added null-forgiving operator
        public decimal PassPercentage { get; set; }
        public int? TimeLimitSeconds { get; set; } // Nullable: No time limit if null
        public QuizQuestionOrderType QuestionOrderType { get; set; } // Type of question order        

        // Navigation properties
        public ICollection<Question> Questions { get; set; } = new List<Question>(); // Initialize to avoid null
        public ICollection<UserQuizAttempt> UserQuizAttempts { get; set; } = new List<UserQuizAttempt>(); // ADDED LINE (to support DBContext configuration)
    }
}
