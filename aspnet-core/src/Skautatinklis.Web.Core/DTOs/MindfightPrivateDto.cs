using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Skautatinklis.Models;

namespace Skautatinklis.DTOs
{
    [AutoMapFrom(typeof(Mindfight))]
    public class MindfightPrivateDto
    {
        public long Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }
        [Required]
        [MaxLength(2550)]
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsPrivate { get; set; }
        public int? PrepareTime { get; set; }
        public int QuestionsCount { get; set; }
        public int TimeLimitInMinutes { get; set; }
        public int PlayersLimit { get; set; }
    }
}
