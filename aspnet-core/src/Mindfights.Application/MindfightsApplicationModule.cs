using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Mindfights.Authorization;

namespace Mindfights
{
    [DependsOn(
        typeof(MindfightsCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class MindfightsApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<MindfightsAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(MindfightsApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddProfiles(thisAssembly)
            );
        }
    }
}
