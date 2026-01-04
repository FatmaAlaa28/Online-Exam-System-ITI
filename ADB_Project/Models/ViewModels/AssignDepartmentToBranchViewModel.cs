// ViewModels/AssignDepartmentToBranchViewModel.cs
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ADB_Project.Models.ViewModels
{
    public class AssignDepartmentToBranchViewModel
    {
        public int BranchId { get; set; }
        public int DeptId { get; set; }
        public DateTime? AssignmentDate { get; set; } = DateTime.Now;

        public List<SelectListItem> Branches { get; set; } = new();
        public List<SelectListItem> Departments { get; set; } = new();
    }
}