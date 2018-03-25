using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using Skautatinklis.Authorization.Users;
using System;
using System.Collections.Generic;

namespace Skautatinklis.Models
{
    public class Mindfight : Entity<long>, IHasCreationTime, ISoftDelete, IPassivable
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? PrepareTime { get; set; }
        public int QuestionsCount { get; set; }
        public int TotalPoints { get; set; }
        public int PlayersLimit { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsFinished { get; set; }
        public int TotalTimeLimitInMinutes { get; set; }
        public long CreatorId { get; set; }
        public User Creator { get; set; }
        public long? WinnersId { get; set; }
        public Team Winners { get; set; }
        public ICollection<MindfightEvaluators> Evaluators { get; set; }
        public ICollection<MindfightRegistration> MindfightRegistrations { get; set; }
        public ICollection<MindfightQuestion> MindfightQuestions { get; set; }
        public ICollection<MindfightAllowedTeam> AllowedTeams { get; set; }

        public Mindfight(User creator, string title, string description, int playersLimit, DateTime startTime,
            DateTime? endTime, int? prepareTime, int totalTimeLimitInMinutes, bool isPrivate) : this()
        {
            Title = title;
            Description = description;
            Evaluators = new List<MindfightEvaluators> { new MindfightEvaluators(this, creator) };
            CreatorId = creator.Id;
            PlayersLimit = playersLimit;
            StartTime = startTime;
            EndTime = endTime;
            PrepareTime = prepareTime;
            TotalTimeLimitInMinutes = totalTimeLimitInMinutes;
            IsPrivate = isPrivate;
        }

        private Mindfight()
        {
            MindfightQuestions = new List<MindfightQuestion>();
            MindfightRegistrations = new List<MindfightRegistration>();
            CreationTime = Clock.Now;
            TotalPoints = 0;
            QuestionsCount = 0;
            IsActive = true;
            IsConfirmed = false;
            IsFinished = false;
        }
    }
}
