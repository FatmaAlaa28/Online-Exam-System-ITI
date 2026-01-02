using System;
using System.Collections.Generic;

namespace ADB_Project.Models;

public partial class CourseGrade
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public int? TotalScore { get; set; }

    public int? MaxScore { get; set; }

    public decimal? Percentage { get; set; }

    public string? LetterGrade { get; set; }

    public string? Status { get; set; }

    public DateTime? GradeDate { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
