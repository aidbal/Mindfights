using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using Skautatinklis.Authorization.Users;

namespace Skautatinklis.Models
{
    public abstract class BaseTeam : Entity<long>, ISoftDelete, IHasCreationTime, IPassivable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long LeaderId { get; set; }
        public int UsersCount { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

        protected BaseTeam(User leaderUser, string name, string description) : this()
        {
            LeaderId = leaderUser.Id;
            UsersCount = 1;
            Name = name;
            Description = description;
        }

        protected BaseTeam()
        {
            CreationTime = Clock.Now;
            IsActive = true;
        }
    }
}
