using System;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Timing;
using Abp.Zero;
using Abp.Zero.Configuration;
using Mindfights.Authorization;
using Mindfights.Authorization.Roles;
using Mindfights.Authorization.Users;
using Mindfights.Configuration;
using Mindfights.Localization;
using Mindfights.MultiTenancy;
using Mindfights.Timing;

namespace Mindfights
{
    [DependsOn(typeof(AbpZeroCoreModule))]
    public class MindfightsCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            AppContext.SetSwitch("Microsoft.EntityFrameworkCore.Issue9825", true);
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            // Declare entity types
            Configuration.Modules.Zero().EntityTypes.Tenant = typeof(Tenant);
            Configuration.Modules.Zero().EntityTypes.Role = typeof(Role);
            Configuration.Modules.Zero().EntityTypes.User = typeof(User);

            MindfightsLocalizationConfigurer.Configure(Configuration.Localization);

            // Enable this line to create a multi-tenant application.
            Configuration.MultiTenancy.IsEnabled = MindfightsConsts.MultiTenancyEnabled;

            // Configure roles
            AppRoleConfig.Configure(Configuration.Modules.Zero().RoleManagement);

            Configuration.Settings.Providers.Add<AppSettingProvider>();

            Configuration.Authorization.Providers.Add<MindfightsCustomAuthorizationProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MindfightsCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<AppTimes>().StartupTime = Clock.Now;
        }
    }
}
