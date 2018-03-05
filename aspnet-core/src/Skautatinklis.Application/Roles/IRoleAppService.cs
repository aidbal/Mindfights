using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Skautatinklis.Roles.Dto;

namespace Skautatinklis.Roles
{
    public interface IRoleAppService : IAsyncCrudAppService<RoleDto, int, PagedResultRequestDto, CreateRoleDto, RoleDto>
    {
        Task<ListResultDto<PermissionDto>> GetAllPermissions();
    }
}
