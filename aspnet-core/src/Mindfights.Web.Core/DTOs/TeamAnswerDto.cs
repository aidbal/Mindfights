using Abp.AutoMapper;
using Mindfights.Models;
using System;

namespace Mindfights.DTOs
{
    [AutoMapFrom(typeof(TeamAnswer))]
    public class TeamAnswerDto
    {
        public string EnteredAnswer { get; set; }
        public int EarnedPoints { get; set; }
        public bool IsEvaluated { get; set; }
        public DateTime CreationTime { get; set; }
        public string EvaluatorComment { get; set; }
        public string QuestionTitle { get; set; }
        public string Evaluator { get; set; }
        public long TeamId { get; set; }
        public long QuestionId { get; set; }
        public int TourOrderNumber { get; set; }
        public long UserId { get; set; }
    }
}
