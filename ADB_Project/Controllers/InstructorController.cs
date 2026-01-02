using ADB_Project.Data;
using ADB_Project.Models;
using ADB_Project.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ADB_Project.Controllers
{
   // [Authorize(Roles = "Admin")]
    public class InstructorController : Controller
    {
        private readonly OnlineExamDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public InstructorController(
            OnlineExamDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ================= LIST =================
        public IActionResult Instructors()
        {
            var instructors = _context.Instructors
                .FromSqlRaw("EXEC sp_Instructor_Select")
                .ToList();

            return View(instructors);
        }

        // ================= CREATE =================
        public IActionResult CreateInstructor()
        {
            ViewBag.Branches = _context.Branches
                .FromSqlRaw("EXEC sp_Branch_Select")
                .ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateInstructor(Instructor instructor, string password)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Branches = _context.Branches.ToList();
                return View(instructor);
            }

            // 1️⃣ Identity User
            var user = new IdentityUser
            {
                UserName = instructor.Email,
                Email = instructor.Email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err.Description);

                ViewBag.Branches = _context.Branches.ToList();
                return View(instructor);
            }

            await _userManager.AddToRoleAsync(user, "Instructor");

            // 2️⃣ Insert Instructor (SP)
            _context.Database.ExecuteSqlRaw(
                "EXEC sp_Instructor_Insert @InstructorName, @Email, @BranchID, @IsActive",
                new SqlParameter("@InstructorName", instructor.InstructorName),
                new SqlParameter("@Email", (object?)instructor.Email ?? DBNull.Value),
                new SqlParameter("@BranchID", instructor.BranchId),
                new SqlParameter("@IsActive", instructor.IsActive ?? true)
            );

            return RedirectToAction(nameof(Instructors));
        }

        // ================= EDIT =================
        public IActionResult EditInstructor(int id)
        {
            ViewBag.Branches = _context.Branches.ToList();

            var instructor = _context.Instructors
                .FromSqlRaw("EXEC sp_Instructor_Select @ID",
                    new SqlParameter("@ID", id))
                .AsEnumerable()
                .FirstOrDefault();

            return View(instructor);
        }

        [HttpPost]
        public IActionResult EditInstructor(Instructor instructor)
        {
            _context.Database.ExecuteSqlRaw(
                "EXEC sp_Instructor_Update @ID, @InstructorName, @Email, @BranchID, @IsActive",
                new SqlParameter("@ID", instructor.InstructorId),
                new SqlParameter("@InstructorName", instructor.InstructorName),
                new SqlParameter("@Email", (object?)instructor.Email ?? DBNull.Value),
                new SqlParameter("@BranchID", instructor.BranchId),
                new SqlParameter("@IsActive", instructor.IsActive ?? true)
            );

            return RedirectToAction(nameof(Instructors));
        }

        // ================= DELETE =================
        [HttpPost]
        public async Task<IActionResult> DeleteInstructor(int id)
        {
            var instructor = _context.Instructors
                .FromSqlRaw("EXEC sp_Instructor_Select @ID",
                    new SqlParameter("@ID", id))
                .AsEnumerable()
                .FirstOrDefault();

            if (instructor != null)
            {
                var user = await _userManager.FindByEmailAsync(instructor.Email);
                if (user != null)
                    await _userManager.DeleteAsync(user);

                _context.Database.ExecuteSqlRaw(
                    "EXEC sp_Instructor_Delete @ID",
                    new SqlParameter("@ID", id)
                );
            }

            return RedirectToAction(nameof(Instructors));
        }
    
     private async Task<int?> GetCurrentInstructorIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return null;

            var instructor = await _context.Instructors
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Email == user.Email);

            return instructor?.InstructorId;
        }

        // GET: /Instructor/GenerateExam
        [HttpGet]
        public async Task<IActionResult> GenerateExam()
        {
            var instructorId = await GetCurrentInstructorIdAsync();
            if (instructorId == null)
            {
                return Unauthorized("Instructor account not found.");
            }

            // جلب المقررات التي يدرسها المحاضر الحالي فقط
            var courses = await _context.InstructorCourses
                .Where(ic => ic.InstructorId == instructorId)
                .Include(ic => ic.Course)
                .Select(ic => ic.Course)
                .ToListAsync();

            var viewModel = new GenerateExamViewModel
            {
                Courses = new SelectList(courses, "CourseId", "CourseName")
            };

            return View(viewModel);
        }

        // POST: /Instructor/GenerateExam
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
                // إذا فشل التحقق، أعد تحميل قائمة المقررات
                var courses = await _context.InstructorCourses
                    .Where(ic => ic.InstructorId == instructorId)
                    .Include(ic => ic.Course)
                    .Select(ic => ic.Course)
                    .ToListAsync();
                model.Courses = new SelectList(courses, "CourseId", "CourseName");
                return View(model);
            }

            // Parameters for the stored procedure
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

                // Redirect to exam details page (which we will create next)
                return RedirectToAction("Details", "Exams", new { id = newExamId });
            }
            catch (SqlException ex)
            {
                // التقاط الأخطاء المخصصة من الإجراء المخزن وعرضها
                ModelState.AddModelError(string.Empty, $"Database Error: {ex.Message}");

                // إعادة تحميل قائمة المقررات عند حدوث خطأ
                var courses = await _context.InstructorCourses
                    .Where(ic => ic.InstructorId == instructorId)
                    .Include(ic => ic.Course)
                    .Select(ic => ic.Course)
                    .ToListAsync();
                model.Courses = new SelectList(courses, "CourseId", "CourseName");
                return View(model);
            }
        } }

    }

