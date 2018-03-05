using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Skautatinklis.Configuration;

namespace Skautatinklis.Web.Host.Startup
{
    [DependsOn(
       typeof(SkautatinklisWebCoreModule))]
    public class SkautatinklisWebHostModule: AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public SkautatinklisWebHostModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(SkautatinklisWebHostModule).GetAssembly());
        }
    }
}
