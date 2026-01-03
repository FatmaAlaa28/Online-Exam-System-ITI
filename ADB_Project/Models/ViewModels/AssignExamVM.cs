using Microsoft.AspNetCore.Mvc.Rendering;

namespace ADB_Project.Models.ViewModels
{
    public class AssignExamVM
    {
        public int ExamId { get; set; }
        public bool AssignByDepartment { get; set; }
        public int? DepartmentId { get; set; }
        public int[] SelectedStudentIds { get; set; } = new int[0];  // ✅ int[] not List<int>
        public DateTime? DueDate { get; set; }
        public MultiSelectList? Students { get; set; }
        public SelectList? Departments { get; set; }
    }
}
