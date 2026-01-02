using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ADB_Project.Models.ViewModels
{
    public class GenerateExamViewModel
    {
        [Required]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Exam Name")]
        public string ExamName { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Number of questions must be between 0 and 100.")]
        [Display(Name = "Number of MCQ Questions")]
        public int NumMCQ { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Number of questions must be between 0 and 100.")]
        [Display(Name = "Number of True/False Questions")]
        public int NumTF { get; set; }

        [Display(Name = "Exam Date (Optional)")]
        [DataType(DataType.Date)]
        public DateTime? ExamDate { get; set; }

        [Required]
        [Range(1, 240, ErrorMessage = "Duration must be between 1 and 240 minutes.")]
        [Display(Name = "Duration (in minutes)")]
        public int DurationMinutes { get; set; } = 60; // Default value

        [Required]
        [Range(0, 100, ErrorMessage = "Percentage must be between 0 and 100.")]
        [Display(Name = "Passing Percentage")]
        public decimal PassingPercentage { get; set; } = 60; // Default value

        // To populate the dropdown list in the view
        public IEnumerable<SelectListItem>? Courses { get; set; }
    }
}
