// Models/ViewModels/AdminDashboardViewModel.cs
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ADB_Project.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        // Basic Counts
        public int TotalStudents { get; set; }
        public int TotalInstructors { get; set; }
        public int TotalCourses { get; set; }
        public int TotalExams { get; set; }
        public int ActiveBranches { get; set; }
        public int ActiveDepartments { get; set; }

        // Advanced Statistics
        public int PendingExams { get; set; }
        public int CompletedExams { get; set; }
        public int TotalQuestions { get; set; }
        public decimal AverageExamScore { get; set; }
        public int StudentsEnrolledThisMonth { get; set; }
        public int ExamsCreatedThisMonth { get; set; }

        // Lists for Charts
        public List<DepartmentStatistic> DepartmentStatistics { get; set; } = new();
        public List<ExamStatistic> RecentExams { get; set; } = new();
        public List<MonthlyEnrollment> MonthlyEnrollments { get; set; } = new();
        public List<TopStudent> TopPerformingStudents { get; set; } = new();
        public List<CoursePopularity> PopularCourses { get; set; } = new();
    }

    public class DepartmentStatistic
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public int CourseCount { get; set; }
    }

    public class ExamStatistic
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public DateTime? ExamDate { get; set; }
        public int ParticipantCount { get; set; }
        public decimal AverageScore { get; set; }
    }

    public class MonthlyEnrollment
    {
        public string Month { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class TopStudent
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public decimal AverageGrade { get; set; }
        public int CompletedExams { get; set; }
    }

    public class CoursePopularity
    {
        public string CourseName { get; set; } = string.Empty;
        public int EnrolledStudents { get; set; }
        public int TotalExams { get; set; }
    }

    public class AssignDepartmentToBranchViewModel
    {
        public int BranchId { get; set; }
        public int DeptId { get; set; }
        public List<SelectListItem> Branches { get; set; } = new();
        public List<SelectListItem> Departments { get; set; } = new();
    }
}