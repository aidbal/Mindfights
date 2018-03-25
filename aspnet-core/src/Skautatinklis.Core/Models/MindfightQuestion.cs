using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;

namespace Skautatinklis.Models
{
    public class MindfightQuestion : Entity<long>, IHasCreationTime, ISoftDelete, IPassivable
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }
        public int TimeToAnswerInSeconds { get; set; }
        public string AttachmentLocation { get; set; }
        public int OrderNumber { get; set; }
        public MindfightQuestionType QuestionType { get; set; }
        public long MindfightId { get; set; }
        public Mindfight Mindfight { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<MindfightQuestionAnswer> MindfightQuestionAnswers { get; set; }
        public ICollection<TeamAnswer> TeamAnswers { get; set; }

        public MindfightQuestion(Mindfight mindfight, MindfightQuestionType questionType, string title, string description,
            int timeToAnswerInSeconds, int points, int orderNumber, string attachmentLocation) : this()
        {
            MindfightId = mindfight.Id;
            Mindfight = mindfight;
            QuestionType = questionType;
            Title = title;
            Description = description;
            TimeToAnswerInSeconds = timeToAnswerInSeconds;
            Points = points;
            OrderNumber = orderNumber;
            AttachmentLocation = attachmentLocation;
        }

        private MindfightQuestion()
        {
            MindfightQuestionAnswers = new List<MindfightQuestionAnswer>();
            TeamAnswers = new List<TeamAnswer>();
            CreationTime = Clock.Now;
            IsActive = true;
        }
    }
}
