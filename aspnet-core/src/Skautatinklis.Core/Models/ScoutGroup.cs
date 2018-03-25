using Skautatinklis.Authorization.Users;
using System.Collections.Generic;

namespace Skautatinklis.Models
{
    public class ScoutGroup : BaseTeam
    {
        public ICollection<User> Users { get; set; }

        public ScoutGroup(User leaderUser, string name, string description)
            : base(leaderUser, name, description)
        {
            Users = new List<User> { leaderUser };
        }

        private ScoutGroup(){ }
    }
}
