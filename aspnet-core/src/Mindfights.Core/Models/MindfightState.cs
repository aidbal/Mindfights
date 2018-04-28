using System;

namespace Mindfights.Models
{
    public class MindfightState
    {
        public long TeamId { get; set; }
        public Team Team { get; set; }
        public long MindfightId { get; set; }
        public Mindfight Mindfight { get; set; }
        public long CurrentTourId { get; set; }
        public long? CurrentQuestionId { get; set; }
        public DateTime ChangeTime { get; set; }

        public MindfightState(Mindfight mindfight, Team team)
        {
            Mindfight = mindfight;
            MindfightId = mindfight.Id;
            Team = team;
            TeamId = team.Id;
        }

        private MindfightState() { }
    }
}
