using System;
using Abp.AutoMapper;
using Mindfights.Models;

namespace Mindfights.DTOs
{
    [AutoMapFrom(typeof(MindfightResult))]
    public class MindfightResultDto
    {
        public long MindfightId { get; set; }
        public long TeamId { get; set; }
        public string MindfightName { get; set; }
        public string TeamName { get; set; }
        public DateTime MindfightStartTime { get; set; }
        public DateTime? MindfightEndTime { get; set; }
        public int ToursCount { get; set; }
        public int TotalPoints { get; set; }
        public int EarnedPoints { get; set; }
        public bool IsEvaluated { get; set; }
        public DateTime CreationTime { get; set; }
        public int Place { get; set; }
    }
}
