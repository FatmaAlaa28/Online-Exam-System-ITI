// ViewModels/AdminDashboardViewModel.cs
namespace ADB_Project.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalInstructors { get; set; }
        public int TotalCourses { get; set; }
        public int TotalExams { get; set; }
        public int ActiveBranches { get; set; }
        public int ActiveDepartments { get; set; }
    }
}