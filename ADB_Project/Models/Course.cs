using System;
using System.Collections.Generic;

namespace ADB_Project.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = null!;

    public string? CourseCode { get; set; }

    public string? Description { get; set; }

    public int MaxDegree { get; set; }

    public int? CreditHours { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<CourseGrade> CourseGrades { get; set; } = new List<CourseGrade>();

    public virtual ICollection<DepartmentCourse> DepartmentCourses { get; set; } = new List<DepartmentCourse>();

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual ICollection<InstructorCourse> InstructorCourses { get; set; } = new List<InstructorCourse>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();

    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
