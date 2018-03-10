using Skautatinklis.Authorization.Users;

namespace Skautatinklis.Models
{
    public class UserMindfightResult
    {
        public long UserId { get; set; }
        public User User { get; set; }
        public int MindfightResultId { get; set; }
        public MindfightResult MindfightResult { get; set; }
    }
}
