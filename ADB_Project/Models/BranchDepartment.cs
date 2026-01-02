using System;
using System.Collections.Generic;

namespace ADB_Project.Models;

public partial class BranchDepartment
{
    public int BranchId { get; set; }

    public int DeptId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual Department Dept { get; set; } = null!;
}
