using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;

namespace Mindfights.Models
{
    public class Answer : Entity<long>, IHasCreationTime, ISoftDelete
    {
        public string Description { get; set; }
        public bool IsCorrect { get; set; }
        public long QuestionId { get; set; }
        public Question Question { get; set; }

        public DateTime CreationTime { get; set; }
        public bool IsDeleted { get; set; }

        public Answer(Question question, string description, bool isCorrect) : this()
        {
            Description = description;
            IsCorrect = isCorrect;
            Question = question;
            QuestionId = question.Id;
        }
        
        private Answer()
        {
            CreationTime = Clock.Now;
        }
    }
}
