using System.Threading.Tasks;
using Abp.Application.Services;
using Mindfights.Sessions.Dto;

namespace Mindfights.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
