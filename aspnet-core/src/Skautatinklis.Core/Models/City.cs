using Abp.Domain.Entities;

namespace Skautatinklis.Models
{
    public class City : Entity<long>
    {
        public string Name { get; set; }

        public City() { }
    }
}
