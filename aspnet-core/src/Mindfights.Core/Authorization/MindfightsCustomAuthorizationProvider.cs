using Abp.Authorization;
using Abp.Localization;

namespace Mindfights.Authorization
{
    public class MindfightsCustomAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission("CreateMindfights", L("Kurti protmūšius"));
            context.CreatePermission("ManageMindfights", L("valdyti protmūšius"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, MindfightsConsts.LocalizationSourceName);
        }
    }
}
