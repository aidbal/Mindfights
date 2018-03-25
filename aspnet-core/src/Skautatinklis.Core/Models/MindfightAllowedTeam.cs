namespace Skautatinklis.Models
{
    public class MindfightAllowedTeam
    {
        public long TeamId { get; set; }
        public Team Team { get; set; }
        public long MindfightId { get; set; }
        public Mindfight Mindfight { get; set; }

        public MindfightAllowedTeam(Mindfight mindfight, Team team)
        {
            Mindfight = mindfight;
            MindfightId = mindfight.Id;
            Team = team;
            TeamId = team.Id;
        }

        private MindfightAllowedTeam() { }
    }
}
