using System;
using System.Collections.Generic;
using ADB_Project.Models;
using ADB_Project.Models.ADB_Project.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ADB_Project.Data;

public partial class OnlineExamDbContext : IdentityDbContext<AppUser>
{
    public OnlineExamDbContext()
    {
    }
public OnlineExamDbContext(DbContextOptions<OnlineExamDbContext> options)
            : base(options)
{
}

public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<BranchDepartment> BranchDepartments { get; set; }

    public virtual DbSet<Choice> Choices { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseGrade> CourseGrades { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<DepartmentCourse> DepartmentCourses { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<ExamAssignment> ExamAssignments { get; set; }

    public virtual DbSet<ExamGrade> ExamGrades { get; set; }

    public virtual DbSet<ExamQuestion> ExamQuestions { get; set; }

    public virtual DbSet<Instructor> Instructors { get; set; }

    public virtual DbSet<InstructorCourse> InstructorCourses { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentAnswer> StudentAnswers { get; set; }

    public virtual DbSet<StudentCourse> StudentCourses { get; set; }

    public virtual DbSet<StudentExam> StudentExams { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=OnlineExamSystemITI;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.BranchId).HasName("PK__Branch__A1682FA5E35EA97C");

            entity.ToTable("Branch");

            entity.HasIndex(e => e.BranchName, "UQ__Branch__3903DB037F088DD6").IsUnique();

            entity.Property(e => e.BranchId).HasColumnName("BranchID");
            entity.Property(e => e.BranchName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<BranchDepartment>(entity =>
        {
            entity.HasKey(e => new { e.BranchId, e.DeptId });

            entity.ToTable("BranchDepartment");

            entity.Property(e => e.BranchId).HasColumnName("BranchID");
            entity.Property(e => e.DeptId).HasColumnName("DeptID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Branch).WithMany(p => p.BranchDepartments)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BD_Branch");

            entity.HasOne(d => d.Dept).WithMany(p => p.BranchDepartments)
                .HasForeignKey(d => d.DeptId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BD_Department");
        });

        modelBuilder.Entity<Choice>(entity =>
        {
            entity.HasKey(e => e.ChoiceId).HasName("PK__Choice__76F51686A63E57FF");

            entity.ToTable("Choice");

            entity.Property(e => e.ChoiceId).HasColumnName("ChoiceID");
            entity.Property(e => e.ChoiceText)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsCorrect).HasDefaultValue(false);
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

            entity.HasOne(d => d.Question).WithMany(p => p.Choices)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Choice_Question");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Course__C92D7187E1C92149");

            entity.ToTable("Course");

            entity.HasIndex(e => e.CourseName, "UQ__Course__9526E277A0A3DC09").IsUnique();

            entity.HasIndex(e => e.CourseCode, "UQ__Course__FC00E00021384EA1").IsUnique();

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CourseName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<CourseGrade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourseGr__3213E83F57881123");

            entity.ToTable("CourseGrade");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.GradeDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LetterGrade)
                .HasMaxLength(2)
                .IsUnicode(false);
            entity.Property(e => e.Percentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseGrades)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CG_Course");

            entity.HasOne(d => d.Student).WithMany(p => p.CourseGrades)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CG_Student");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DeptId).HasName("PK__Departme__0148818ED7826F7A");

            entity.ToTable("Department");

            entity.HasIndex(e => e.DeptName, "UQ__Departme__5E508265EA7EBCF6").IsUnique();

            entity.Property(e => e.DeptId).HasColumnName("DeptID");
            entity.Property(e => e.DeptName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<DepartmentCourse>(entity =>
        {
            entity.HasKey(e => new { e.DeptId, e.CourseId });

            entity.ToTable("DepartmentCourse");

            entity.Property(e => e.DeptId).HasColumnName("DeptID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsRequired).HasDefaultValue(true);

            entity.HasOne(d => d.Course).WithMany(p => p.DepartmentCourses)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DC_Course");

            entity.HasOne(d => d.Dept).WithMany(p => p.DepartmentCourses)
                .HasForeignKey(d => d.DeptId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DC_Department");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.ExamId).HasName("PK__Exam__297521A788ACBA76");

            entity.ToTable("Exam");

            entity.HasIndex(e => e.CourseId, "IX_Exam_CourseID");

            entity.Property(e => e.ExamId).HasColumnName("ExamID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExamDate).HasColumnType("datetime");
            entity.Property(e => e.ExamName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ExamType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.IsRandomized).HasDefaultValue(true);

            entity.HasOne(d => d.Course).WithMany(p => p.Exams)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Exam_Course");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Exams)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Exam_Instructor");
        });

        modelBuilder.Entity<ExamAssignment>(entity =>
        {
            entity.HasKey(e => new { e.ExamId, e.StudentId });

            entity.ToTable("ExamAssignment");

            entity.HasIndex(e => e.ExamId, "IX_ExamAssignment_ExamID");

            entity.HasIndex(e => e.StudentId, "IX_ExamAssignment_StudentID");

            entity.Property(e => e.ExamId).HasColumnName("ExamID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.AssignedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Exam).WithMany(p => p.ExamAssignments)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EA_Exam");

            entity.HasOne(d => d.Student).WithMany(p => p.ExamAssignments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EA_Student");
        });

        modelBuilder.Entity<ExamGrade>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.ExamId });

            entity.ToTable("ExamGrade");

            entity.HasIndex(e => e.ExamId, "IX_ExamGrade_ExamID");

            entity.HasIndex(e => e.StudentId, "IX_ExamGrade_StudentID");

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.ExamId).HasColumnName("ExamID");
            entity.Property(e => e.Grade)
                .HasMaxLength(2)
                .IsUnicode(false);
            entity.Property(e => e.GradeDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Percentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.Exam).WithMany(p => p.ExamGrades)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EG_Exam");

            entity.HasOne(d => d.Student).WithMany(p => p.ExamGrades)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EG_Student");
        });

        modelBuilder.Entity<ExamQuestion>(entity =>
        {
            entity.HasKey(e => new { e.ExamId, e.QuestionId });

            entity.ToTable("ExamQuestion");

            entity.HasIndex(e => new { e.ExamId, e.QuestionOrder }, "UQ_ExamQuestion_Order").IsUnique();

            entity.Property(e => e.ExamId).HasColumnName("ExamID");
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

            entity.HasOne(d => d.Exam).WithMany(p => p.ExamQuestions)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EQ_Exam");

            entity.HasOne(d => d.Question).WithMany(p => p.ExamQuestions)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EQ_Question");
        });

        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.HasKey(e => e.InstructorId).HasName("PK__Instruct__9D010B7B1426B677");

            entity.ToTable("Instructor");

            entity.HasIndex(e => e.Email, "UQ__Instruct__A9D10534CCAA87F3").IsUnique();

            entity.Property(e => e.InstructorId).HasColumnName("InstructorID");
            entity.Property(e => e.BranchId).HasColumnName("BranchID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HireDate).HasDefaultValueSql("(CONVERT([date],getdate()))");
            entity.Property(e => e.InstructorName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Branch).WithMany(p => p.Instructors)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Instructor_Branch");
        });

        modelBuilder.Entity<InstructorCourse>(entity =>
        {
            entity.HasKey(e => new { e.InstructorId, e.CourseId });

            entity.ToTable("InstructorCourse");

            entity.HasIndex(e => e.CourseId, "IX_InstructorCourse_CourseID");

            entity.HasIndex(e => e.InstructorId, "IX_InstructorCourse_InstructorID");

            entity.Property(e => e.InstructorId).HasColumnName("InstructorID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.AssignmentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Course).WithMany(p => p.InstructorCourses)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_IC_Course");

            entity.HasOne(d => d.Instructor).WithMany(p => p.InstructorCourses)
                .HasForeignKey(d => d.InstructorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_IC_Instructor");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PK__Question__0DC06F8C2B2240B5");

            entity.ToTable("Question");

            entity.HasIndex(e => e.CourseId, "IX_Question_CourseID");

            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DifficultyLevel)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.QuestionText).IsUnicode(false);
            entity.Property(e => e.QuestionType)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.Course).WithMany(p => p.Questions)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Question_Course");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Student__32C52A793238ED0A");

            entity.ToTable("Student");

            entity.HasIndex(e => e.BranchId, "IX_Student_BranchID");

            entity.HasIndex(e => e.DeptId, "IX_Student_DeptID");

            entity.HasIndex(e => e.Email, "UQ__Student__A9D10534628BA46F").IsUnique();

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.BranchId).HasColumnName("BranchID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeptId).HasColumnName("DeptID");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EnrollmentDate).HasDefaultValueSql("(CONVERT([date],getdate()))");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StudentName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Branch).WithMany(p => p.Students)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Student_Branch");

            entity.HasOne(d => d.Dept).WithMany(p => p.Students)
                .HasForeignKey(d => d.DeptId)
                .HasConstraintName("FK_Student_Department");
        });

        modelBuilder.Entity<StudentAnswer>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.ExamId, e.QuestionId });

            entity.ToTable("StudentAnswer");

            entity.HasIndex(e => e.ExamId, "IX_StudentAnswer_ExamID");

            entity.HasIndex(e => e.StudentId, "IX_StudentAnswer_StudentID");

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.ExamId).HasColumnName("ExamID");
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.AnsweredDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EarnedPoints).HasDefaultValue(0);
            entity.Property(e => e.SelectedChoiceId).HasColumnName("SelectedChoiceID");

            entity.HasOne(d => d.Exam).WithMany(p => p.StudentAnswers)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SA_Exam");

            entity.HasOne(d => d.Question).WithMany(p => p.StudentAnswers)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SA_Question");

            entity.HasOne(d => d.SelectedChoice).WithMany(p => p.StudentAnswers)
                .HasForeignKey(d => d.SelectedChoiceId)
                .HasConstraintName("FK_SA_Choice");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentAnswers)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SA_Student");
        });

        modelBuilder.Entity<StudentCourse>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.CourseId });

            entity.ToTable("StudentCourse");

            entity.HasIndex(e => e.CourseId, "IX_StudentCourse_CourseID");

            entity.HasIndex(e => e.StudentId, "IX_StudentCourse_StudentID");

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EnrollmentDate).HasDefaultValueSql("(CONVERT([date],getdate()))");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Course).WithMany(p => p.StudentCourses)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SC_Course");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentCourses)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SC_Student");
        });

        modelBuilder.Entity<StudentExam>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.ExamId });

            entity.ToTable("StudentExam");

            entity.HasIndex(e => e.ExamId, "IX_StudentExam_ExamID");

            entity.HasIndex(e => e.StudentId, "IX_StudentExam_StudentID");

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.ExamId).HasColumnName("ExamID");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.IsCorrected).HasDefaultValue(false);
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.SubmittedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Exam).WithMany(p => p.StudentExams)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SE_Exam");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentExams)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SE_Student");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.TopicId).HasName("PK__Topic__022E0F7DA006ABB4");

            entity.ToTable("Topic");

            entity.HasIndex(e => e.CourseId, "IX_Topic_CourseID");

            entity.HasIndex(e => new { e.TopicName, e.CourseId }, "UQ_Topic_Course").IsUnique();

            entity.Property(e => e.TopicId).HasColumnName("TopicID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.TopicName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Course).WithMany(p => p.Topics)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Topic_Course");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
