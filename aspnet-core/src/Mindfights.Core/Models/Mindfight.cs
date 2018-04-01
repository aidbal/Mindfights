using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using Mindfights.Authorization.Users;
using System;
using System.Collections.Generic;

namespace Mindfights.Models
{
    public class Mindfight : Entity<long>, IHasCreationTime, ISoftDelete, IPassivable
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? PrepareTime { get; set; }
        public int ToursCount { get; set; }
        public int TotalPoints { get; set; }
        public int PlayersLimit { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsFinished { get; set; }
        public int TotalTimeLimitInMinutes { get; set; }
        public long CreatorId { get; set; }
        public User Creator { get; set; }
        public ICollection<MindfightEvaluator> Evaluators { get; set; }
        public ICollection<Registration> Registrations { get; set; }
        public ICollection<Tour> Tours { get; set; }

        public Mindfight(User creator, string title, string description, int playersLimit, DateTime startTime,
            DateTime? endTime, int? prepareTime, int totalTimeLimitInMinutes, bool isPrivate) : this()
        {
            Title = title;
            Description = description;
            Evaluators = new List<MindfightEvaluator> { new MindfightEvaluator(this, creator) };
            CreatorId = creator.Id;
            PlayersLimit = playersLimit;
            StartTime = startTime;
            EndTime = endTime;
            PrepareTime = prepareTime;
            TotalTimeLimitInMinutes = totalTimeLimitInMinutes;
        }

        private Mindfight()
        {
            Tours = new List<Tour>();
            Registrations = new List<Registration>();
            CreationTime = Clock.Now;
            TotalPoints = 0;
            ToursCount = 0;
            IsActive = true;
            IsConfirmed = false;
            IsFinished = false;
        }
    }
}
