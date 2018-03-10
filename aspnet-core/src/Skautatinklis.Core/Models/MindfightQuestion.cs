using System;
using System.Collections;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;

namespace Skautatinklis.Models
{
    public class MindfightQuestion : Entity, IHasCreationTime, ISoftDelete, IPassivable
    {
        public MindfightQuestionTypes QuestionType { get; set; }
        public int Points { get; set; }
        public int TimeToAnswerInSeconds { get; set; }
        public string AttachmentLocation { get; set; }
        public ICollection<MindfightQuestionMindfight> Mindfights;

        public DateTime CreationTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public MindfightQuestion(MindfightQuestionTypes questionType, int points, int timeToAnswerInSeconds)
        {
            QuestionType = questionType;
            Points = points;
            TimeToAnswerInSeconds = timeToAnswerInSeconds;
            Mindfights = new List<MindfightQuestionMindfight>();

            CreationTime = Clock.Now;
            IsActive = true;
            IsDeleted = false;
        }

        public MindfightQuestion(MindfightQuestionTypes questionType, int points, int timeToAnswerInSeconds,
            string attachmentLocation) : this(questionType, points, timeToAnswerInSeconds)
        {
            AttachmentLocation = attachmentLocation;
        }
    }
}
