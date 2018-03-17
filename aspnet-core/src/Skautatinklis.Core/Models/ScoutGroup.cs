using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using Skautatinklis.Authorization.Users;

namespace Skautatinklis.Models
{
    public class ScoutGroup : BaseTeam
    {
        public ICollection<User> Users { get; set; }

        public ScoutGroup(User leaderUser, string name, string description)
            : base(leaderUser, name, description)
        {
            Users = new List<User> { leaderUser };
        }

        private ScoutGroup(){ }

        public void AddPlayer(User user)
        {
            Users.Add(user);
        }

        public void RemovePlayer(User user)
        {
            Users.Remove(user);
        }
    }
}
