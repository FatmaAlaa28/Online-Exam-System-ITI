using ADB_Project.Data;
using ADB_Project.Models;
using ADB_Project.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ADB_Project.Controllers
{
    [Authorize(Roles = "Admin , Instructor")]
    public class CourseController : Controller
    {
        private readonly OnlineExamDbContext _context;

        public CourseController(OnlineExamDbContext context)
        {
            _context = context;
        }

        // ======================
        // GET: Courses
        // ======================
        public IActionResult Index()
        {
            var courses = _context.Courses.ToList();
            return View(courses);
        }

        // ======================
        // GET: Courses/Create
        // ======================
        public IActionResult Create()
        {
            return View();
        }

        // ======================
        // POST: Courses/Create
        // ======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Course course)
        {
            if (!ModelState.IsValid)
                return View(course);

            course.CreatedDate = DateTime.Now;
            course.IsActive = true;

            _context.Courses.Add(course);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // ======================
        // GET: Courses/Edit/5
        // ======================
        public IActionResult Edit(int id)
        {
            var course = _context.Courses.Find(id);
            if (course == null)
                return NotFound();

            return View(course);
        }

        // ======================
        // POST: Courses/Edit/5
        // ======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Course course)
        {
            if (id != course.CourseId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(course);

            course.ModifiedDate = DateTime.Now;

            _context.Update(course);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // ======================
        // GET: Courses/Details/5
        // ======================
        public IActionResult Details(int id)
        {
            var course = _context.Courses
                .FirstOrDefault(c => c.CourseId == id);

            if (course == null)
                return NotFound();

            return View(course);
        }

        // ======================
        // GET: Courses/Delete/5
        // ======================
        public IActionResult Delete(int id)
        {
            var course = _context.Courses.Find(id);
            if (course == null)
                return NotFound();

            return View(course);
        }

        // ======================
        // POST: Courses/Delete/5
        // ======================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var course = _context.Courses.Find(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }


        // ========================================
        // COURSES CRUD
        // ========================================

        // GET: /Admin/Courses
        public async Task<IActionResult> Courses()
        {
            var model = new AssignCourseViewModel
            {
                Instructors = _context.Instructors
           .Select(i => new SelectListItem
           {
               Value = i.InstructorId.ToString(),   // اللي هيتبعت
               Text = i.InstructorName              // اللي هيظهر
           }).ToList(),

                Courses = _context.Courses
           .Select(c => new SelectListItem
           {
               Value = c.CourseId.ToString(),
               Text = c.CourseName
           }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Courses(AssignCourseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // إعادة ملء القوائم في حالة الخطأ
                model.Instructors = _context.Instructors
                    .Select(i => new SelectListItem
                    {
                        Value = i.InstructorId.ToString(),
                        Text = i.InstructorName
                    }).ToList();

                model.Courses = _context.Courses
                    .Select(c => new SelectListItem
                    {
                        Value = c.CourseId.ToString(),
                        Text = c.CourseName
                    }).ToList();

                return View(model);
            }

            // منع التكرار (نفس المدرس + نفس الكورس)
            bool alreadyAssigned = _context.InstructorCourses
                .Any(ic => ic.InstructorId == model.InstructorId
                        && ic.CourseId == model.CourseId);

            if (alreadyAssigned)
            {
                ModelState.AddModelError("", "This course is already assigned to this instructor.");

                model.Instructors = _context.Instructors
                    .Select(i => new SelectListItem
                    {
                        Value = i.InstructorId.ToString(),
                        Text = i.InstructorName
                    }).ToList();

                model.Courses = _context.Courses
                    .Select(c => new SelectListItem
                    {
                        Value = c.CourseId.ToString(),
                        Text = c.CourseName
                    }).ToList();

                return View(model);
            }

            // الحفظ
            var instructorCourse = new InstructorCourse
            {
                InstructorId = model.InstructorId,
                CourseId = model.CourseId,
                AssignmentDate = model.AssignmentDate ?? DateTime.Now
            };

            _context.InstructorCourses.Add(instructorCourse);
            _context.SaveChanges();

            return RedirectToAction("Index"); // أو أي صفحة تحبيها
        }

    }
}
    
