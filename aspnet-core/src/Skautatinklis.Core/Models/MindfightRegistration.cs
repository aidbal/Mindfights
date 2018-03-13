using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;

namespace Skautatinklis.Models
{
    public class MindfightRegistration : Entity<long>, IHasCreationTime, ISoftDelete
    {
        public long TeamId { get; set; }
        public Team Team { get; set; }
        public long MindfightId { get; set; }
        public Mindfight Mindfight { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsDeleted { get; set; }

        public MindfightRegistration(Mindfight mindfight, Team team) : this()
        {
            Team = team;
            TeamId = team.Id;
            Mindfight = mindfight;
            MindfightId = mindfight.Id;
        }

        private MindfightRegistration()
        {
            CreationTime = Clock.Now;
        }
    }
}
