using System;
using System.Collections.Generic;

namespace ADB_Project.Models;

public partial class StudentExam
{
    public int StudentId { get; set; }

    public int ExamId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public DateTime? SubmittedDate { get; set; }

    public int? TotalScore { get; set; }

    public bool? IsCorrected { get; set; }

    public virtual Exam Exam { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
