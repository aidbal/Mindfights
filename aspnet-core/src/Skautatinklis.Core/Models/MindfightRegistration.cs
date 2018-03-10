using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using Abp.Timing;
using Skautatinklis.Authorization.Users;

namespace Skautatinklis.Models
{
    public class MindfightRegistration : Entity, IHasCreationTime, ISoftDelete
    {
        public int TeamId { get; set; }
        public Team Team { get; set; }
        public int MindfightId { get; set; }
        public Mindfight Mindfight { get; set; }

        public DateTime CreationTime { get; set; }
        public bool IsDeleted { get; set; }

        public MindfightRegistration(Team team, Mindfight mindfight)
        {
            Team = team;
            TeamId = team.Id;
            Mindfight = mindfight;
            MindfightId = mindfight.Id;

            CreationTime = Clock.Now;
        }
    }
}
