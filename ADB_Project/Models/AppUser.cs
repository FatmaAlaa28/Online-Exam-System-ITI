using Microsoft.AspNetCore.Identity;
namespace ADB_Project.Models
{
    namespace ADB_Project.Models
    {
        public class AppUser : IdentityUser
        {
            public string FullName { get; set; } = string.Empty;
            public string? ProfilePicture { get; set; }
            public DateTime CreatedDate { get; set; } = DateTime.Now;

            // Relation to Role-specific tables (optional later)
            public int? StudentId { get; set; }
            public Student? Student { get; set; }

            public int? InstructorId { get; set; }
            public Instructor? Instructor { get; set; }
        }
    }
}
