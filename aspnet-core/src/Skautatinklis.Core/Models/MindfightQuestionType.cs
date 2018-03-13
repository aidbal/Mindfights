using Abp.Domain.Entities;

namespace Skautatinklis.Models
{
    public class MindfightQuestionType : Entity<long>
    {
        public string Type { get; set; }

        public MindfightQuestionType() { }
    }
}
