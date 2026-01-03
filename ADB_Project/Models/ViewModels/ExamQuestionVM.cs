namespace ADB_Project.Models.ViewModels
{
    public class ExamQuestionVM
    {
        public int QuestionId { get; set; }
        public int QuestionOrder { get; set; }
        public string QuestionText { get; set; } = "";
        public string QuestionType { get; set; } = "MCQ";
        public int Points { get; set; }
        public List<ChoiceVM> Choices { get; set; } = new();
        public int? SelectedChoiceId { get; set; } // for form binding
    }
}
