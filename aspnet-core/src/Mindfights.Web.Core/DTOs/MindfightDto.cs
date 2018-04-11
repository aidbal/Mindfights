using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using AutoMapper;
using Mindfights.Models;

namespace Mindfights.DTOs
{
    [AutoMapTo(typeof(Mindfight))]
    [AutoMapFrom(typeof(Mindfight))]
    public class MindfightDto
    {
        public long Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }
        [Required]
        [MaxLength(2550)]
        public string Description { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? PrepareTime { get; set; }
        public int? ToursCount { get; set; }
        public long? CreatorId { get; set; }
        public string CreatorEmail { get; set; }
        [Required]
        public int? TotalTimeLimitInMinutes { get; set; }
        [Required]
        public int TeamsLimit { get; set; }
        public int RegisteredTeamsCount { get; set; }
        public List<string> UsersAllowedToEvaluate { get; set; }
        public List<string> TeamsAllowedToParticipate { get; set; }
    }
}
