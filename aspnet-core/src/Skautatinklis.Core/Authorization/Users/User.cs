using Abp.Authorization.Users;
using Abp.Extensions;
using Skautatinklis.Models;
using System;
using System.Collections.Generic;

namespace Skautatinklis.Authorization.Users
{
    public class User : AbpUser<User>
    {
        public const string DefaultPassword = "123qwe";

        public DateTime? Birthdate { get; set; }
        public long? CityId { get; set; }
        public City City { get; set; }
        public int Points { get; set; }
        public long? ScoutGroupId { get; set; }
        public ScoutGroup ScoutGroup { get; set; }
        public ICollection<ScoutAchievements> ScoutAchievements { get; set; }
        public ICollection<MindfightEvaluators> Mindfights { get; set; }
        public ICollection<TeamAnswer> TeamAnswers { get; set; }
        public ICollection<UserMindfightResult> MindfightResults { get; set; }

        public User()
        {
            ScoutAchievements = new List<ScoutAchievements>();
            Mindfights = new List<MindfightEvaluators>();
            MindfightResults = new List<UserMindfightResult>();
            TeamAnswers = new List<TeamAnswer>();
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
