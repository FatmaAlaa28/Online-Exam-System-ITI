using System;
using System.Collections.Generic;

namespace ADB_Project.Models;

public partial class DepartmentCourse
{
    public int DeptId { get; set; }

    public int CourseId { get; set; }

    public bool? IsRequired { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Department Dept { get; set; } = null!;
}
