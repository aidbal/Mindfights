using Abp.AutoMapper;
using Mindfights.Authorization.Users;

namespace Mindfights.DTOs
{
    [AutoMapFrom(typeof(User))]
    public class PlayerDto
    {
        public long Id { get; set; }
        public string EmailAddress { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public long TeamId { get; set; }
        public string TeamName { get; set; }
    }
}
