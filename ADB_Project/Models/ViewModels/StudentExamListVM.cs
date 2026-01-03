namespace ADB_Project.Models.ViewModels
{
    public class StudentExamListVM
    {
        public string ExamAssignmentId { get; set; } = string.Empty;
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime? ExamDate { get; set; }
        public DateTime? DueDate { get; set; }
        public bool HasStarted { get; set; }
        public bool HasSubmitted { get; set; }
        public bool HasGrade { get; set; }
    }
}