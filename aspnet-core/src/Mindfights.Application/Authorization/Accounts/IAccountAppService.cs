using System.Threading.Tasks;
using Abp.Application.Services;
using Mindfights.Authorization.Accounts.Dto;

namespace Mindfights.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
