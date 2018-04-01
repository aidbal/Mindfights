using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using Mindfights.Configuration.Dto;

namespace Mindfights.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : MindfightsAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
