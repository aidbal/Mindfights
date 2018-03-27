using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;

namespace Skautatinklis.Models
{
    public class Tour : Entity<long>, IHasCreationTime, ISoftDelete, IPassivable
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int TotalPoints { get; set; }
        public int OrderNumber { get; set; }
        public int QuestionsCount { get; set; }
        public int TimeToEnterAnswersInSeconds { get; set; }
        public long MindfightId { get; set; }
        public Mindfight Mindfight { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Question> Questions { get; set; }
        public ICollection<TeamAnswer> TeamAnswers { get; set; }

        public Tour(Mindfight mindfight, string title, string description,
            int timeToEnterAnswersInSeconds, int orderNumber) : this()
        {
            MindfightId = mindfight.Id;
            Mindfight = mindfight;
            Title = title;
            Description = description;
            OrderNumber = orderNumber;
            TimeToEnterAnswersInSeconds = timeToEnterAnswersInSeconds;
        }

        private Tour()
        {
            QuestionsCount = 0;
            TotalPoints = 0;
            Questions = new List<Question>();
            TeamAnswers = new List<TeamAnswer>();
            CreationTime = Clock.Now;
            IsActive = true;
        }
    }
}
