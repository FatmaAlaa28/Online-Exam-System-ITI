using System;
using System.Collections.Generic;

namespace ADB_Project.Models;

public partial class InstructorCourse
{
    public int InstructorId { get; set; }

    public int CourseId { get; set; }

    public DateTime? AssignmentDate { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Instructor Instructor { get; set; } = null!;
}
