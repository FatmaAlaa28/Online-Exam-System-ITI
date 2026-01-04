using Microsoft.AspNetCore.Mvc.Rendering;

namespace ADB_Project.Models.ViewModels
{
    public class AssignCourseViewModel
    {
        public int InstructorId { get; set; }
        public int CourseId { get; set; }
        public DateTime? AssignmentDate { get; set; }

        public List<SelectListItem> Instructors { get; set; } = new();
        public List<SelectListItem> Courses { get; set; } = new();
    }
}
