using System;
using System.Collections.Generic;

namespace ADB_Project.Models;

public partial class ExamAssignment
{
    public int ExamId { get; set; }

    public int StudentId { get; set; }

    public DateTime? AssignedDate { get; set; }

    public DateTime? DueDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual Exam Exam { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
