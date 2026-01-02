using ADB_Project.Data;
using ADB_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ADB_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CourseController : Controller
    {
        private readonly OnlineExamDbContext _context;

        public CourseController(OnlineExamDbContext context)
        {
            _context = context;
        }

        public IActionResult Courses()
        {
            return View(_context.Courses.ToList());
        }

        public IActionResult CreateCourse()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCourse(Course course)
        {
            if (!ModelState.IsValid)
                return View(course);

            _context.Courses.Add(course);
            _context.SaveChanges();

            return RedirectToAction("Courses");
        }

        public IActionResult EditCourse(int id)
        {
            return View(_context.Courses.Find(id));
        }

        [HttpPost]
        public IActionResult EditCourse(Course course)
        {
            _context.Courses.Update(course);
            _context.SaveChanges();

            return RedirectToAction("Courses");
        }

        [HttpPost]
        public IActionResult DeleteCourse(int id)
        {
            var course = _context.Courses.Find(id);
            _context.Courses.Remove(course);
            _context.SaveChanges();

            return RedirectToAction("Courses");
        }
    }

}
