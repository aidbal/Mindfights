using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;

namespace Mindfights.Models
{
    public class Tour : Entity<long>, IHasCreationTime, ISoftDelete, IPassivable
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int OrderNumber { get; set; }
        public int TimeToEnterAnswersInSeconds { get; set; }

        public int IntroTimeInSeconds { get; set; }
        public long MindfightId { get; set; }
        public Mindfight Mindfight { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Question> Questions { get; set; }

        public Tour(Mindfight mindfight, string title, string description,
            int timeToEnterAnswersInSeconds, int introTimeInSeconds, int orderNumber) : this()
        {
            MindfightId = mindfight.Id;
            Mindfight = mindfight;
            Title = title;
            Description = description;
            OrderNumber = orderNumber;
            TimeToEnterAnswersInSeconds = timeToEnterAnswersInSeconds;
            IntroTimeInSeconds = introTimeInSeconds;
        }

        private Tour()
        {
            Questions = new List<Question>();
            CreationTime = Clock.Now;
            IsActive = true;
        }
    }
}
