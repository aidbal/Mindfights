namespace Mindfights.DTOs
{
    public class TeamPlayerDto
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public int Points { get; set; }
        public bool IsActiveInTeam { get; set; }
        public bool IsTeamLeader { get; set; }
    }
}
