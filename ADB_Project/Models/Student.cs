using System;
using System.Collections.Generic;

namespace ADB_Project.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string StudentName { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public int? DeptId { get; set; }

    public int? BranchId { get; set; }

    public DateOnly? EnrollmentDate { get; set; }

    public DateOnly? GraduationDate { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual Branch? Branch { get; set; }

    public virtual ICollection<CourseGrade> CourseGrades { get; set; } = new List<CourseGrade>();

    public virtual Department? Dept { get; set; }

    public virtual ICollection<ExamAssignment> ExamAssignments { get; set; } = new List<ExamAssignment>();

    public virtual ICollection<ExamGrade> ExamGrades { get; set; } = new List<ExamGrade>();

    public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();

    public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();

    public virtual ICollection<StudentExam> StudentExams { get; set; } = new List<StudentExam>();
}
