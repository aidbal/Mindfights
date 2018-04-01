using System;
using System.ComponentModel.DataAnnotations;

namespace Mindfights.DTOs
{
    public class TeamAnswerDto
    {
        [Required]
        public string EnteredAnswer { get; set; }
        public int ElapsedTimeInSeconds { get; set; }
        public int EarnedPoints { get; set; }
        public bool IsEvaluated { get; set; }
        public bool IsCurrentlyEvaluated { get; set; }
        public DateTime CreationTime { get; set; }
        public string EvaluatorComment { get; set; }
        public long TeamId { get; set; }
        public long QuestionId { get; set; }
        public long UserId { get; set; }
    }
}
