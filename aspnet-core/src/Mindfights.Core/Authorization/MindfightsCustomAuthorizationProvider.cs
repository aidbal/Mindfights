using Abp.Authorization;
using Abp.Localization;

namespace Mindfights.Authorization
{
    public class MindfightsCustomAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission("CreateMindfights", L("Create mindfights"));
            context.CreatePermission("ManageMindfights", L("Manage mindfights"));
            context.CreatePermission("ConfirmLeaders", L("Confirm leaders"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, MindfightsConsts.LocalizationSourceName);
        }
    }
}
