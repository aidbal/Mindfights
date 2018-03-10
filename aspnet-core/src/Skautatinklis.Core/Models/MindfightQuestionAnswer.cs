using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;

namespace Skautatinklis.Models
{
    public class MindfightQuestionAnswer : Entity, IHasCreationTime, ISoftDelete
    {
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionId { get; set; }
        public MindfightQuestion Question { get; set; }

        public DateTime CreationTime { get; set; }
        public bool IsDeleted { get; set; }

        public MindfightQuestionAnswer(string text, bool isCorrect, MindfightQuestion question)
        {
            Text = text;
            IsCorrect = isCorrect;
            Question = question;
            QuestionId = question.Id;

            CreationTime = Clock.Now;
            IsDeleted = false;
        }
    }
}
