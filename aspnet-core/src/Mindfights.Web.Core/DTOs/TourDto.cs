using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Mindfights.Models;

namespace Mindfights.DTOs
{
    [AutoMapTo(typeof(Tour))]
    [AutoMapFrom(typeof(Tour))]
    public class TourDto
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        [MaxLength(2550)]
        public string Description { get; set; }

        [Required]
        public int TimeToEnterAnswersInSeconds { get; set; }

        public int TotalPoints { get; set; }

        public int OrderNumber { get; set; }

        public int QuestionsCount { get; set; }

        public bool IsLastTour { get; set; } = false;
    }
}
