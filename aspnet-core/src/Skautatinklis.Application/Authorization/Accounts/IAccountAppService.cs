using System.Threading.Tasks;
using Abp.Application.Services;
using Skautatinklis.Authorization.Accounts.Dto;

namespace Skautatinklis.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
