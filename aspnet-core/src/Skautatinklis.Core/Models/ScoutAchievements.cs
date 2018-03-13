using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using Skautatinklis.Authorization.Users;

namespace Skautatinklis.Models
{
    public class ScoutAchievements : Entity, IHasCreationTime, ISoftDelete
    {
        public ScoutAchievementType Type { get; set; }
        public DateTime? DateAchieved { get; set; }
        public DateTime CreationTime { get; set; }
        public string Description { get; set; }
        public int? Hours { get; set; }
        public bool IsDeleted { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
        public bool IsConfirmed { get; set; }
        public long ConfirmedByUserId { get; set; }

        public ScoutAchievements(ScoutAchievementType type, DateTime dateAchieved, int? hours, string description, User user) : this()
        {
            Type = type;
            DateAchieved = dateAchieved;
            Hours = hours;
            Description = description;
            User = user;
            UserId = user.Id;
        }

        public void ConfirmAchievement(User confirmedByUser)
        {
            IsConfirmed = true;
            ConfirmedByUserId = ConfirmedByUserId;
        }

        private ScoutAchievements()
        {
            CreationTime = Clock.Now;
            IsConfirmed = false;
        }

    }
}
