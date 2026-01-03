using System.ComponentModel.DataAnnotations;

namespace ADB_Project.Models.ViewModels
{
    public class CreateMCQVM
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
        public string Choice1 { get; set; }
        [Required]
        public string Choice2 { get; set; }
        [Required]
        public string Choice3 { get; set; }
        [Required]
        public string Choice4 { get; set; }
        [Required, Range(1, 4)]
        public int CorrectChoice { get; set; }
    }
}
