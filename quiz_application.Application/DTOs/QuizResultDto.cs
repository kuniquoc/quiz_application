namespace quiz_application.Application.DTOs
{
    public class QuizResultDto
    {
        public double TotalTimeTakenSeconds { get; set; } // Total time spent on the quiz
        public int CorrectAnswersCount { get; set; }       // Number of correct answers
        public int IncorrectAnswersCount { get; set; }     // Number of incorrect answers
        public bool IsPassed { get; set; }                  // Pass/Fail result
        public decimal PassPercentageRequired { get; set; } // Percentage needed to pass
        public int? TimeLimitSeconds { get; set; }        // Time limit of the Quiz
        public List<ReviewQuestionDto> ReviewQuestions { get; set; } = new List<ReviewQuestionDto>(); // List of questions for review
    }
}
