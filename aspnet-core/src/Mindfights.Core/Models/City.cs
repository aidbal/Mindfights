using Abp.Domain.Entities;

namespace Mindfights.Models
{
    public class City : Entity<long>
    {
        public string Name { get; set; }

        public City() { }
    }
}
