using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace Skautatinklis.Controllers
{
    public abstract class SkautatinklisControllerBase: AbpController
    {
        protected SkautatinklisControllerBase()
        {
            LocalizationSourceName = SkautatinklisConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
