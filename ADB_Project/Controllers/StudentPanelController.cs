using ADB_Project.Data;
using ADB_Project.Models;
using ADB_Project.Models.ADB_Project.Models;
using ADB_Project.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ADB_Project.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentPanelController : Controller
    {
        private readonly OnlineExamDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public StudentPanelController(OnlineExamDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<int?> GetCurrentStudentIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return null;

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Email == user.Email && s.IsActive == true);

            return student?.StudentId;
        }

        // ================ DASHBOARD ================
        public async Task<IActionResult> Dashboard()
        {
            var studentId = await GetCurrentStudentIdAsync();
            if (!studentId.HasValue) return Unauthorized();

            var enrolledCourses = await _context.StudentCourses
                .CountAsync(sc => sc.StudentId == studentId.Value);

            var assignedExams = await _context.ExamAssignments
                .CountAsync(ea => ea.StudentId == studentId.Value && ea.IsActive == true);

            var submittedExams = await _context.StudentExams
                .CountAsync(se => se.StudentId == studentId.Value && se.SubmittedDate != null);

            var passedExams = await _context.ExamGrades
                .CountAsync(eg => eg.StudentId == studentId.Value && eg.Status == "Pass");

            var model = new StudentDashboardVM
            {
                EnrolledCourses = enrolledCourses,
                AssignedExams = assignedExams,
                SubmittedExams = submittedExams,
                PassedExams = passedExams
            };

            return View(model);
        }

        // ================ AVAILABLE EXAMS ================
        public async Task<IActionResult> Exams()
        {
            var studentId = await GetCurrentStudentIdAsync();
            if (!studentId.HasValue) return Unauthorized();

            var assignments = await _context.ExamAssignments
                .Where(ea => ea.StudentId == studentId.Value && ea.IsActive == true)
                .Include(ea => ea.Exam)
                    .ThenInclude(e => e.Course)
                .Include(ea => ea.Exam.ExamGrades)
                .OrderBy(ea => ea.Exam.ExamDate)
                .ToListAsync();

            var model = assignments.Select(ea => new StudentExamListVM
            {
                ExamAssignmentId = ea.ExamId + "-" + ea.StudentId, // unique key
                ExamId = ea.ExamId,
                ExamName = ea.Exam.ExamName,
                CourseName = ea.Exam.Course.CourseName,
                DurationMinutes = ea.Exam.DurationMinutes ?? 60,
                TotalQuestions = ea.Exam.TotalQuestions ?? 0,
                ExamDate = ea.Exam.ExamDate,
                DueDate = ea.DueDate,
                HasStarted = _context.StudentExams.Any(se => se.StudentId == studentId && se.ExamId == ea.ExamId),
                HasSubmitted = _context.StudentExams.Any(se => se.StudentId == studentId && se.ExamId == ea.ExamId && se.SubmittedDate != null),
                HasGrade = ea.Exam.ExamGrades.Any(eg => eg.StudentId == studentId)
            }).ToList();

            return View(model);
        }

        // ================ START EXAM (Call SP) ================
        [HttpPost]
        public async Task<IActionResult> StartExam(int examId)
        {
            var studentId = await GetCurrentStudentIdAsync();
            if (!studentId.HasValue) return Unauthorized();

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.StartExam @StudentID, @ExamID",
                    new SqlParameter("@StudentID", studentId.Value),
                    new SqlParameter("@ExamID", examId));

                return RedirectToAction("TakeExam", new { examId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message.Contains("not assigned")
                    ? "This exam is not assigned to you."
                    : "Error starting exam: " + ex.Message;
                return RedirectToAction("Exams");
            }
        }

        // ================ TAKE EXAM PAGE ================
        public async Task<IActionResult> TakeExam(int examId)
        {
            var studentId = await GetCurrentStudentIdAsync();
            if (!studentId.HasValue) return Unauthorized();

            var assignment = await _context.ExamAssignments
                .Include(ea => ea.Exam)
                    .ThenInclude(e => e.Course)
                .Include(ea => ea.Exam.ExamQuestions)
                    .ThenInclude(eq => eq.Question)
                    .ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync(ea => ea.ExamId == examId && ea.StudentId == studentId.Value);

            if (assignment == null)
            {
                TempData["Error"] = "Exam not assigned or not found.";
                return RedirectToAction("Exams");
            }

            var studentExam = await _context.StudentExams
                .FirstOrDefaultAsync(se => se.ExamId == examId && se.StudentId == studentId.Value);

            if (studentExam?.SubmittedDate != null)
            {
                return RedirectToAction("ExamResult", new { examId });
            }

            var model = new TakeExamVM
            {
                ExamId = examId,
                ExamName = assignment.Exam.ExamName,
                CourseName = assignment.Exam.Course.CourseName,
                DurationMinutes = assignment.Exam.DurationMinutes ?? 60,
                StartTime = studentExam?.StartTime ?? DateTime.Now,
                Questions = assignment.Exam.ExamQuestions
                    .OrderBy(eq => eq.QuestionOrder)
                    .Select(eq => new ExamQuestionVM
                    {
                        QuestionId = eq.Question.QuestionId,
                        QuestionOrder = eq.QuestionOrder,
                        QuestionText = eq.Question.QuestionText,
                        QuestionType = eq.Question.QuestionType ?? "MCQ",
                        Points = eq.Question.Points ?? 1,
                        Choices = eq.Question.Choices.Select(c => new ChoiceVM
                        {
                            ChoiceId = c.ChoiceId,
                            ChoiceText = c.ChoiceText ?? ""
                        }).ToList()
                    }).ToList()
            };

            return View(model);
        }

        // ================ SUBMIT EXAM (Using JSON SP) ================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitExam(int ExamId, List<ExamQuestionVM> Questions)
        {
            var studentId = await GetCurrentStudentIdAsync();
            if (!studentId.HasValue) return Unauthorized();

            // Validate that exam is assigned and started
            var studentExam = await _context.StudentExams
                .FirstOrDefaultAsync(se => se.StudentId == studentId.Value && se.ExamId == ExamId);

            if (studentExam == null || studentExam.SubmittedDate != null)
            {
                TempData["Error"] = "Invalid exam state.";
                return RedirectToAction("Exams");
            }

            // Build JSON array: [{"QuestionID":1,"ChoiceID":5}, ...]
            var answers = new List<object>();
            foreach (var q in Questions)
            {
                if (q.SelectedChoiceId.HasValue && q.SelectedChoiceId.Value > 0)
                {
                    answers.Add(new
                    {
                        QuestionID = q.QuestionId,
                        ChoiceID = q.SelectedChoiceId.Value
                    });
                }
            }

            var json = System.Text.Json.JsonSerializer.Serialize(answers);

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.SubmitExamAnswers_JSON @StudentID, @ExamID, @AnswersJSON",
                    new SqlParameter("@StudentID", studentId.Value),
                    new SqlParameter("@ExamID", ExamId),
                    new SqlParameter("@AnswersJSON", json));

                // Update StudentExam as submitted
                studentExam.SubmittedDate = DateTime.Now;
                studentExam.EndTime = DateTime.Now;
                await _context.SaveChangesAsync();

                // Auto-correct the exam
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.ExamCorrection @ExamID",
                    new SqlParameter("@ExamID", ExamId));

                TempData["Success"] = "Exam submitted and graded successfully!";
                return RedirectToAction("ExamResult", new { examId = ExamId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error submitting exam: " + ex.Message;
                return RedirectToAction("TakeExam", new { examId = ExamId });
            }
        }

        // ================ EXAM RESULT PAGE ================
        public async Task<IActionResult> ExamResult(int examId)
        {
            var studentId = await GetCurrentStudentIdAsync();
            if (!studentId.HasValue) return Unauthorized();

            var grade = await _context.ExamGrades
                .Include(eg => eg.Exam)
                    .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(eg => eg.ExamId == examId && eg.StudentId == studentId.Value);

            if (grade == null)
            {
                TempData["Error"] = "Grade not available yet.";
                return RedirectToAction("Exams");
            }

            // Get formatted answers for review
            var parameters = new[]
            {
        new SqlParameter("@ExamID", examId),
        new SqlParameter("@StudentID", studentId.Value)
    };

            var formattedAnswers = await _context.Database
     .SqlQueryRaw<FormattedAnswerVM>("EXEC sp_ExamStudentAnswersFormatted @ExamID, @StudentID", parameters)
     .ToListAsync();

            var model = new ExamResultVM
            {
                ExamName = grade.Exam.ExamName,
                CourseName = grade.Exam.Course.CourseName,
                TotalScore = grade.TotalScore,
                MaxScore = grade.MaxScore,
                Percentage = grade.Percentage ?? 0,
                Grade = grade.Grade ?? "N/A",
                Status = grade.Status ?? "Pending",
                FormattedAnswers = formattedAnswers
            };

            return View(model);
        }
        // GET: /StudentPanel/MyCourses
        public async Task<IActionResult> MyCourses()
        {
            var studentId = await GetCurrentStudentIdAsync();
            if (studentId == null) return RedirectToAction("Login", "Account");

            var student = await _context.Students.FindAsync(studentId.Value);
            if (student == null) return NotFound();

            // Auto-enroll in all department courses if not already
            var deptCourses = await _context.DepartmentCourses
                .Where(dc => dc.DeptId == student.DeptId)
                .Select(dc => dc.CourseId)
                .ToListAsync();

            foreach (var courseId in deptCourses)
            {
                if (!await _context.StudentCourses.AnyAsync(sc => sc.StudentId == studentId.Value && sc.CourseId == courseId))
                {
                    _context.StudentCourses.Add(new StudentCourse
                    {
                        StudentId = studentId.Value,
                        CourseId = courseId,
                        EnrollmentDate = DateOnly.FromDateTime(DateTime.Now),
                        IsActive = true
                    });
                }
            }
            await _context.SaveChangesAsync();

            // Fetch enrolled courses
            var courses = await _context.StudentCourses
                .Where(sc => sc.StudentId == studentId.Value && sc.IsActive == true)
                .Include(sc => sc.Course)
                .Select(sc => sc.Course)
                .ToListAsync();

            return View(courses);
        }
    }
}