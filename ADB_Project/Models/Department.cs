using System;
using System.Collections.Generic;

namespace ADB_Project.Models;

public partial class Department
{
    public int DeptId { get; set; }

    public string DeptName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<BranchDepartment> BranchDepartments { get; set; } = new List<BranchDepartment>();

    public virtual ICollection<DepartmentCourse> DepartmentCourses { get; set; } = new List<DepartmentCourse>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
