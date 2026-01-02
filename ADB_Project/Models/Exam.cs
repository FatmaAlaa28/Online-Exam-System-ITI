using System;
using System.Collections.Generic;

namespace ADB_Project.Models;

public partial class Exam
{
    public int ExamId { get; set; }

    public int CourseId { get; set; }

    public string ExamName { get; set; } = null!;

    public string? ExamType { get; set; }

    public int? TotalDegree { get; set; }

    public int? TotalQuestions { get; set; }

    public int? DurationMinutes { get; set; }

    public int? PassingScore { get; set; }

    public DateTime? ExamDate { get; set; }

    public int? CreatedBy { get; set; }

    public bool? IsRandomized { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Instructor? CreatedByNavigation { get; set; }

    public virtual ICollection<ExamAssignment> ExamAssignments { get; set; } = new List<ExamAssignment>();

    public virtual ICollection<ExamGrade> ExamGrades { get; set; } = new List<ExamGrade>();

    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();

    public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();

    public virtual ICollection<StudentExam> StudentExams { get; set; } = new List<StudentExam>();
}
