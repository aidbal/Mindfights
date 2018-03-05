using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using Skautatinklis.Configuration.Dto;

namespace Skautatinklis.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : SkautatinklisAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
