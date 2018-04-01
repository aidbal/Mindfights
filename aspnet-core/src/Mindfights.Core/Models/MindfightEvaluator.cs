using Mindfights.Authorization.Users;

namespace Mindfights.Models
{
    public class MindfightEvaluator
    {
        public long UserId { get; set; }
        public User User { get; set; }
        public long MindfightId { get; set; }
        public Mindfight Mindfight { get; set; }

        public MindfightEvaluator(Mindfight mindfight, User user)
        {
            Mindfight = mindfight;
            MindfightId = mindfight.Id;
            User = user;
            UserId = user.Id;
        }

        private MindfightEvaluator() {}
    }
}
