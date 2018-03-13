using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using Skautatinklis.Authorization.Users;

namespace Skautatinklis.Models
{
    public class ScoutGroup : Entity<long>, ISoftDelete, IHasCreationTime, IPassivable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int PlayersCount { get; set; }
        public long LeaderId { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public ICollection<User> Users { get; set; }

        public ScoutGroup(User leaderUser, string name, string description)
            : this()
        {
            Users = new List<User> { leaderUser };
            LeaderId = leaderUser.Id;
            Name = name;
            Description = description;
        }

        protected ScoutGroup()
        {
            CreationTime = Clock.Now;
            PlayersCount = 1;
            IsActive = true;
        }

        public void AddPlayer(User user)
        {
            Users.Add(user);
            PlayersCount += 1;
        }

        public void RemovePlayer(User user)
        {
            if (Users.Remove(user))
            {
                PlayersCount -= 1;
            }
        }
    }
}
