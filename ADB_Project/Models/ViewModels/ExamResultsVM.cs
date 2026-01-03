namespace ADB_Project.Models.ViewModels
{
    public class ExamResultsVM
    {
        public Exam Exam { get; set; } = null!;

        // Students who completed the exam with grades
        public List<ExamGrade> ExamGrades { get; set; } = new List<ExamGrade>();

        // Students assigned but haven't taken/submitted
        public List<Student> NotTakenStudents { get; set; } = new List<Student>();

        // Summary Stats
        public int TotalAssigned { get; set; }
        public int Completed { get; set; }
        public int NotTaken { get; set; }
        public double AveragePercentage { get; set; }
        public double PassRate { get; set; }
        public int PassedCount { get; set; }
        public int FailedCount { get; set; }

        // Hardest questions
        public List<QuestionStatVM> QuestionStats { get; set; } = new List<QuestionStatVM>();
    }
}
