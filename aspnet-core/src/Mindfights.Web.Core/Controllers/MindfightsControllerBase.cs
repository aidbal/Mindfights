using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace Mindfights.Controllers
{
    public abstract class MindfightsControllerBase: AbpController
    {
        protected MindfightsControllerBase()
        {
            LocalizationSourceName = MindfightsConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
