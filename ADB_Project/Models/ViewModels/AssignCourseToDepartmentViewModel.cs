using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ADB_Project.Models.ViewModels
{
    public class AssignCourseToDepartmentViewModel
    {
        [Required(ErrorMessage = "Please select a department")]
        public int DeptId { get; set; }

        [Required(ErrorMessage = "Please select a course")]
        public int CourseId { get; set; }

        public bool IsRequired { get; set; } = false;

        public List<SelectListItem> Departments { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Courses { get; set; } = new List<SelectListItem>();

    }
}
