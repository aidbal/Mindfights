using System;
using System.Collections;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using Skautatinklis.Authorization.Users;

namespace Skautatinklis.Models
{
    public class Mindfight : Entity, IHasCreationTime, ISoftDelete, IPassivable
    {
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? TimeToStartInMinutes { get; set; }
        public int QuestionsCount { get; set; }
        public int OverallPoints { get; set; }
        public int PlayersLimit { get; set; }

        public DateTime CreationTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public long CreatorId { get; set; }
        public User Creator { get; set; }
        public ICollection<MindfightQuestionMindfight> Questions;
        public ICollection<MindfightRegistration> MindfightRegistrations;

        public Mindfight(User user, int playersLimit, DateTime startTime)
        {
            CreationTime = Clock.Now;
            IsActive = true;
            IsDeleted = false;
            Creator = user;
            CreatorId = user.Id;
            Questions = new List<MindfightQuestionMindfight>();
            MindfightRegistrations = new List<MindfightRegistration>();

            PlayersLimit = playersLimit;
            StartTime = startTime;
            OverallPoints = 0;
            QuestionsCount = 0;
        }

        public Mindfight(User user, int playersLimit, DateTime startTime, DateTime endTime) : this(user, playersLimit, startTime)
        {
            EndTime = endTime;
        }

        public Mindfight(User user, int playersLimit, DateTime startTime, int timeToStartInMinutes) : this(user, playersLimit, startTime)
        {
            TimeToStartInMinutes = timeToStartInMinutes;
        }
    }
}
