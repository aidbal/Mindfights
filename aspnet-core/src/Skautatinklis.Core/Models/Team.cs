using System.Collections;
using System.Collections.Generic;
using Skautatinklis.Authorization.Users;

namespace Skautatinklis.Models
{
    public class Team : BaseTeam
    {
        public int GamePoints { get; set; }
        public ICollection<TeamAnswer> TeamAnswers { get; set; }
        public ICollection<MindfightRegistration> MindfightRegistrations { get; set; }
        public ICollection<MindfightAllowedTeam> AllowedPrivateMindfights { get; set; }
        public ICollection<User> Users { get; set; }

        public Team(User leaderUser, string name, string description) : base(leaderUser, name, description)
        {
            GamePoints = 0;
            Users = new List<User> { leaderUser };
            TeamAnswers = new List<TeamAnswer>();
            MindfightRegistrations = new List<MindfightRegistration>();
        }

        private Team()  { }
    }
}
