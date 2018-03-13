using Abp.Domain.Entities;

namespace Skautatinklis.Models
{
    public class ScoutAchievementType : Entity<long>
    {
        public string Name { get; set; }
        public string DateType { get; set; }

        public ScoutAchievementType() { }
    }
}
