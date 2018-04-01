using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mindfights.Models;

namespace Mindfights.Services.PlayerService
{
    public interface IPlayerService : IApplicationService
    {
        Task<int> GetPlayerPoints(long? userId);

        Task<string> GetTeam(long userId);
        //void ConfirmUser(long userId);
        //Task<string> GetTeam(long userId);
        //void RemoveFromTeam(long userId);
        //Task<List<Mindfight>> GetAllowedEvaluateMindfights(long userId);
        //Task<Mindfight> GetPlayedMindfightsHistory(long userId);
        //Task<List<TeamAnswer>> GetPlayedMindfightAnswers(long userId, long mindfightId);
    }
}
