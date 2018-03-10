using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;

namespace Skautatinklis.Models
{
    public class TeamAnswer : Entity, IHasCreationTime, ISoftDelete
    {
        public string EnteredAnswer { get; set; }
        public int ElapsedTimeInSeconds { get; set; }
        public bool IsEvaluated { get; set; }

        public int TeamId { get; set; }
        public Team Team { get; set; }
        public int QuestionId { get; set; }
        public MindfightQuestion Question { get; set; }
        public int MindfightId { get; set; }
        public Mindfight Mindfight { get; set; }


        public DateTime CreationTime { get; set; }
        public bool IsDeleted { get; set; }

        public TeamAnswer(string enteredAnswer, int elapsedTimeInSeconds, Team team, MindfightQuestion question,
            Mindfight mindfight)
        {
            IsEvaluated = question.QuestionType == MindfightQuestionTypes.Test;
            EnteredAnswer = enteredAnswer;
            ElapsedTimeInSeconds = elapsedTimeInSeconds;
            Team = team;
            TeamId = team.Id;
            Question = question;
            QuestionId = question.Id;
            Mindfight = mindfight;
            MindfightId = mindfight.Id;

            CreationTime = Clock.Now;
            IsDeleted = false;
        }
    }
}
