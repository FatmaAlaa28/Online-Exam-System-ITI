using System.ComponentModel.DataAnnotations;

namespace ADB_Project.Models.ViewModels
{

        public class StudentViewModel
        {
            public int StudentId { get; set; }
            public string StudentName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Phone { get; set; }
            public string DepartmentName { get; set; } = string.Empty;
            public string BranchName { get; set; } = string.Empty;
            public DateTime? EnrollmentDate { get; set; }
            public bool IsActive { get; set; }
        }

        public class StudentCreateViewModel
        {
            [Required]
            [StringLength(100)]
            public string StudentName { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Phone]
            public string? Phone { get; set; }

            [Required]
            public int DeptId { get; set; }

            [Required]
            public int BranchId { get; set; }
        }

        public class StudentEditViewModel
        {
            public int StudentId { get; set; }

            [Required]
            [StringLength(100)]
            public string StudentName { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Phone]
            public string? Phone { get; set; }

            [Required]
            public int DeptId { get; set; }

            [Required]
            public int BranchId { get; set; }

            public bool IsActive { get; set; }
        }

}
