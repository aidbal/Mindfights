using Abp.AutoMapper;
using Mindfights.Models;
using System.ComponentModel.DataAnnotations;

namespace Mindfights.DTOs
{
    [AutoMapFrom(typeof(Team))]
    public class TeamDto
    {
        public long? Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required]
        [MaxLength(255)]
        public string Description { get; set; }
        public long LeaderId { get; set; }
        public string LeaderName { get; set; }
        public int? UsersCount { get; set; }
        public int? GamePoints { get; set; }
    }
}
