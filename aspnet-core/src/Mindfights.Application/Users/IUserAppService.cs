using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Mindfights.Roles.Dto;
using Mindfights.Users.Dto;

namespace Mindfights.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedResultRequestDto, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();

        Task ChangeLanguage(ChangeUserLanguageDto input);
    }
}
