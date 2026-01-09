using ADB_Project.Data;
using ADB_Project.Models;
using ADB_Project.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ADB_Project.Controllers
{
    [Authorize(Roles = "Admin")]
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

        // GET: /Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var model = new AdminDashboardViewModel
            {
                // Basic Statistics
                TotalStudents = await _context.Students.CountAsync(s => s.IsActive == true),
                TotalInstructors = await _context.Instructors.CountAsync(i => i.IsActive == true),
                TotalCourses = await _context.Courses.CountAsync(c => c.IsActive == true),
                TotalExams = await _context.Exams.CountAsync(),
                ActiveBranches = await _context.Branches.CountAsync(b => b.IsActive == true),
                ActiveDepartments = await _context.Departments.CountAsync(d => d.IsActive == true),

                // Advanced Statistics
                PendingExams = await _context.ExamAssignments
                    .Where(ea => ea.IsActive == true && !_context.StudentExams
                        .Any(se => se.ExamId == ea.ExamId && se.StudentId == ea.StudentId && se.SubmittedDate != null))
                    .CountAsync(),

                CompletedExams = await _context.StudentExams
                    .CountAsync(se => se.SubmittedDate != null),

                TotalQuestions = await _context.Questions.CountAsync(q => q.IsActive == true),

                AverageExamScore = await _context.ExamGrades
                    .AnyAsync()
                    ? (decimal)await _context.ExamGrades.AverageAsync(eg => eg.Percentage ?? 0)
                    : 0,

                StudentsEnrolledThisMonth = await _context.Students
                    .CountAsync(s => s.EnrollmentDate.HasValue &&
                                   s.EnrollmentDate.Value.Month == DateTime.Now.Month &&
                                   s.EnrollmentDate.Value.Year == DateTime.Now.Year),

                ExamsCreatedThisMonth = await _context.Exams
                    .CountAsync(e => e.CreatedDate.HasValue &&
                                   e.CreatedDate.Value.Month == DateTime.Now.Month &&
                                   e.CreatedDate.Value.Year == DateTime.Now.Year),

                // Department Statistics
                DepartmentStatistics = await _context.Departments
                    .Where(d => d.IsActive == true)
                    .Select(d => new DepartmentStatistic
                    {
                        DepartmentName = d.DeptName,
                        StudentCount = d.Students.Count(s => s.IsActive == true),
                        CourseCount = d.DepartmentCourses.Count()
                    })
                    .OrderByDescending(d => d.StudentCount)
                    .Take(5)
                    .ToListAsync(),

                // Recent Exams
                RecentExams = await _context.Exams
                    .Include(e => e.Course)
                    .OrderByDescending(e => e.ExamDate)
                    .Take(5)
                    .Select(e => new ExamStatistic
                    {
                        ExamId = e.ExamId,
                        ExamName = e.ExamName,
                        CourseName = e.Course.CourseName,
                        ExamDate = e.ExamDate,
                        ParticipantCount = _context.ExamAssignments.Count(ea => ea.ExamId == e.ExamId),
                        AverageScore = _context.ExamGrades
                            .Where(eg => eg.ExamId == e.ExamId)
                            .Any()
                            ? (decimal)_context.ExamGrades
                                .Where(eg => eg.ExamId == e.ExamId)
                                .Average(eg => eg.Percentage ?? 0)
                            : 0
                    })
                    .ToListAsync(),

                // Top Performing Students
                TopPerformingStudents = await _context.ExamGrades
                    .Include(eg => eg.Student)
                        .ThenInclude(s => s.Dept)
                    .GroupBy(eg => new { eg.StudentId, eg.Student.StudentName, eg.Student.Dept.DeptName })
                    .Select(g => new TopStudent
                    {
                        StudentId = g.Key.StudentId,
                        StudentName = g.Key.StudentName,
                        DepartmentName = g.Key.DeptName ?? "N/A",
                        AverageGrade = (decimal)g.Average(eg => eg.Percentage ?? 0),
                        CompletedExams = g.Count()
                    })
                    .OrderByDescending(s => s.AverageGrade)
                    .Take(5)
                    .ToListAsync(),

                // Popular Courses
                PopularCourses = await _context.Courses
                    .Where(c => c.IsActive == true)
                    .Select(c => new CoursePopularity
                    {
                        CourseName = c.CourseName,
                        EnrolledStudents = c.StudentCourses.Count(sc => sc.IsActive == true),
                        TotalExams = c.Exams.Count()
                    })
                    .OrderByDescending(c => c.EnrolledStudents)
                    .Take(5)
                    .ToListAsync(),

                // Monthly Enrollments (Last 6 months)
                MonthlyEnrollments = Enumerable.Range(0, 6)
                    .Select(i => DateTime.Now.AddMonths(-i))
                    .Select(date => new MonthlyEnrollment
                    {
                        Month = date.ToString("MMM yyyy"),
                        Count = _context.Students
                            .Count(s => s.EnrollmentDate.HasValue &&
                                      s.EnrollmentDate.Value.Month == date.Month &&
                                      s.EnrollmentDate.Value.Year == date.Year)
                    })
                    .OrderBy(m => m.Month)
                    .ToList()
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

        // GET: /Admin/BranchDepartments (List all assignments)
        public async Task<IActionResult> BranchDepartments()
        {
            var assignments = await _context.BranchDepartments
                .Include(bd => bd.Branch)
                .Include(bd => bd.Dept)
                .ToListAsync();

            return View(assignments);
        }

        // GET: /Admin/AssignDepartmentToBranch
        public async Task<IActionResult> AssignDepartmentToBranch()
        {
            var model = new AssignDepartmentToBranchViewModel
            {
                Branches = await _context.Branches
                    .Where(b => b.IsActive)
                    .Select(b => new SelectListItem
                    {
                        Value = b.BranchId.ToString(),
                        Text = b.BranchName
                    }).ToListAsync(),

                Departments = await _context.Departments
                    .Where(d => d.IsActive)
                    .Select(d => new SelectListItem
                    {
                        Value = d.DeptId.ToString(),
                        Text = d.DeptName
                    }).ToListAsync()
            };

            return View(model);
        }

        // POST: /Admin/AssignDepartmentToBranch
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDepartmentToBranch(AssignDepartmentToBranchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // إعادة تحميل القوائم لو في خطأ
                model.Branches = await _context.Branches.Where(b => b.IsActive).Select(b => new SelectListItem { Value = b.BranchId.ToString(), Text = b.BranchName }).ToListAsync();
                model.Departments = await _context.Departments.Where(d => d.IsActive).Select(d => new SelectListItem { Value = d.DeptId.ToString(), Text = d.DeptName }).ToListAsync();
                return View(model);
            }

            // تحقق من التكرار
            var exists = await _context.BranchDepartments
                .AnyAsync(bd => bd.BranchId == model.BranchId && bd.DeptId == model.DeptId);

            if (exists)
            {
                TempData["Error"] = "This department is already assigned to this branch!";
                model.Branches = await _context.Branches.Select(b => new SelectListItem { Value = b.BranchId.ToString(), Text = b.BranchName }).ToListAsync();
                model.Departments = await _context.Departments.Select(d => new SelectListItem { Value = d.DeptId.ToString(), Text = d.DeptName }).ToListAsync();
                return View(model);
            }

            var assignment = new BranchDepartment
            {
                BranchId = model.BranchId,
                DeptId = model.DeptId,
                CreatedDate = DateTime.Now
            };

            _context.BranchDepartments.Add(assignment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Department assigned to branch successfully!";
            return RedirectToAction(nameof(BranchDepartments));
        }


        
        // POST: /Admin/RemoveDepartmentFromBranch (Delete assignment)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveDepartmentFromBranch(int branchId, int deptId)
        {
            var assignment = await _context.BranchDepartments
                .FirstOrDefaultAsync(bd => bd.BranchId == branchId && bd.DeptId == deptId);

            if (assignment == null)
            {
                TempData["Error"] = "Assignment not found!";
                return RedirectToAction(nameof(BranchDepartments));
            }

            _context.BranchDepartments.Remove(assignment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Assignment removed successfully!";
            return RedirectToAction(nameof(BranchDepartments));
        }




        //*******************
        // Controller Actions
        // GET: /Admin/CourseDepartments - List all assignments
        public async Task<IActionResult> CourseDepartments()
        {
            var assignments = await _context.DepartmentCourses
                .Include(dc => dc.Dept)
                .Include(dc => dc.Course)
                .OrderByDescending(dc => dc.CreatedDate)
                .ToListAsync();

            return View(assignments);
        }

        // GET: /Admin/AssignCourseToDepartment
        public async Task<IActionResult> AssignCourseToDepartment()
        {
            var model = new AssignCourseToDepartmentViewModel
            {
                Departments = await _context.Departments
                    .Where(d => d.IsActive)
                    .Select(d => new SelectListItem
                    {
                        Value = d.DeptId.ToString(),
                        Text = d.DeptName
                    }).ToListAsync(),

                Courses = await _context.Courses
                    .Select(c => new SelectListItem
                    {
                        Value = c.CourseId.ToString(),
                        Text = $"{c.CourseCode} - {c.CourseName}"
                    }).ToListAsync()
            };

            return View(model);
        }

        // POST: /Admin/AssignCourseToDepartment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignCourseToDepartment(AssignCourseToDepartmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reload lists if there's an error
                model.Departments = await _context.Departments
                    .Where(d => d.IsActive)
                    .Select(d => new SelectListItem
                    {
                        Value = d.DeptId.ToString(),
                        Text = d.DeptName
                    }).ToListAsync();

                model.Courses = await _context.Courses
                    .Select(c => new SelectListItem
                    {
                        Value = c.CourseId.ToString(),
                        Text = $"{c.CourseCode} - {c.CourseName}"
                    }).ToListAsync();

                return View(model);
            }

            // Check if assignment already exists
            var exists = await _context.DepartmentCourses
                .AnyAsync(dc => dc.DeptId == model.DeptId && dc.CourseId == model.CourseId);

            if (exists)
            {
                TempData["Error"] = "This course is already assigned to this department!";

                model.Departments = await _context.Departments
                    .Where(d => d.IsActive)
                    .Select(d => new SelectListItem
                    {
                        Value = d.DeptId.ToString(),
                        Text = d.DeptName
                    }).ToListAsync();

                model.Courses = await _context.Courses
                    .Select(c => new SelectListItem
                    {
                        Value = c.CourseId.ToString(),
                        Text = $"{c.CourseCode} - {c.CourseName}"
                    }).ToListAsync();

                return View(model);
            }

            // Create new assignment
            var assignment = new DepartmentCourse
            {
                DeptId = model.DeptId,
                CourseId = model.CourseId,
                IsRequired = model.IsRequired,
                CreatedDate = DateTime.Now
            };

            _context.DepartmentCourses.Add(assignment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Course assigned to department successfully!";
            return RedirectToAction("Dashboard");
        }

        // POST: /Admin/RemoveCourseFromDepartment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveCourseFromDepartment(int deptId, int courseId)
        {
            var assignment = await _context.DepartmentCourses
                .FirstOrDefaultAsync(dc => dc.DeptId == deptId && dc.CourseId == courseId);

            if (assignment == null)
            {
                TempData["Error"] = "Assignment not found!";
                return RedirectToAction(nameof(CourseDepartments));
            }

            _context.DepartmentCourses.Remove(assignment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Course removed from department successfully!";
            return RedirectToAction(nameof(CourseDepartments));
        }

        // POST: /Admin/ToggleCourseRequirement
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleCourseRequirement(int deptId, int courseId)
        {
            var assignment = await _context.DepartmentCourses
                .FirstOrDefaultAsync(dc => dc.DeptId == deptId && dc.CourseId == courseId);

            if (assignment == null)
            {
                TempData["Error"] = "Assignment not found!";
                return RedirectToAction(nameof(CourseDepartments));
            }

            assignment.IsRequired = !assignment.IsRequired;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Course requirement status updated successfully!";
            return RedirectToAction(nameof(CourseDepartments));
        }
    }
}