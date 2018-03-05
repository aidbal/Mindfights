using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Skautatinklis.Roles.Dto;
using Skautatinklis.Users.Dto;

namespace Skautatinklis.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedResultRequestDto, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();

        Task ChangeLanguage(ChangeUserLanguageDto input);
    }
}
