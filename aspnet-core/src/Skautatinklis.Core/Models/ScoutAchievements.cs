using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using Skautatinklis.Authorization.Users;

namespace Skautatinklis.Models
{
    public class ScoutAchievements : Entity, IHasCreationTime
    {
        public ScoutAchievementTypes Type { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreationTime { get; set; }
        public int? Hours { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }

        public ScoutAchievements()
        {
            CreationTime = Clock.Now;
        }

        public ScoutAchievements(ScoutAchievementTypes type, DateTime date, int? hours, User user)
        {
            Type = type;
            Date = date;
            Hours = hours;
            User = user;
            UserId = user.Id;
        }
    }
}
