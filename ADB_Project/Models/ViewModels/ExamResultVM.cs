namespace ADB_Project.Models.ViewModels
{
    public class ExamResultVM
    {
        public string ExamName { get; set; } = "";
        public string CourseName { get; set; } = "";
        public int TotalScore { get; set; }
        public int MaxScore { get; set; }
        public decimal Percentage { get; set; }
        public string Grade { get; set; } = "";
        public string Status { get; set; } = "";
        public List<FormattedAnswerVM> FormattedAnswers { get; set; } = new();
    }
}