using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Skautatinklis.Models;

namespace Skautatinklis.DTOs
{
    [AutoMapFrom(typeof(ScoutGroup))]
    public class ScoutGroupDto
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
    }
}
