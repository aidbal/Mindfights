using Abp.AutoMapper;
using Skautatinklis.Models;
using System.ComponentModel.DataAnnotations;

namespace Skautatinklis.DTOs
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
        public string LeaderName { get; set; }
        public int? UsersCount { get; set; }
        public int? GamePoints { get; set; }
    }
}
