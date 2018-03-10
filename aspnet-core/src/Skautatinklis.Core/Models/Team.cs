using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using Skautatinklis.Authorization.Users;

namespace Skautatinklis.Models
{
    public class Team : Entity, IHasCreationTime, ISoftDelete, IPassivable
    {
        public const int MaxNameLength = 256;

        public User LeaderUser { get; set; }
        [Required]
        [MaxLength(MaxNameLength)]
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public int GamePoints { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int PlayersCount { get; set; }
        public ICollection<User> Players;
        public ICollection<MindfightRegistration> MindfightRegistrations;

        public Team()
        {
            CreationTime = Clock.Now;
            Players = new List<User>();
            MindfightRegistrations = new List<MindfightRegistration>();
        }

        public Team(User leaderUser, string name)
            : this()
        {
            LeaderUser = leaderUser;
            Name = name;
            GamePoints = 0;
            IsActive = true;
            PlayersCount = 1;
        }

        public void AddPlayer(User player)
        {
            Players.Add(player);
            PlayersCount += 1;
        }

        public void RemovePlayer(User player)
        {
            if (Players.Remove(player))
            {
                PlayersCount -= 1;
            }
        }
    }
}
