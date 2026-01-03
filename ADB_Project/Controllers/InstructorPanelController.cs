// Note: Place this in Controllers/InstructorPanelController.cs
// This is a new controller specifically for logged-in instructors.
// The existing InstructorController seems to be for admin management of instructors.
// I've moved/copied the GenerateExam actions here and adjusted for Instructor role.
using System.Linq;
using ADB_Project.Data;
using ADB_Project.Models;
using ADB_Project.Models.ADB_Project.Models; // Adjust if namespace is different
using ADB_Project.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace ADB_Project.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorPanelController : Controller
    {
        private readonly OnlineExamDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public InstructorPanelController(
            OnlineExamDbContext context,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ================= HELPER METHOD =================
        private async Task<int?> GetCurrentInstructorIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return null;

            var instructor = await _context.Instructors
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Email == user.Email);

            return instructor?.InstructorId;
        }

        // ================= DASHBOARD =================
        // Shows list of assigned courses with links to questions, generate exam, etc.
        public async Task<IActionResult> Dashboard()
        {
            var instructorId = await GetCurrentInstructorIdAsync();
            if (instructorId == null)
            {
                return Unauthorized("Instructor account not found.");
            }

            var courses = await _context.InstructorCourses
                .Where(ic => ic.InstructorId == instructorId.Value)
                .Include(ic => ic.Course)
                .Select(ic => ic.Course)
                .ToListAsync();

            // Also get list of exams created by this instructor for quick access
            var exams = await _context.Exams
                .Where(e => e.CreatedBy == instructorId.Value)
                .ToListAsync();

            var viewModel = new InstructorDashboardVM
            {
                Courses = courses,
                Exams = exams
            };

            return View(viewModel);
        }

        // ================= COURSES =================
        // Not needed since dashboard lists them, but if you want a separate view
        public async Task<IActionResult> MyCourses()
        {
            var instructorId = await GetCurrentInstructorIdAsync();
            if (instructorId == null) return Unauthorized();

            var courses = await _context.InstructorCourses
                .Where(ic => ic.InstructorId == instructorId.Value)
                .Include(ic => ic.Course)
                .Select(ic => ic.Course)
                .ToListAsync();

            return View(courses);
        }

        // ================= QUESTIONS CRUD =================
        // List Questions for a Course
        public async Task<IActionResult> Questions(int courseId)
        {
            var instructorId = await GetCurrentInstructorIdAsync();
            if (instructorId == null) return Unauthorized();

            // Check if instructor teaches this course
            if (!await _context.InstructorCourses.AnyAsync(ic => ic.InstructorId == instructorId.Value && ic.CourseId == courseId))
            {
                return Unauthorized("You do not teach this course.");
            }

            var questions = await _context.Questions
                .Where(q => q.CourseId == courseId && q.IsActive == true)
                .Include(q => q.Choices)
                .ToListAsync();

            ViewBag.CourseId = courseId;
            ViewBag.CourseName = (await _context.Courses.FindAsync(courseId))?.CourseName;

            return View(questions);
        }

        // Create MCQ Question (Separate action for MCQ to simplify form without JS)
        [HttpGet]
        public async Task<IActionResult> CreateMCQ(int courseId)
        {
            var instructorId = await GetCurrentInstructorIdAsync();
            if (instructorId == null || !await _context.InstructorCourses.AnyAsync(ic => ic.InstructorId == instructorId.Value && ic.CourseId == courseId))
                return Unauthorized();

            var model = new CreateMCQVM { CourseId = courseId };
            ViewBag.Difficulties = new SelectList(new[] { "Easy", "Medium", "Hard" });
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMCQ(CreateMCQVM model)
        {
            var instructorId = await GetCurrentInstructorIdAsync();
            if (instructorId == null || !await _context.InstructorCourses.AnyAsync(ic => ic.InstructorId == instructorId.Value && ic.CourseId == model.CourseId))
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                ViewBag.Difficulties = new SelectList(new[] { "Easy", "Medium", "Hard" });
                return View(model);
            }

            var question = new Question
            {
                QuestionText = model.QuestionText,
                QuestionType = "MCQ",
                Points = model.Points,
                DifficultyLevel = model.DifficultyLevel,
                CourseId = model.CourseId,
                IsActive = true,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            // Add choices
            var choices = new[]
            {
                new Choice { ChoiceText = model.Choice1, IsCorrect = model.CorrectChoice == 1, QuestionId = question.QuestionId, CreatedDate = DateTime.Now },
                new Choice { ChoiceText = model.Choice2, IsCorrect = model.CorrectChoice == 2, QuestionId = question.QuestionId, CreatedDate = DateTime.Now },
                new Choice { ChoiceText = model.Choice3, IsCorrect = model.CorrectChoice == 3, QuestionId = question.QuestionId, CreatedDate = DateTime.Now },
                new Choice { ChoiceText = model.Choice4, IsCorrect = model.CorrectChoice == 4, QuestionId = question.QuestionId, CreatedDate = DateTime.Now }
            };

            if (choices.Count(c => c.IsCorrect == true) != 1)
            {
                ModelState.AddModelError("", "Exactly one correct choice must be selected.");
                ViewBag.Difficulties = new SelectList(new[] { "Easy", "Medium", "Hard" });
                return View(model);
            }

            _context.Choices.AddRange(choices);
            await _context.SaveChangesAsync();

            TempData["Success"] = "MCQ Question created successfully!";
            return RedirectToAction(nameof(Questions), new { courseId = model.CourseId });
        }

        // Create TF Question
        [HttpGet]
        public async Task<IActionResult> CreateTF(int courseId)
        {
            var instructorId = await GetCurrentInstructorIdAsync();
            if (instructorId == null || !await _context.InstructorCourses.AnyAsync(ic => ic.InstructorId == instructorId.Value && ic.CourseId == courseId))
                return Unauthorized();

            var model = new CreateTFVM { CourseId = courseId };
            ViewBag.Difficulties = new SelectList(new[] { "Easy", "Medium", "Hard" });
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTF(CreateTFVM model)
        {
            var instructorId = await GetCurrentInstructorIdAsync();
            if (instructorId == null || !await _context.InstructorCourses.AnyAsync(ic => ic.InstructorId == instructorId.Value && ic.CourseId == model.CourseId))
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                ViewBag.Difficulties = new SelectList(new[] { "Easy", "Medium", "Hard" });
                return View(model);
            }

            var question = new Question
            {
                QuestionText = model.QuestionText,
                QuestionType = "TF",
                Points = model.Points,
                DifficultyLevel = model.DifficultyLevel,
                CourseId = model.CourseId,
                IsActive = true,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            // Add fixed True/False choices
            _context.Choices.Add(new Choice { ChoiceText = "True", IsCorrect = model.IsTrueCorrect, QuestionId = question.QuestionId, CreatedDate = DateTime.Now });
            _context.Choices.Add(new Choice { ChoiceText = "False", IsCorrect = !model.IsTrueCorrect, QuestionId = question.QuestionId, CreatedDate = DateTime.Now });

            await _context.SaveChangesAsync();

            TempData["Success"] = "True/False Question created successfully!";
            return RedirectToAction(nameof(Questions), new { courseId = model.CourseId });
        }

        // Edit Question (Handles both MCQ and TF)
        [HttpGet]
        public async Task<IActionResult> EditQuestion(int id)
        {
            var question = await _context.Questions
                .Include(q => q.Choices)
                .Include(q => q.Course)
                .FirstOrDefaultAsync(q => q.QuestionId == id);

            if (question == null) return NotFound();

            var instructorId = await GetCurrentInstructorIdAsync();
            if (instructorId == null || !await _context.InstructorCourses.AnyAsync(ic => ic.InstructorId == instructorId.Value && ic.CourseId == question.CourseId))
                return Unauthorized();

            if (question.QuestionType == "MCQ")
            {
                var model = new CreateMCQVM
                {
                    QuestionId = question.QuestionId,
                    QuestionText = question.QuestionText,
                    Points = question.Points ?? 0,
                    DifficultyLevel = question.DifficultyLevel,
                    CourseId = question.CourseId,
                    Choice1 = question.Choices.ElementAtOrDefault(0)?.ChoiceText,
                    Choice2 = question.Choices.ElementAtOrDefault(1)?.ChoiceText,
                    Choice3 = question.Choices.ElementAtOrDefault(2)?.ChoiceText,
                    Choice4 = question.Choices.ElementAtOrDefault(3)?.ChoiceText,
                    // CORRECT
                    CorrectChoice = question.Choices.FirstOrDefault(c => c.IsCorrect == true) != null
    ? Array.IndexOf(question.Choices.Select(c => c.IsCorrect).ToArray(), true) + 1
    : 1
                };
                ViewBag.Difficulties = new SelectList(new[] { "Easy", "Medium", "Hard" }, model.DifficultyLevel);
                return View("EditMCQ", model);
            }
            else if (question.QuestionType == "TF")
            {
                var model = new CreateTFVM
                {
                    QuestionId = question.QuestionId,
                    QuestionText = question.QuestionText,
                    Points = question.Points ?? 0,
                    DifficultyLevel = question.DifficultyLevel,
                    CourseId = question.CourseId,
                    IsTrueCorrect = question.Choices.FirstOrDefault(c => c.ChoiceText == "True")?.IsCorrect ?? false
                };
                ViewBag.Difficulties = new SelectList(new[] { "Easy", "Medium", "Hard" }, model.DifficultyLevel);
                return View("EditTF", model);
            }

            return BadRequest("Unsupported question type.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMCQ(CreateMCQVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Difficulties = new SelectList(new[] { "Easy", "Medium", "Hard" });
                return View(model);
            }

            var question = await _context.Questions
                .Include(q => q.Choices)
                .FirstOrDefaultAsync(q => q.QuestionId == model.QuestionId);

            if (question == null) return NotFound();

            var instructorId = await GetCurrentInstructorIdAsync();
            if (instructorId == null || !await _context.InstructorCourses.AnyAsync(ic => ic.InstructorId == instructorId.Value && ic.CourseId == question.CourseId))
                return Unauthorized();

            // Update question
            question.QuestionText = model.QuestionText;
            question.Points = model.Points;
            question.DifficultyLevel = model.DifficultyLevel;
            question.ModifiedDate = DateTime.Now;

            // Update choices
            var choices = question.Choices.ToList();
            if (choices.Count != 4) return BadRequest("Invalid number of choices for MCQ.");

            choices[0].ChoiceText = model.Choice1;
            choices[0].IsCorrect = model.CorrectChoice == 1;
            choices[1].ChoiceText = model.Choice2;
            choices[1].IsCorrect = model.CorrectChoice == 2;
            choices[2].ChoiceText = model.Choice3;
            choices[2].IsCorrect = model.CorrectChoice == 3;
            choices[3].ChoiceText = model.Choice4;
            choices[3].IsCorrect = model.CorrectChoice == 4;

            if (choices.Count(c => c.IsCorrect == true) != 1)
            {
                ModelState.AddModelError("", "Exactly one correct choice must be selected.");
                ViewBag.Difficulties = new SelectList(new[] { "Easy", "Medium", "Hard" });
                return View(model);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "MCQ Question updated successfully!";
            return RedirectToAction(nameof(Questions), new { courseId = question.CourseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTF(CreateTFVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Difficulties = new SelectList(new[] { "Easy", "Medium", "Hard" });
                return View(model);
            }

            var question = await _context.Questions
                .Include(q => q.Choices)
                .FirstOrDefaultAsync(q => q.QuestionId == model.QuestionId);

            if (question == null) return NotFound();

            var instructorId = await GetCurrentInstructorIdAsync();
            if (instructorId == null || !await _context.InstructorCourses.AnyAsync(ic => ic.InstructorId == instructorId.Value && ic.CourseId == question.CourseId))
                return Unauthorized();

            // Update question
            question.QuestionText = model.QuestionText;
            question.Points = model.Points;
            question.DifficultyLevel = model.DifficultyLevel;
            question.ModifiedDate = DateTime.Now;

            // Update choices (fixed True/False)
            var trueChoice = question.Choices.FirstOrDefault(c => c.ChoiceText == "True");
            var falseChoice = question.Choices.FirstOrDefault(c => c.ChoiceText == "False");

            if (trueChoice == null || falseChoice == null) return BadRequest("Invalid choices for TF question.");

            trueChoice.IsCorrect = model.IsTrueCorrect;
            falseChoice.IsCorrect = !model.IsTrueCorrect;

            await _context.SaveChangesAsync();

            TempData["Success"] = "True/False Question updated successfully!";
            return RedirectToAction(nameof(Questions), new { courseId = question.CourseId });
        }

        // Delete Question
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _context.Questions
                .Include(q => q.Choices)
                .FirstOrDefaultAsync(q => q.QuestionId == id);

            if (question == null) return NotFound();

            var instructorId = await GetCurrentInstructorIdAsync();
            if (instructorId == null || !await _context.InstructorCourses.AnyAsync(ic => ic.InstructorId == instructorId.Value && ic.CourseId == question.CourseId))
                return Unauthorized();

            // Delete choices first (if no cascade)
            _context.Choices.RemoveRange(question.Choices);

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Question deleted successfully!";
            return RedirectToAction(nameof(Questions), new { courseId = question.CourseId });
        }

        // ================= GENERATE EXAM =================
        // (Moved from InstructorController and adjusted)
        [HttpGet]
        public async Task<IActionResult> GenerateExam()
        {
            var instructorId = await GetCurrentInstructorIdAsync();
            if (instructorId == null)
            {
                return Unauthorized("Instructor account not found.");
            }

            var courses = await _context.InstructorCourses
                .Where(ic => ic.InstructorId == instructorId.Value)
                .Include(ic => ic.Course)
                .Select(ic => ic.Course)
                .ToListAsync();

            var viewModel = new GenerateExamViewModel
            {
                Courses = new SelectList(courses, "CourseId", "CourseName")
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateExam(GenerateExamViewModel model)
        {
            var instructorId = await GetCurrentInstructorIdAsync();
            if (instructorId == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                var courses = await _context.InstructorCourses
                    .Where(ic => ic.InstructorId == instructorId.Value)
                    .Include(ic => ic.Course)
                    .Select(ic => ic.Course)
                    .ToListAsync();
                model.Courses = new SelectList(courses, "CourseId", "CourseName");
                return View(model);
            }

            var parameters = new[]
            {
                new SqlParameter("@CourseID", model.CourseId),
                new SqlParameter("@ExamName", model.ExamName),
                new SqlParameter("@NumMCQ", model.NumMCQ),
                new SqlParameter("@NumTF", model.NumTF),
                new SqlParameter("@InstructorID", instructorId.Value),
                new SqlParameter("@ExamDate", (object)model.ExamDate ?? DBNull.Value),
                new SqlParameter("@DurationMinutes", model.DurationMinutes),
                new SqlParameter("@PassingPercentage", model.PassingPercentage),
                new SqlParameter("@NewExamID", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC SP_GenerateExam @CourseID, @ExamName, @NumMCQ, @NumTF, @InstructorID, @ExamDate, @DurationMinutes, @PassingPercentage, @NewExamID OUTPUT",
                    parameters);

                var newExamId = (int)parameters.Last().Value;
                TempData["Success"] = $"Exam '{model.ExamName}' generated successfully with ID: {newExamId}!";

                return RedirectToAction("ExamDetails", new { examId = newExamId });
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError(string.Empty, $"Database Error: {ex.Message}");

                var courses = await _context.InstructorCourses
                    .Where(ic => ic.InstructorId == instructorId.Value)
                    .Include(ic => ic.Course)
                    .Select(ic => ic.Course)
                    .ToListAsync();
                model.Courses = new SelectList(courses, "CourseId", "CourseName");
                return View(model);
            }
        }

        // ================= MY EXAMS =================
        public async Task<IActionResult> MyExams()
        {
            var instructorId = await GetCurrentInstructorIdAsync();
            if (instructorId == null) return Unauthorized();

            var exams = await _context.Exams
                .Where(e => e.CreatedBy == instructorId.Value)
                .Include(e => e.Course)
                .ToListAsync();

            return View(exams);
        }

        // ================= EXAM DETAILS =================
        public async Task<IActionResult> ExamDetails(int examId)
        {
            var instructorId = await GetCurrentInstructorIdAsync();
            if (instructorId == null) return Unauthorized();

            var exam = await _context.Exams
                .Include(e => e.Course)
                .Include(e => e.ExamQuestions).ThenInclude(eq => eq.Question).ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync(e => e.ExamId == examId);

            if (exam == null) return NotFound();
            if (exam.CreatedBy != instructorId.Value) return Unauthorized();

            return View(exam);
        }

        // ================= ASSIGN EXAM =================
        [HttpGet]
        public async Task<IActionResult> AssignExam(int examId)
        {
            var instructorId = await GetCurrentInstructorIdAsync();
            if (instructorId == null) return Unauthorized();

            var exam = await _context.Exams.FindAsync(examId);
            if (exam == null) return NotFound();
            if (exam.CreatedBy != instructorId.Value) return Unauthorized();

            // Get eligible students: those enrolled in the course
            var eligibleStudents = await _context.StudentCourses
                .Where(sc => sc.CourseId == exam.CourseId)
                .Include(sc => sc.Student)
                .Select(sc => sc.Student)
                .ToListAsync();

            var viewModel = new AssignExamVM
            {
                ExamId = examId,
                Students = new MultiSelectList(eligibleStudents, "StudentId", "StudentName"),
                Departments = new SelectList(await _context.Departments.ToListAsync(), "DeptId", "DeptName")
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignExam(AssignExamVM model)
        {
            var instructorId = await GetCurrentInstructorIdAsync();
            if (instructorId == null) return Unauthorized();

            var exam = await _context.Exams.FindAsync(model.ExamId);
            if (exam == null) return NotFound();
            if (exam.CreatedBy != instructorId.Value) return Unauthorized();

            if (!ModelState.IsValid)
            {
                // Reload lists
                var eligibleStudents = await _context.StudentCourses
                    .Where(sc => sc.CourseId == exam.CourseId)
                    .Include(sc => sc.Student)
                    .Select(sc => sc.Student)
                    .ToListAsync();
                model.Students = new MultiSelectList(eligibleStudents, "StudentId", "StudentName");
                model.Departments = new SelectList(await _context.Departments.ToListAsync(), "DeptId", "DeptName");
                return View(model);
            }

            var assignedDate = DateTime.Now;
            var studentIds = new List<int>();

            if (model.AssignByDepartment && model.DepartmentId.HasValue)
            {
                // Assign to all students in department enrolled in course
                studentIds = await _context.Students
                    .Where(s => s.DeptId == model.DepartmentId.Value)
                    .Join(_context.StudentCourses.Where(sc => sc.CourseId == exam.CourseId),
                          s => s.StudentId,
                          sc => sc.StudentId,
                          (s, sc) => s.StudentId)
                    .ToListAsync();
            }
            else if (model.SelectedStudentIds != null && model.SelectedStudentIds.Any())
            {
                // Manual selection, ensure they are eligible
                studentIds = model.SelectedStudentIds
                    .Where(sid => _context.StudentCourses.Any(sc => sc.StudentId == sid && sc.CourseId == exam.CourseId))
                    .ToList();
            }
            else
            {
                ModelState.AddModelError("", "Please select students or a department.");
                // Reload lists...
                return View(model);
            }

            foreach (var studentId in studentIds)
            {
                if (!await _context.ExamAssignments.AnyAsync(ea => ea.ExamId == model.ExamId && ea.StudentId == studentId))
                {
                    _context.ExamAssignments.Add(new ExamAssignment
                    {
                        ExamId = model.ExamId,
                        StudentId = studentId,
                        AssignedDate = assignedDate,
                        DueDate = model.DueDate,
                        IsActive = true
                    });
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Exam assigned successfully to selected students!";
            return RedirectToAction(nameof(ExamDetails), new { examId = model.ExamId });
        }

        // ================= EXAM RESULTS & STATS =================
        public async Task<IActionResult> ExamResults(int examId)
        {
            var instructorId = await GetCurrentInstructorIdAsync();
            if (instructorId == null) return Unauthorized();

            var exam = await _context.Exams
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.ExamId == examId);

            if (exam == null) return NotFound();
            if (exam.CreatedBy != instructorId.Value) return Unauthorized();

            // Get ExamGrades (pre-calculated scores)
            var examGrades = await _context.ExamGrades
                .Where(eg => eg.ExamId == examId)
                .Include(eg => eg.Student)
                .ToListAsync();

            // Also get assigned students who haven't taken it yet
            var assignedStudentIds = await _context.ExamAssignments
                .Where(ea => ea.ExamId == examId)
                .Select(ea => ea.StudentId)
                .ToListAsync();

            var gradedStudentIds = examGrades.Select(eg => eg.StudentId).ToList();

            var notTakenStudents = await _context.Students
                .Where(s => assignedStudentIds.Contains(s.StudentId) && !gradedStudentIds.Contains(s.StudentId))
                .ToListAsync();

            // Calculate statistics
            int totalAssigned = assignedStudentIds.Count;
            int completed = examGrades.Count;
            int notTaken = notTakenStudents.Count;

            // CORRECT
            double averagePercentage = examGrades.Any()
                ? Math.Round(examGrades.Average(eg => (double)(eg.Percentage ?? 0)), 2)
                : 0;

            int passed = examGrades.Count(eg => eg.Status == "Pass" || (eg.Percentage >= exam.PassingScore));
            double passRate = completed > 0 ? Math.Round((double)passed / completed * 100, 2) : 0;

            // Get hardest questions stats
            var questionStats = await GetQuestionStats(examId);

            var viewModel = new ExamResultsVM
            {
                Exam = exam,
                ExamGrades = examGrades,                    // Students who took the exam
                NotTakenStudents = notTakenStudents,        // Assigned but not submitted
                TotalAssigned = totalAssigned,
                Completed = completed,
                NotTaken = notTaken,
                AveragePercentage = averagePercentage,
                PassRate = passRate,
                PassedCount = passed,
                FailedCount = completed - passed,
                QuestionStats = questionStats
            };

            return View(viewModel);
        }


        // Helper for question stats
        private async Task<List<QuestionStatVM>> GetQuestionStats(int examId)
        {
            var questionAttempts = await _context.StudentAnswers
                .Where(sa => sa.ExamId == examId)
                .Include(sa => sa.Question)
                .Include(sa => sa.SelectedChoice)
                .GroupBy(sa => new { sa.QuestionId, sa.Question.QuestionText })
                .Select(g => new
                {
                    QuestionId = g.Key.QuestionId,
                    QuestionText = g.Key.QuestionText,
                    TotalAttempts = g.Count(),
                    CorrectAnswers = g.Count(sa => sa.SelectedChoice != null && sa.SelectedChoice.IsCorrect == true)
                })
                .ToListAsync();

            var stats = questionAttempts.Select(q => new QuestionStatVM
            {
                QuestionId = q.QuestionId,
                QuestionText = q.QuestionText,
                TotalAttempts = q.TotalAttempts,
                CorrectAnswers = q.CorrectAnswers,
                WrongAnswers = q.TotalAttempts - q.CorrectAnswers,
                CorrectPercentage = q.TotalAttempts > 0
                    ? Math.Round((double)q.CorrectAnswers / q.TotalAttempts * 100, 2)
                    : 0
            }).OrderBy(s => s.CorrectPercentage) // Hardest first
              .ToList();

            return stats;
        }
    }
}