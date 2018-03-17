using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using Skautatinklis.Models;

namespace Skautatinklis.DTOs
{
    [AutoMapFrom(typeof(Team))]
    public class TeamDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string LeaderName { get; set; }
        public int? PlayersCount { get; set; }
        public int? GamePoints { get; set; }
    }
}
