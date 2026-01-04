using ADB_Project.Data;
using ADB_Project.Models;
using ADB_Project.Models.ADB_Project.Models;
using Microsoft.AspNetCore.Authorization; // تأكد من وجود هذا
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq; // تأكد من وجود هذا
using System.Threading.Tasks; // تأكد من وجود هذا

namespace ADB_Project.Controllers
{

    [Authorize(Roles = "Admin , Instructor")] // تأمين الـ Controller بالكامل
    public class StudentController : Controller
    {
        private readonly OnlineExamDbContext _context;
        private readonly UserManager<AppUser> _userManager;   // ← غيّر هنا من IdentityUser إلى AppUser

        public StudentController(OnlineExamDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/Student/Students
        public async Task<IActionResult> Students()
        {
            // تحسين الأداء: جلب البيانات مع الأقسام والفروع في استعلام واحد
            var students = await _context.Students
                .Include(s => s.Dept)
                .Include(s => s.Branch)
                .AsNoTracking() // للقراءة فقط، يحسن الأداء
                .ToListAsync();
            return View(students);
        }

        // GET: Admin/Student/CreateStudent
        public async Task<IActionResult> CreateStudent()
        {
            await LoadDropdowns(); // استخدام دالة مساعدة لتحميل القوائم
            return View();
        }

        // POST: Admin/Student/CreateStudent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudent(Student student, string password)
        {
            // التحقق من أن البريد الإلكتروني غير مستخدم مسبقًا
            if (await _userManager.FindByEmailAsync(student.Email) != null)
            {
                ModelState.AddModelError("Email", "This email address is already in use.");
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(student);
            }

            var user = new AppUser { UserName = student.Email, Email = student.Email };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Student");

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Student created successfully!";
                return RedirectToAction(nameof(Students));
            }

            // إذا فشل إنشاء مستخدم Identity، أضف الأخطاء للموديل
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            await LoadDropdowns();
            return View(student);
        }

        // GET: Admin/Student/EditStudent/5
        public async Task<IActionResult> EditStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            await LoadDropdowns();
            return View(student);
        }

        // POST: Admin/Student/EditStudent/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudent(int id, Student student)
        {
            if (id != student.StudentId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Student updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Students.Any(e => e.StudentId == student.StudentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Students));
            }
            await LoadDropdowns();
            return View(student);
        }

        // POST: Admin/Student/DeleteStudent/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                TempData["Error"] = "Student not found.";
                return RedirectToAction(nameof(Students));
            }

            // استخدام Transaction لضمان حذف المستخدم والطالب معًا
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. حذف الطالب من جدول Students
                    _context.Students.Remove(student);
                    await _context.SaveChangesAsync();

                    // 2. حذف حساب Identity
                    var user = await _userManager.FindByEmailAsync(student.Email);
                    if (user != null)
                    {
                        var result = await _userManager.DeleteAsync(user);
                        if (!result.Succeeded)
                        {
                            // إذا فشل حذف المستخدم، تراجع عن حذف الطالب
                            await transaction.RollbackAsync();
                            TempData["Error"] = "Could not delete the user account. Rolling back changes.";
                            return RedirectToAction(nameof(Students));
                        }
                    }

                    // إذا نجحت كل العمليات، قم بتأكيد المعاملة
                    await transaction.CommitAsync();
                    TempData["Success"] = "Student and their account deleted successfully!";
                }
                catch (DbUpdateException ex) // للتعامل مع مشاكل Foreign Key
                {
                    await transaction.RollbackAsync();
                    TempData["Error"] = $"Cannot delete student. They might be linked to other records (e.g., exams, courses). Error: {ex.InnerException?.Message}";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["Error"] = $"An unexpected error occurred: {ex.Message}";
                }
            }

            return RedirectToAction(nameof(Students));
        }

        // دالة مساعدة لتحميل القوائم المنسدلة
        private async Task LoadDropdowns()
        {
            ViewBag.Departments = await _context.Departments.Where(d => d.IsActive).ToListAsync();
            ViewBag.Branches = await _context.Branches.Where(b => b.IsActive).ToListAsync();
        }
    }
}
