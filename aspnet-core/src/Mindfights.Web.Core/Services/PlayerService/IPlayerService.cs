using Abp.Application.Services;
using System.Threading.Tasks;

namespace Mindfights.Services.PlayerService
{
    public interface IPlayerService : IApplicationService
    {
        Task<int> GetPlayerPoints(long? userId);
        Task<long> GetPlayerTeam(long userId);
    }
}
