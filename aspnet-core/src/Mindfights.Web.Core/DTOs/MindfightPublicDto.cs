using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Mindfights.Models;

namespace Mindfights.DTOs
{
    [AutoMapFrom(typeof(Mindfight))]
    public class MindfightPublicDto
    {
        public long Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }
        [Required]
        [MaxLength(2550)]
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public int PlayersLimit { get; set; }
        public bool IsPrivate { get; set; }
    }
}
