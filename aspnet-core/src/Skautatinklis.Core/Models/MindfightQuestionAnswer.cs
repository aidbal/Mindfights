using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;

namespace Skautatinklis.Models
{
    public class MindfightQuestionAnswer : Entity<long>, IHasCreationTime, ISoftDelete
    {
        public string Answer { get; set; }
        public bool IsCorrect { get; set; }
        public long QuestionId { get; set; }
        public MindfightQuestion Question { get; set; }

        public DateTime CreationTime { get; set; }
        public bool IsDeleted { get; set; }


        private MindfightQuestionAnswer()
        {
            CreationTime = Clock.Now;
        }

        public MindfightQuestionAnswer(MindfightQuestion question, string answer, bool isCorrect) : this()
        {
            Answer = answer;
            IsCorrect = isCorrect;
            Question = question;
            QuestionId = question.Id;
        }

    }
}
