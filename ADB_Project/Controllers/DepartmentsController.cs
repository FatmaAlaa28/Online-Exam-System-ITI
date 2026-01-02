using ADB_Project.Data;
using ADB_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ADB_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentsController : Controller
    {
        private readonly OnlineExamDbContext _context;

        public DepartmentsController(OnlineExamDbContext context)
        {
            _context = context;
        }

        public IActionResult Departments()
        {
            var depts = _context.Departments
                .FromSqlRaw("EXEC sp_Department_Select")
                .ToList();

            return View(depts);
        }

        public IActionResult CreateDepartment()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateDepartment(Department dept)
        {
            if (!ModelState.IsValid)
                return View(dept);

            _context.Database.ExecuteSqlRaw(
                "EXEC sp_Department_Insert @DeptName, @Description, @IsActive",
                new SqlParameter("@DeptName", dept.DeptName),
                new SqlParameter("@Description", (object?)dept.Description ?? DBNull.Value),
                new SqlParameter("@IsActive", dept.IsActive)
            );

            return RedirectToAction("Departments");
        }

        public IActionResult EditDepartment(int id)
        {
            var dept = _context.Departments
                .FromSqlRaw("EXEC sp_Department_Select @DeptID",
                    new SqlParameter("@DeptID", id))
                .AsEnumerable()
                .FirstOrDefault();

            return View(dept);
        }

        [HttpPost]
        public IActionResult EditDepartment(Department dept)
        {
            _context.Database.ExecuteSqlRaw(
                "EXEC sp_Department_Update @DeptID, @DeptName, @Description, @IsActive",
                new SqlParameter("@DeptID", dept.DeptId),
                new SqlParameter("@DeptName", dept.DeptName),
                new SqlParameter("@Description", (object?)dept.Description ?? DBNull.Value),
                new SqlParameter("@IsActive", dept.IsActive)
            );

            return RedirectToAction("Departments");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "dbo.sp_Department_Delete";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@DeptID", id));

                if (_context.Database.GetDbConnection().State != System.Data.ConnectionState.Open)
                    await _context.Database.OpenConnectionAsync();

                await command.ExecuteNonQueryAsync();
            }

            TempData["Success"] = "Department deleted successfully!";
            return RedirectToAction(nameof(Departments));
        }
    }

}
