using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Skautatinklis.MultiTenancy.Dto;

namespace Skautatinklis.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}
