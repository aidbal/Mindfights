using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Mindfights.Configuration;

namespace Mindfights.Web.Host.Startup
{
    [DependsOn(
       typeof(MindfightsWebCoreModule))]
    public class MindfightsWebHostModule: AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public MindfightsWebHostModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MindfightsWebHostModule).GetAssembly());
        }
    }
}
