using System;
using System.Collections.Generic;

namespace ADB_Project.Models;

public partial class Branch
{
    public int BranchId { get; set; }

    public string BranchName { get; set; } = null!;

    public string? Location { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<BranchDepartment> BranchDepartments { get; set; } = new List<BranchDepartment>();

    public virtual ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
