using Abp.Domain.Entities;
using Skautatinklis.Authorization.Users;

namespace Skautatinklis.Models
{
    public class UserMindfightResult : ISoftDelete
    {
        public long UserId { get; set; }
        public User User { get; set; }
        public long MindfightResultId { get; set; }
        public MindfightResult MindfightResult { get; set; }
        public bool IsDeleted { get; set; }

        public UserMindfightResult(User user, MindfightResult mindfightResult)
        {
            User = user;
            UserId = user.Id;
            MindfightResult = mindfightResult;
            MindfightResultId = mindfightResult.Id;
        }

        private UserMindfightResult() { }
    }
}
