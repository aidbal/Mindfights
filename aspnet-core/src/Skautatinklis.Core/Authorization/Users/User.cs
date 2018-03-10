using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Authorization.Users;
using Abp.Extensions;
using Skautatinklis.Models;

namespace Skautatinklis.Authorization.Users
{
    public class User : AbpUser<User>
    {
        public const string DefaultPassword = "123qwe";

        public virtual DateTime? Birthday { get; set; }
        public virtual int Points { get; set; }
        public virtual int UserType { get; set; }
        public virtual bool IsConfirmedByLeader { get; set; }
        public virtual int? TeamId { get; set; }
        public virtual Team Team { get; set; }
        public ICollection<ScoutAchievements> ScoutAchievements;
        public ICollection<Mindfight> Mindfights;
        public ICollection<UserMindfightResult> MindfightResults;

        public User()
        {
            ScoutAchievements = new List<ScoutAchievements>();
            Mindfights = new List<Mindfight>();
            MindfightResults = new List<UserMindfightResult>();
        }

        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        public static User CreateTenantAdminUser(int tenantId, string emailAddress)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = AdminUserName,
                Name = AdminUserName,
                Surname = AdminUserName,
                EmailAddress = emailAddress
            };

            user.SetNormalizedNames();

            return user;
        }
    }
}
