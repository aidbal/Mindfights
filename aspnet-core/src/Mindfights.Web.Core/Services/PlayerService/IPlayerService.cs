using System.Collections.Generic;
using Abp.Application.Services;
using Mindfights.DTOs;
using System.Threading.Tasks;

namespace Mindfights.Services.PlayerService
{
    public interface IPlayerService : IApplicationService
    {
        Task<PlayerDto> GetPlayerInfo(long userId);
        Task UpdatePlayerInfo(PlayerDto playerInfo, long userId);
        Task<List<PlayerDto>> GetAllPlayers();
    }
}
