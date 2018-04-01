using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;

namespace Mindfights.Models
{
    public sealed class MindfightResult : Entity<long>, IHasCreationTime, ISoftDelete
    {
        public int EarnedPoints { get; set; }
        public bool IsEvaluated { get; set; }
        public int Place { get; set; }
        public long TeamId { get; set; }
        public Team Team { get; set; }
        public long MindfightId { get; set; }
        public Mindfight Mindfight { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<UserMindfightResult> Users { get; set; }

        public MindfightResult(int earnedPoints, bool isEvaluated, Team team, Mindfight mindfight) : this()
        {
            EarnedPoints = earnedPoints;
            IsEvaluated = isEvaluated;
            Team = team;
            TeamId = team.Id;
            Mindfight = mindfight;
            MindfightId = mindfight.Id;
        }

        private MindfightResult()
        {
            Place = 0;
            Users = new List<UserMindfightResult>();
            CreationTime = Clock.Now;
        }
    }
}
