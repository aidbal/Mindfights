using System;
using System.Collections;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using Mindfights.Authorization.Users;

namespace Mindfights.Models
{
    public class Team : Entity<long>, ISoftDelete, IHasCreationTime, IPassivable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long LeaderId { get; set; }
        public int UsersCount { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public int GamePoints { get; set; }
        public int WonMindfightsCount { get; set; }
        public ICollection<TeamAnswer> TeamAnswers { get; set; }
        public ICollection<Registration> Registrations { get; set; }
        public ICollection<User> Users { get; set; }

        public Team(User leaderUser, string name, string description)
        {
            GamePoints = 0;
            LeaderId = leaderUser.Id;
            UsersCount = 1;
            Name = name;
            Description = description;
            Users = new List<User> { leaderUser };
            TeamAnswers = new List<TeamAnswer>();
            Registrations = new List<Registration>();
        }

        private Team()
        {
            CreationTime = Clock.Now;
            IsActive = true;
        }
    }
}
