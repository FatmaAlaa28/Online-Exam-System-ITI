namespace ADB_Project.Models.ViewModels
{
    public class InstructorDashboardVM
    {
        public List<Course> Courses { get; set; } = new List<Course>();
        public List<Exam> Exams { get; set; } = new List<Exam>();
    }
}
