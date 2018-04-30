using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Mindfights.Models;

namespace Mindfights.DTOs
{
    [AutoMapFrom(typeof(Mindfight))]
    public class MindfightPublicDto
    {
        public long Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }
        [Required]
        [MaxLength(2550)]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsFinished { get; set; }
        public DateTime StartTime { get; set; }
        public int TeamsLimit { get; set; }
        public int RegisteredTeamsCount { get; set; }
        public long? CreatorId { get; set; }
        public string CreatorEmail { get; set; }
    }
}
