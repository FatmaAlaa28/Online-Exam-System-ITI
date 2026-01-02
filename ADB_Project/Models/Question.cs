using System;
using System.Collections.Generic;

namespace ADB_Project.Models;

public partial class Question
{
    public int QuestionId { get; set; }

    public string QuestionText { get; set; } = null!;

    public string? QuestionType { get; set; }

    public int? Points { get; set; }

    public int CourseId { get; set; }

    public string? DifficultyLevel { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<Choice> Choices { get; set; } = new List<Choice>();

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();

    public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
}
