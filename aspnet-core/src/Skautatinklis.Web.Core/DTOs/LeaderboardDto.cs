namespace Skautatinklis.DTOs
{
    public class LeaderBoardDto
    {
        public long TeamId { get; set; }
        public string TeamName { get; set; }
        public int PlayedMindfightsCount { get; set; }
        public int WonMindfightsCount { get; set; }
        public int EarnedPoints { get; set; }
    }
}
