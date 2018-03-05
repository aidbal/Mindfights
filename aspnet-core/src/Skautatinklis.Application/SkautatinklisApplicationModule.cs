using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Skautatinklis.Authorization;

namespace Skautatinklis
{
    [DependsOn(
        typeof(SkautatinklisCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class SkautatinklisApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<SkautatinklisAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(SkautatinklisApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddProfiles(thisAssembly)
            );
        }
    }
}
