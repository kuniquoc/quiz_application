namespace quiz_application.Domain.Entities
{
    public class UserQuizAttempt
    {
        public int AttemptId { get; set; }
        public int QuizId { get; set; } // Foreign Key        
        public DateTime ClientStartTime { get; set; } // Quiz start time (UTC), sent from client
        public DateTime ClientEndTime { get; set; }   // Quiz end time (UTC), sent from client
        public int Score { get; set; }               // Total number of correct answers
        public bool IsPassed { get; set; }            // Pass/Fail result        

        // Navigation properties
        public Quiz Quiz { get; set; } = null!; // null-forgiving operator
        public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>(); // Initialize to avoid null
    }
}
