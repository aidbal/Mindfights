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

        public int Points { get; set; }
        public DateTime? Birthdate { get; set; }
        public long? CityId { get; set; }
        public City City { get; set; }
        public long? TeamId { get; set; }
        public Team Team { get; set; }
        public ICollection<MindfightEvaluator> Mindfights { get; set; }
        public ICollection<UserMindfightResult> MindfightResults { get; set; }
        public ICollection<TeamAnswer> EvaluatedAnswers { get; set; }

        public User()
        {
            EvaluatedAnswers = new List<TeamAnswer>();
            Mindfights = new List<MindfightEvaluator>();
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
