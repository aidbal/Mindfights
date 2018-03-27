using Abp.AutoMapper;
using Skautatinklis.Models;
using System.ComponentModel.DataAnnotations;

namespace Skautatinklis.DTOs
{
    [AutoMapTo(typeof(Question))]
    [AutoMapFrom(typeof(Question))]
    public class QuestionDto
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        [MaxLength(2550)]
        public string Description { get; set; }

        [Required]
        public int Points { get; set; }

        [Required]
        public int TimeToAnswerInSeconds { get; set; }

        public string AttachmentLocation { get; set; }

        [Required]
        public int OrderNumber { get; set; }

        public long TourId { get; set; }

        public bool IsLastQuestion { get; set; } = false;
    }
}
