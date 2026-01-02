namespace ADB_Project.Models.ViewModels
{
        public class StudentDashboardViewModel
        {
            public string StudentName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string DepartmentName { get; set; } = string.Empty;
            public string BranchName { get; set; } = string.Empty;

            public int TotalCourses { get; set; }
            public int PendingExams { get; set; }
            public int CompletedExams { get; set; }
            public decimal AverageGrade { get; set; }

            public List<AvailableExamViewModel> UpcomingExams { get; set; } = new();
            public List<RecentGradeViewModel> RecentGrades { get; set; } = new();
        }

        public class RecentGradeViewModel
        {
            public string ExamName { get; set; } = string.Empty;
            public string CourseName { get; set; } = string.Empty;
            public int Score { get; set; }
            public int MaxScore { get; set; }
            public decimal Percentage { get; set; }
            public string Grade { get; set; } = string.Empty;
            public DateTime? GradeDate { get; set; }
        }

        public class InstructorDashboardViewModel
        {
            public string InstructorName { get; set; } = string.Empty;
            public int TotalCourses { get; set; }
            public int TotalStudents { get; set; }
            public int TotalExams { get; set; }
            public int PendingCorrections { get; set; }

            public List<CourseStatViewModel> Courses { get; set; } = new();
        }

        public class CourseStatViewModel
        {
            public int CourseId { get; set; }
            public string CourseName { get; set; } = string.Empty;
            public int StudentCount { get; set; }
            public int ExamCount { get; set; }
        }


   
}
