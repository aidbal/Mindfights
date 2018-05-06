using Abp.AutoMapper;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Mindfights.Authorization.Users;
using Mindfights.DTOs;
using System.Threading.Tasks;

namespace Mindfights.Services.PlayerService
{
    public class Player : IPlayerService
    {
        private readonly UserManager _userManager;

        public Player(UserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<PlayerDto> GetPlayerInfo(long userId)
        {
            var playerDto = new PlayerDto();
            var player = await _userManager.Users
                .IgnoreQueryFilters()
                .Include(x => x.Team)
                .Include(x => x.City)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (player == null)
            {
                throw new UserFriendlyException("Vartotojas su nurodytu id neegzistuoja!");
            }
            player.MapTo(playerDto);
            playerDto.City = player.City == null ? string.Empty : player.City.Name;
            if (player.Team != null)
            {
                playerDto.TeamId = player.Team.Id;
                playerDto.IsTeamLeader = player.Team.LeaderId == player.Id;
                playerDto.IsActiveInTeam = player.IsActiveInTeam;
            }
            return playerDto;
        }
    }
}

