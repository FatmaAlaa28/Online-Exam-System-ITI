using System;
using System.Collections.Generic;

namespace ADB_Project.Models;

public partial class StudentCourse
{
    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public DateOnly? EnrollmentDate { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
