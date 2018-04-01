using System.ComponentModel.DataAnnotations;

namespace Mindfights.DTOs
{
    public class MindfightQuestionAnswerDto
    {
        public long Id { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public bool IsCorrect { get; set; }
    }
}
