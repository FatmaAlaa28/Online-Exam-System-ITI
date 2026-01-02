using ADB_Project.Data;
using ADB_Project.Models;
using ADB_Project.Models.ADB_Project.Models;
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
    [Authorize(Roles = "Admin")]
    public class InstructorController : Controller
    {
        private readonly OnlineExamDbContext _context;
        private readonly UserManager<AppUser> _userManager; // Changed from IdentityUser to AppUser

        public InstructorController(
            OnlineExamDbContext context,
            UserManager<AppUser> userManager) // Changed from IdentityUser to AppUser
        {
            _context = context;
            _userManager = userManager;
        }

        // ================= LIST =================
        public IActionResult Instructors()
        {
            var instructors = _context.Instructors
                .Include(i => i.Branch)
                .Where(i => i.IsActive == true)
                .ToList();

            return View(instructors);
        }

        // ================= CREATE =================
        public IActionResult CreateInstructor()
        {
            ViewBag.Branches = _context.Branches
                .Where(b => b.IsActive == true)
                .ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateInstructor(Instructor instructor, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("password", "Password is required.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Branches = _context.Branches.Where(b => b.IsActive == true).ToList();
                return View(instructor);
            }

            try
            {
                // 1️⃣ Create Identity User
                var user = new AppUser
                {
                    UserName = instructor.Email,
                    Email = instructor.Email,
                    FullName = instructor.InstructorName,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                        ModelState.AddModelError("", err.Description);

                    ViewBag.Branches = _context.Branches.Where(b => b.IsActive == true).ToList();
                    return View(instructor);
                }

                await _userManager.AddToRoleAsync(user, "Instructor");

                // 2️⃣ Add Instructor to database
                instructor.CreatedDate = DateTime.Now;
                instructor.ModifiedDate = DateTime.Now;
                instructor.IsActive = instructor.IsActive ?? true;

                _context.Instructors.Add(instructor);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Instructor created successfully!";
                return RedirectToAction(nameof(Instructors));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating instructor: {ex.Message}");
                ViewBag.Branches = _context.Branches.Where(b => b.IsActive == true).ToList();
                return View(instructor);
            }
        }

        // ================= EDIT =================
        public IActionResult EditInstructor(int id)
        {
            ViewBag.Branches = _context.Branches
                .Where(b => b.IsActive == true)
                .ToList();

            var instructor = _context.Instructors
                .Include(i => i.Branch)
                .FirstOrDefault(i => i.InstructorId == id);

            if (instructor == null)
            {
                TempData["Error"] = "Instructor not found.";
                return RedirectToAction(nameof(Instructors));
            }

            return View(instructor);
        }

        [HttpPost]
        public async Task<IActionResult> EditInstructor(Instructor instructor)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Branches = _context.Branches.Where(b => b.IsActive == true).ToList();
                return View(instructor);
            }

            try
            {
                var existingInstructor = await _context.Instructors
                    .FirstOrDefaultAsync(i => i.InstructorId == instructor.InstructorId);

                if (existingInstructor == null)
                {
                    TempData["Error"] = "Instructor not found.";
                    return RedirectToAction(nameof(Instructors));
                }

                // Update properties
                existingInstructor.InstructorName = instructor.InstructorName;
                existingInstructor.Phone = instructor.Phone;
                existingInstructor.HireDate = instructor.HireDate;
                existingInstructor.BranchId = instructor.BranchId;
                existingInstructor.IsActive = instructor.IsActive;
                existingInstructor.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Instructor updated successfully!";
                return RedirectToAction(nameof(Instructors));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating instructor: {ex.Message}";
                ViewBag.Branches = _context.Branches.Where(b => b.IsActive == true).ToList();
                return View(instructor);
            }
        }

        // ================= DELETE =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInstructor(int id)
        {
            try
            {
                var instructor = await _context.Instructors
                    .FirstOrDefaultAsync(i => i.InstructorId == id);

                if (instructor != null)
                {
                    // Delete Identity User first
                    if (!string.IsNullOrEmpty(instructor.Email))
                    {
                        var user = await _userManager.FindByEmailAsync(instructor.Email);
                        if (user != null)
                        {
                            await _userManager.DeleteAsync(user);
                        }
                    }

                    // Delete Instructor from database
                    _context.Instructors.Remove(instructor);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Instructor deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Instructor not found.";
                }
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = "Cannot delete this instructor because they have related records (courses, exams).";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting instructor: {ex.Message}";
            }

            return RedirectToAction(nameof(Instructors));
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

        // ================= GENERATE EXAM =================
        [HttpGet]
        public async Task<IActionResult> GenerateExam()
        {
            var instructorId = await GetCurrentInstructorIdAsync();
            if (instructorId == null)
            {
                return Unauthorized("Instructor account not found.");
            }

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
                    .Where(ic => ic.InstructorId == instructorId)
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

                return RedirectToAction("Details", "Exams", new { id = newExamId });
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError(string.Empty, $"Database Error: {ex.Message}");

                var courses = await _context.InstructorCourses
                    .Where(ic => ic.InstructorId == instructorId)
                    .Include(ic => ic.Course)
                    .Select(ic => ic.Course)
                    .ToListAsync();
                model.Courses = new SelectList(courses, "CourseId", "CourseName");
                return View(model);
            }
        }
    }
}