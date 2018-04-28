using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Mindfights.Models;

namespace Mindfights.DTOs
{
    [AutoMapFrom(typeof(TeamAnswer))]
    public class TeamAnswerDto
    {
        [Required]
        public string EnteredAnswer { get; set; }
        public int EarnedPoints { get; set; }
        public bool IsEvaluated { get; set; }
        public bool IsCurrentlyEvaluated { get; set; }
        public DateTime CreationTime { get; set; }
        public string EvaluatorComment { get; set; }
        public string QuestionTitle { get; set; }
        public long TeamId { get; set; }
        public long QuestionId { get; set; }
        public long UserId { get; set; }
    }
}
