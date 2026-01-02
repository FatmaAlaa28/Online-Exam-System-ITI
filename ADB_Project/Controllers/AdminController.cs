using ADB_Project.Data;
using ADB_Project.Models;
using ADB_Project.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ADB_Project.Controllers
{
    public class AdminController : Controller
    {
        private readonly OnlineExamDbContext _context;

        public AdminController(OnlineExamDbContext context)
        {
            _context = context;
        }

        // ========================================
        // DASHBOARD
        // ========================================

        public async Task<IActionResult> Dashboard()
        {
            var model = new AdminDashboardViewModel
            {
                TotalStudents = await _context.Students
                    .CountAsync(s => s.IsActive == true),

                TotalInstructors = await _context.Instructors
                    .CountAsync(i => i.IsActive == true),

                TotalCourses = await _context.Courses
                    .CountAsync(c => c.IsActive == true),

                TotalExams = await _context.Exams.CountAsync(),

                ActiveBranches = await _context.Branches
                    .CountAsync(b => b.IsActive == true),

                ActiveDepartments = await _context.Departments
                    .CountAsync(d => d.IsActive == true)
            };

            return View(model);
        }

        // ========================================
        // BRANCHES CRUD
        // ========================================

        // GET: /Admin/Branches
        public async Task<IActionResult> Branches()
        {
            var branches = new List<Branch>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "EXEC sp_Branch_Select";
                command.CommandType = CommandType.Text;

                await _context.Database.OpenConnectionAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        branches.Add(new Branch
                        {
                            BranchId = reader.GetInt32(reader.GetOrdinal("BranchID")),
                            BranchName = reader.GetString(reader.GetOrdinal("BranchName")),
                            Location = reader.IsDBNull(reader.GetOrdinal("Location"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("Location")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                        });
                    }
                }

                await _context.Database.CloseConnectionAsync();
            }

            return View(branches);
        }

        // GET: /Admin/CreateBranch
        public IActionResult CreateBranch()
        {
            // ✅ FIX: Initialize Model
            return View(new Branch { IsActive = true });
        }

        // POST: /Admin/CreateBranch
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBranch(Branch branch)
        {
            if (!ModelState.IsValid)
            {
                return View(branch);
            }

            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "sp_Branch_Insert";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@BranchName", SqlDbType.NVarChar, 100)
                    {
                        Value = branch.BranchName
                    });

                    command.Parameters.Add(new SqlParameter("@Location", SqlDbType.NVarChar, 255)
                    {
                        Value = (object)branch.Location ?? DBNull.Value
                    });

                    command.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit)
                    {
                        Value = branch.IsActive
                    });

                    var outputParam = new SqlParameter("@NewBranchID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputParam);

                    await _context.Database.OpenConnectionAsync();
                    await command.ExecuteNonQueryAsync();
                    await _context.Database.CloseConnectionAsync();

                    TempData["Success"] = "Branch created successfully!";
                    return RedirectToAction(nameof(Branches));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error creating branch: {ex.Message}";
                return View(branch);
            }
        }

        // GET: /Admin/EditBranch/5
        public async Task<IActionResult> EditBranch(int id)
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "sp_Branch_Select";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@BranchID", id));

                await _context.Database.OpenConnectionAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var branch = new Branch
                        {
                            BranchId = reader.GetInt32(reader.GetOrdinal("BranchID")),
                            BranchName = reader.GetString(reader.GetOrdinal("BranchName")),
                            Location = reader.IsDBNull(reader.GetOrdinal("Location"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("Location")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                        };

                        await _context.Database.CloseConnectionAsync();
                        return View(branch);
                    }
                }

                await _context.Database.CloseConnectionAsync();
            }

            return NotFound();
        }

        // POST: /Admin/EditBranch
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBranch(Branch branch)
        {
            if (!ModelState.IsValid)
            {
                return View(branch);
            }

            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "sp_Branch_Update";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@BranchID", branch.BranchId));
                    command.Parameters.Add(new SqlParameter("@BranchName", branch.BranchName));
                    command.Parameters.Add(new SqlParameter("@Location", (object)branch.Location ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@IsActive", branch.IsActive));

                    await _context.Database.OpenConnectionAsync();
                    await command.ExecuteNonQueryAsync();
                    await _context.Database.CloseConnectionAsync();
                }

                TempData["Success"] = "Branch updated successfully!";
                return RedirectToAction(nameof(Branches));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating branch: {ex.Message}";
                return View(branch);
            }
        }

        // POST: /Admin/DeleteBranch
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBranch(int branchID)
        {
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "sp_Branch_Delete";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@BranchID", branchID));

                    await _context.Database.OpenConnectionAsync();
                    await command.ExecuteNonQueryAsync();
                    await _context.Database.CloseConnectionAsync();
                }

                TempData["Success"] = "Branch deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Cannot delete branch: {ex.Message}";
            }

            return RedirectToAction(nameof(Branches));
        }

        // ========================================
        // DEPARTMENTS CRUD
        // ========================================

        // GET: /Admin/Departments
        public async Task<IActionResult> Departments()
        {
            var departments = new List<Department>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "EXEC sp_Department_Select";
                await _context.Database.OpenConnectionAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        departments.Add(new Department
                        {
                            DeptId = reader.GetInt32(reader.GetOrdinal("DeptID")),
                            DeptName = reader.GetString(reader.GetOrdinal("DeptName")),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("Description")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                        });
                    }
                }

                await _context.Database.CloseConnectionAsync();
            }

            return View(departments);
        }

        // GET: /Admin/CreateDepartment
        public IActionResult CreateDepartment()
        {
            return View(new Department { IsActive = true });
        }

        // POST: /Admin/CreateDepartment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDepartment(Department department)
        {
            if (!ModelState.IsValid)
            {
                return View(department);
            }

            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "sp_Department_Insert";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@DeptName", department.DeptName));
                    command.Parameters.Add(new SqlParameter("@Description", (object)department.Description ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@IsActive", department.IsActive));

                    var outputParam = new SqlParameter("@NewDeptID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputParam);

                    await _context.Database.OpenConnectionAsync();
                    await command.ExecuteNonQueryAsync();
                    await _context.Database.CloseConnectionAsync();
                }

                TempData["Success"] = "Department created successfully!";
                return RedirectToAction(nameof(Departments));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return View(department);
            }
        }

        // GET: /Admin/EditDepartment/5
        public async Task<IActionResult> EditDepartment(int id)
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "sp_Department_Select";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@DeptID", id));

                await _context.Database.OpenConnectionAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var dept = new Department
                        {
                            DeptId = reader.GetInt32(reader.GetOrdinal("DeptID")),
                            DeptName = reader.GetString(reader.GetOrdinal("DeptName")),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("Description")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                        };

                        await _context.Database.CloseConnectionAsync();
                        return View(dept);
                    }
                }

                await _context.Database.CloseConnectionAsync();
            }

            return NotFound();
        }

        // POST: /Admin/EditDepartment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDepartment(Department department)
        {
            if (!ModelState.IsValid)
            {
                return View(department);
            }

            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "sp_Department_Update";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@DeptID", department.DeptId));
                    command.Parameters.Add(new SqlParameter("@DeptName", department.DeptName));
                    command.Parameters.Add(new SqlParameter("@Description", (object)department.Description ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@IsActive", department.IsActive));

                    await _context.Database.OpenConnectionAsync();
                    await command.ExecuteNonQueryAsync();
                    await _context.Database.CloseConnectionAsync();
                }

                TempData["Success"] = "Department updated successfully!";
                return RedirectToAction(nameof(Departments));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return View(department);
            }
        }

        // POST: /Admin/DeleteDepartment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDepartment(int deptID)
        {
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "sp_Department_Delete";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@DeptID", deptID));

                    await _context.Database.OpenConnectionAsync();
                    await command.ExecuteNonQueryAsync();
                    await _context.Database.CloseConnectionAsync();
                }

                TempData["Success"] = "Department deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Cannot delete: {ex.Message}";
            }

            return RedirectToAction(nameof(Departments));
        }

        // ========================================
        // STUDENTS CRUD
        // ========================================

        // GET: /Admin/Students
        public async Task<IActionResult> Students()
        {
            var students = await _context.Students
                .Include(s => s.Dept)
                .Include(s => s.Branch)
                .Where(s => s.IsActive == true)
                .ToListAsync();

            return View(students);
        }

        // ========================================
        // INSTRUCTORS CRUD
        // ========================================

        // GET: /Admin/Instructors
        public async Task<IActionResult> Instructors()
        {
            var instructors = await _context.Instructors
                .Include(i => i.Branch)
                .Where(i => i.IsActive == true)
                .ToListAsync();

            return View(instructors);
        }

        // ========================================
        // COURSES CRUD
        // ========================================

        // GET: /Admin/Courses
        public async Task<IActionResult> Courses()
        {
            var courses = await _context.Courses
                .Where(c => c.IsActive == true)
                .ToListAsync();

            return View(courses);
        }
    }
}