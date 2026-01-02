using System;
using System.Collections.Generic;

namespace ADB_Project.Models;

public partial class ExamGrade
{
    public int StudentId { get; set; }

    public int ExamId { get; set; }

    public int TotalScore { get; set; }

    public int MaxScore { get; set; }

    public decimal? Percentage { get; set; }

    public string? Grade { get; set; }

    public string? Status { get; set; }

    public DateTime? GradeDate { get; set; }

    public virtual Exam Exam { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
