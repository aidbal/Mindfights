using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Mindfights.Authorization.Users;

namespace Mindfights.DTOs
{
    [AutoMapFrom(typeof(User))]
    public class PlayerDto
    {
        public long Id { get; set; }
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public long CityId { get; set; }
        public string City { get; set; }
        [Required]
        public DateTime? Birthdate { get; set; }
        public int Points { get; set; }
        public long? TeamId { get; set; }
        public string TeamName { get; set; }
        public bool IsTeamLeader { get; set; }
        public bool IsActiveInTeam { get; set; }
    }
}
