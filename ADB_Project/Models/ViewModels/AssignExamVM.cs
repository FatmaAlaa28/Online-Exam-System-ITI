using Microsoft.AspNetCore.Mvc.Rendering;

namespace ADB_Project.Models.ViewModels
{
    public class AssignExamVM
    {
        public int ExamId { get; set; }
        public string AssignmentMethod { get; set; } = "Manual"; 
        public int[] SelectedBranchIds { get; set; } = new int[0];
        public int[] SelectedDeptIds { get; set; } = new int[0];
        public int[] SelectedStudentIds { get; set; } = new int[0];
        public DateTime? DueDate { get; set; }

        public SelectList? Branches { get; set; }
        public SelectList? Departments { get; set; }
        public MultiSelectList? Students { get; set; }
    }
}