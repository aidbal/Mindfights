using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using Skautatinklis.Authorization.Users;

namespace Skautatinklis.Models
{
    public class TeamAnswer : Entity<long>, IHasCreationTime, ISoftDelete
    {
        public string EnteredAnswer { get; set; }
        public int ElapsedTimeInSeconds { get; set; }
        public bool IsEvaluated { get; set; }
        public bool IsCurrentlyEvaluated { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsDeleted { get; set; }
        public long TeamId { get; set; }
        public Team Team { get; set; }
        public long QuestionId { get; set; }
        public MindfightQuestion Question { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }

        public TeamAnswer(MindfightQuestion question, User user, Team team,
            string enteredAnswer, int elapsedTimeInSeconds, bool isEvaluated) : this()
        {
            IsEvaluated = isEvaluated;
            EnteredAnswer = enteredAnswer;
            ElapsedTimeInSeconds = elapsedTimeInSeconds;
            Team = team;
            TeamId = team.Id;
            Question = question;
            QuestionId = question.Id;
            User = user;
            UserId = user.Id;
        }

        private TeamAnswer()
        {
            IsCurrentlyEvaluated = false;
            CreationTime = Clock.Now;
        }
    }
}
