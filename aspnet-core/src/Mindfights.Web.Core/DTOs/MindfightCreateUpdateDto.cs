using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Mindfights.Models;

namespace Mindfights.DTOs
{
    [AutoMapTo(typeof(Mindfight))]
    [AutoMapFrom(typeof(Mindfight))]
    public class MindfightCreateUpdateDto
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
        public int PrepareTime { get; set; }
        [Required]
        public int TeamsLimit { get; set; }
        public List<long> UsersAllowedToEvaluateIds { get; set; }
        public List<long> TeamsAllowedToParticipateIds { get; set; }
    }
}
