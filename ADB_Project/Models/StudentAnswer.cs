using System;
using System.Collections.Generic;

namespace ADB_Project.Models;

public partial class StudentAnswer
{
    public int StudentId { get; set; }

    public int ExamId { get; set; }

    public int QuestionId { get; set; }

    public int? SelectedChoiceId { get; set; }

    public int QuestionPoints { get; set; }

    public int? EarnedPoints { get; set; }

    public bool? IsCorrect { get; set; }

    public DateTime? AnsweredDate { get; set; }

    public virtual Exam Exam { get; set; } = null!;

    public virtual Question Question { get; set; } = null!;

    public virtual Choice? SelectedChoice { get; set; }

    public virtual Student Student { get; set; } = null!;
}
