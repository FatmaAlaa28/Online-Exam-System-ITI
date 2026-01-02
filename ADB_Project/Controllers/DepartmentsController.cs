using ADB_Project.Data;
using ADB_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

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

            try
            {
                _context.Database.ExecuteSqlRaw(
                    "EXEC sp_Department_Insert @DeptName, @Description, @IsActive",
                    new SqlParameter("@DeptName", dept.DeptName),
                    new SqlParameter("@Description", (object?)dept.Description ?? DBNull.Value),
                    new SqlParameter("@IsActive", dept.IsActive)
                );

                TempData["Success"] = "Department created successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error creating department: {ex.Message}";
            }

            return RedirectToAction("Departments");
        }

        public IActionResult EditDepartment(int id)
        {
            var dept = _context.Departments
                .FromSqlRaw("EXEC sp_Department_Select @DeptID",
                    new SqlParameter("@DeptID", id))
                .AsEnumerable()
                .FirstOrDefault();

            if (dept == null)
            {
                TempData["Error"] = "Department not found.";
                return RedirectToAction("Departments");
            }

            return View(dept);
        }

        [HttpPost]
        public IActionResult EditDepartment(Department dept)
        {
            if (!ModelState.IsValid)
                return View(dept);

            try
            {
                _context.Database.ExecuteSqlRaw(
                    "EXEC sp_Department_Update @DeptID, @DeptName, @Description, @IsActive",
                    new SqlParameter("@DeptID", dept.DeptId),
                    new SqlParameter("@DeptName", dept.DeptName),
                    new SqlParameter("@Description", (object?)dept.Description ?? DBNull.Value),
                    new SqlParameter("@IsActive", dept.IsActive)
                );

                TempData["Success"] = "Department updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating department: {ex.Message}";
            }

            return RedirectToAction("Departments");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();

                if (connection.State != ConnectionState.Open)
                    await _context.Database.OpenConnectionAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "sp_Department_Delete";
                    command.CommandType = CommandType.StoredProcedure;

                    var param = command.CreateParameter();
                    param.ParameterName = "@DeptID";
                    param.Value = id;
                    command.Parameters.Add(param);

                    await command.ExecuteNonQueryAsync();
                }

                TempData["Success"] = "Department deleted successfully!";
            }
            catch (SqlException ex)
            {
                // Handle foreign key constraint violations
                if (ex.Number == 547) // FK constraint error
                {
                    TempData["Error"] = "Cannot delete this department because it has related records (students, courses, or branches).";
                }
                else
                {
                    TempData["Error"] = $"Database error: {ex.Message}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting department: {ex.Message}";
            }

            return RedirectToAction(nameof(Departments));
        }
    }
}