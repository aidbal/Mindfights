using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using Mindfights.Authorization.Users;

namespace Mindfights.Models
{
    public class TeamAnswer : Entity<long>, IHasCreationTime, ISoftDelete
    {
        public string EnteredAnswer { get; set; }
        public int EarnedPoints { get; set; }
        public bool IsEvaluated { get; set; }
        public bool IsCurrentlyEvaluated { get; set; }
        public string EvaluatorComment { get; set; }

        public DateTime CreationTime { get; set; }
        public bool IsDeleted { get; set; }
        public long TeamId { get; set; }
        public Team Team { get; set; }
        public long QuestionId { get; set; }
        public Question Question { get; set; }
        public long? EvaluatorId { get; set; }
        public User Evaluator { get; set; }

        public TeamAnswer(Question question, Team team,
            string enteredAnswer, bool isEvaluated) : this()
        {
            IsEvaluated = isEvaluated;
            EnteredAnswer = enteredAnswer;
            Team = team;
            TeamId = team.Id;
            Question = question;
            QuestionId = question.Id;
        }

        private TeamAnswer()
        {
            IsCurrentlyEvaluated = false;
            CreationTime = Clock.Now;
            EarnedPoints = 0;
        }
    }
}
