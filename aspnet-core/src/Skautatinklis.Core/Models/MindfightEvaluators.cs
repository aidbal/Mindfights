using Skautatinklis.Authorization.Users;

namespace Skautatinklis.Models
{
    public class MindfightEvaluators
    {
        public long UserId { get; set; }
        public User User { get; set; }
        public long MindfightId { get; set; }
        public Mindfight Mindfight { get; set; }

        public MindfightEvaluators(Mindfight mindfight, User user)
        {
            Mindfight = mindfight;
            MindfightId = mindfight.Id;
            User = user;
            UserId = user.Id;
        }

        private MindfightEvaluators() {}
    }
}
