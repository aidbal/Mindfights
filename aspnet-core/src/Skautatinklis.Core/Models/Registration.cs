using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;

namespace Skautatinklis.Models
{
    public class Registration : Entity<long>, IHasCreationTime
    {
        public long TeamId { get; set; }
        public Team Team { get; set; }
        public long MindfightId { get; set; }
        public Mindfight Mindfight { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsConfirmed { get; set; }

        public Registration(Mindfight mindfight, Team team) : this()
        {
            Team = team;
            TeamId = team.Id;
            Mindfight = mindfight;
            MindfightId = mindfight.Id;
        }

        private Registration()
        {
            IsConfirmed = false;
            CreationTime = Clock.Now;
        }
    }
}
