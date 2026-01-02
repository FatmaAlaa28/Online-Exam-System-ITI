using System;
using System.Collections.Generic;

namespace ADB_Project.Models;

public partial class Choice
{
    public int ChoiceId { get; set; }

    public string ChoiceText { get; set; } = null!;

    public int QuestionId { get; set; }

    public bool? IsCorrect { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Question Question { get; set; } = null!;

    public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
}
