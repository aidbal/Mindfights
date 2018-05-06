using Abp.Application.Services;
using Mindfights.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mindfights.Services.TeamService
{
    public interface ITeamService : IApplicationService
    {
        Task<long> CreateTeam(TeamDto team);
        Task<TeamDto> GetTeam(long teamId);
        Task<List<TeamDto>> GetAllTeams();
        Task UpdateTeam(TeamDto team, long teamId);
        Task DeleteTeam(long teamId);
        Task UpdateUserActiveStatus(long userId, bool status);
        Task InsertUser(long teamId, string username);
        Task RemoveUser(long teamId, long userId);
        Task<List<TeamPlayerDto>> GetAllTeamPlayers(long teamId);
        Task ChangeTeamLeader(long teamId, long newLeaderId);
        Task LeaveCurrentTeam();
    }
}
