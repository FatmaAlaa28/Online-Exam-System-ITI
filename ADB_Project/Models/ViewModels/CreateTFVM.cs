using System.ComponentModel.DataAnnotations;

namespace ADB_Project.Models.ViewModels
{
    public class CreateTFVM
    {
        public int? QuestionId { get; set; } // For edit
        [Required]
        public string QuestionText { get; set; }
        [Required, Range(1, int.MaxValue)]
        public int Points { get; set; }
        [Required]
        public string DifficultyLevel { get; set; }
        public int CourseId { get; set; }
        [Required]
        public bool IsTrueCorrect { get; set; }
    }
}
