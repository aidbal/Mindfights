using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Skautatinklis.Authorization.Users;
using System;
using System.Collections.Generic;
using Abp.Timing;

namespace Skautatinklis.Models
{
    public sealed class MindfightResult : Entity, IHasCreationTime, ISoftDelete
    {
        public int Points { get; set; }
        public bool IsEvaluated { get; set; }

        public int? TeamId { get; set; }
        public Team Team { get; set; }
        public int? MindfightId { get; set; }
        public Mindfight Mindfight { get; set; }
        public ICollection<UserMindfightResult> Players { get; set; }

        public DateTime CreationTime { get; set; }
        public bool IsDeleted { get; set; }

        public MindfightResult(int points, bool isEvaluated, Team team, Mindfight mindfight)
        {
            Points = points;
            IsEvaluated = isEvaluated;
            Team = team;
            TeamId = team.Id;
            Mindfight = mindfight;
            MindfightId = mindfight.Id;
            Players = new List<UserMindfightResult>();
            foreach(var player in team.Players)
            {
                var userMindfightResult = new UserMindfightResult
                {
                    User = player,
                    UserId = player.Id,
                    MindfightResult = this,
                    MindfightResultId = Id
                };
                Players.Add(userMindfightResult);
            }

            CreationTime = Clock.Now;
            IsDeleted = false;
        }
    }
}
