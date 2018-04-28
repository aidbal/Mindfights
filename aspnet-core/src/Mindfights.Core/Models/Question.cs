using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;

namespace Mindfights.Models
{
    public class Question : Entity<long>, IHasCreationTime, ISoftDelete, IPassivable
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Answer { get; set; }
        public int Points { get; set; }
        public int TimeToAnswerInSeconds { get; set; }
        public int OrderNumber { get; set; }
        public long TourId { get; set; }
        public Tour Tour { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<TeamAnswer> TeamAnswers { get; set; }

        public Question(Tour tour, string title, string description, string answer,
            int timeToAnswerInSeconds, int points, int orderNumber) : this()
        {
            TourId = tour.Id;
            Tour = tour;
            Title = title;
            Description = description;
            Answer = answer;
            TimeToAnswerInSeconds = timeToAnswerInSeconds;
            Points = points;
            OrderNumber = orderNumber;
        }

        private Question()
        {
            TeamAnswers = new List<TeamAnswer>();
            CreationTime = Clock.Now;
            IsActive = true;
        }
    }
}
