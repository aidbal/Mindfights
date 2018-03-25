using System.ComponentModel.DataAnnotations;

namespace Skautatinklis.DTOs
{
    public class MindfightQuestionAnswerDto
    {
        public long Id { get; set; }
        [Required]
        public string Answer { get; set; }
        [Required]
        public bool IsCorrect { get; set; }
    }
}
