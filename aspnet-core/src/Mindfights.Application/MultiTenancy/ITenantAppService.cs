using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Mindfights.MultiTenancy.Dto;

namespace Mindfights.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}
