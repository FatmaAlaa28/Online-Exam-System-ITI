using ADB_Project.Models;

namespace ADB_Project.Models.ViewModels
{
    public class InstructorDashboardVM
    {
        // Stats
        public int TotalCourses { get; set; }
        public int TotalStudents { get; set; }
        public int TotalExams { get; set; }
        public decimal AverageGrade { get; set; } = 0;

        // Charts data
        public int ExamsCreated { get; set; }
        public int ExamsOngoing { get; set; }
        public int ExamsCompleted { get; set; }
        public int ExamsGraded { get; set; }

        public int ActiveCourses { get; set; }
        public int InactiveCourses { get; set; }

        // Recent Exams
        public List<Exam> RecentExams { get; set; } = new();

        // Courses list (if you want to show them)
        public List<Course> Courses { get; set; } = new();
    }
}