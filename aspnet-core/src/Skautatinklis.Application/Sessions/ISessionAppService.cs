using System.Threading.Tasks;
using Abp.Application.Services;
using Skautatinklis.Sessions.Dto;

namespace Skautatinklis.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
