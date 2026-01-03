namespace ADB_Project.Models.ViewModels
{
    public class QuestionStatVM
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int TotalAttempts { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public double CorrectPercentage { get; set; }
    }
}
