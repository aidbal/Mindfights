using Skautatinklis.Authorization.Users;

namespace Skautatinklis.Models
{
    public class UserMindfightResult
    {
        public long UserId { get; set; }
        public User User { get; set; }
        public long MindfightResultId { get; set; }
        public MindfightResult MindfightResult { get; set; }

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
