using System.ComponentModel.DataAnnotations;
namespace ADB_Project.Models.ViewModels
{
        // للـ Dashboard - عرض الامتحانات المتاحة
        public class AvailableExamViewModel
        {
            public int ExamId { get; set; }
            public string ExamName { get; set; } = string.Empty;
            public string CourseName { get; set; } = string.Empty;
            public int TotalQuestions { get; set; }
            public int DurationMinutes { get; set; }
            public DateTime? ExamDate { get; set; }
            public DateTime? DueDate { get; set; }
            public bool HasStarted { get; set; }
            public bool HasSubmitted { get; set; }
            public int? Score { get; set; }
        }

        // للـ Take Exam Page - Session الامتحان
        public class ExamSessionViewModel
        {
            public int ExamId { get; set; }
            public string ExamName { get; set; } = string.Empty;
            public string CourseName { get; set; } = string.Empty;
            public int TotalQuestions { get; set; }
            public int TotalDegree { get; set; }
            public int DurationMinutes { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public int CurrentQuestionIndex { get; set; } = 0;

            public List<ExamQuestionViewModel> Questions { get; set; } = new();

            // Dictionary: QuestionID -> SelectedChoiceID
            public Dictionary<int, int?> StudentAnswers { get; set; } = new();

            // للـ Quick Navigation
            public HashSet<int> MarkedQuestions { get; set; } = new();
        }

        // سؤال واحد في الامتحان
        public class ExamQuestionViewModel
        {
            public int QuestionId { get; set; }
            public string QuestionText { get; set; } = string.Empty;
            public string QuestionType { get; set; } = string.Empty; // MCQ, TF
            public int Points { get; set; }
            public int QuestionOrder { get; set; }

            public List<ChoiceViewModel> Choices { get; set; } = new();
        }

        // اختيار واحد
        public class ChoiceViewModel
        {
            public int ChoiceId { get; set; }
            public string ChoiceText { get; set; } = string.Empty;
        }

        // للـ Submit
        public class ExamSubmissionViewModel
        {
            public int ExamId { get; set; }
            public int StudentId { get; set; }
            public Dictionary<int, int?> Answers { get; set; } = new();
        }

        // للـ Results Page
        public class ExamResultViewModel
        {
            public int ExamId { get; set; }
            public string ExamName { get; set; } = string.Empty;
            public string CourseName { get; set; } = string.Empty;
            public int TotalScore { get; set; }
            public int MaxScore { get; set; }
            public decimal Percentage { get; set; }
            public string Grade { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public DateTime? SubmittedDate { get; set; }

            public List<QuestionResultViewModel> Questions { get; set; } = new();
        }

        // نتيجة سؤال واحد
        public class QuestionResultViewModel
        {
            public int QuestionOrder { get; set; }
            public string QuestionText { get; set; } = string.Empty;
            public int Points { get; set; }
            public int EarnedPoints { get; set; }
            public bool? IsCorrect { get; set; }

            public string? StudentAnswer { get; set; }
            public string? CorrectAnswer { get; set; }

            public List<ChoiceResultViewModel> Choices { get; set; } = new();
        }

        public class ChoiceResultViewModel
        {
            public int ChoiceId { get; set; }
            public string ChoiceText { get; set; } = string.Empty;
            public bool IsCorrect { get; set; }
            public bool IsStudentChoice { get; set; }
        }


    
}
