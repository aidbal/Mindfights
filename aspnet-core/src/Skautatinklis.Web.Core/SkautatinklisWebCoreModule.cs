using System;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.Configuration;
using Skautatinklis.Authentication.JwtBearer;
using Skautatinklis.Authorization.Users;
using Skautatinklis.Configuration;
using Skautatinklis.DTOs;
using Skautatinklis.EntityFrameworkCore;
using Skautatinklis.Models;
using Skautatinklis.Services.MindfightService;
using Skautatinklis.Services.PlayerService;
using Skautatinklis.Services.QuestionAnswerService;
using Skautatinklis.Services.QuestionService;
using Skautatinklis.Services.RegistrationService;
using Skautatinklis.Services.ResultService;
using Skautatinklis.Services.TeamAnswerService;
using Skautatinklis.Services.TeamService;
#if FEATURE_SIGNALR
using Abp.Web.SignalR;
#elif FEATURE_SIGNALR_ASPNETCORE
using Abp.AspNetCore.SignalR;
#endif

namespace Skautatinklis
{
    [DependsOn(
         typeof(SkautatinklisApplicationModule),
         typeof(SkautatinklisEntityFrameworkModule),
         typeof(AbpAspNetCoreModule)
#if FEATURE_SIGNALR 
        ,typeof(AbpWebSignalRModule)
#elif FEATURE_SIGNALR_ASPNETCORE
        , typeof(AbpAspNetCoreSignalRModule)
#endif
     )]
    public class SkautatinklisWebCoreModule : AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public SkautatinklisWebCoreModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                SkautatinklisConsts.ConnectionStringName
            );

            Configuration.Modules.AbpAutoMapper().Configurators.Add(config =>
            {
                config.CreateMap<MindfightCreateUpdateDto, Mindfight>()
                    .ForMember(u => u.Id, options => options.Ignore());
            });

            // Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            Configuration.Modules.AbpAspNetCore()
                 .CreateControllersForAppServices(
                     typeof(SkautatinklisApplicationModule).GetAssembly()
                 );

            ConfigureTokenAuth();

            Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(typeof(IPlayerService).Assembly, moduleName: "mindfights", useConventionalHttpVerbs: true);
            Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(typeof(ITeamService).Assembly, moduleName: "mindfights", useConventionalHttpVerbs: true);
            //Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(typeof(IScoutGroupService).Assembly, moduleName: "mindfights", useConventionalHttpVerbs: true);
            Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(typeof(IMindfightService).Assembly, moduleName: "mindfights", useConventionalHttpVerbs: true);
            Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(typeof(IQuestionService).Assembly, moduleName: "mindfights", useConventionalHttpVerbs: true);
            Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(typeof(IAnswerService).Assembly, moduleName: "mindfights", useConventionalHttpVerbs: true);
            Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(typeof(ITeamAnswerService).Assembly, moduleName: "mindfights", useConventionalHttpVerbs: true);
            Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(typeof(IRegistrationService).Assembly, moduleName: "mindfights", useConventionalHttpVerbs: true);
            Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(typeof(IResultService).Assembly, moduleName: "mindfights", useConventionalHttpVerbs: true);
        }

        private void ConfigureTokenAuth()
        {
            IocManager.Register<TokenAuthConfiguration>();
            var tokenAuthConfig = IocManager.Resolve<TokenAuthConfiguration>();

            tokenAuthConfig.SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appConfiguration["Authentication:JwtBearer:SecurityKey"]));
            tokenAuthConfig.Issuer = _appConfiguration["Authentication:JwtBearer:Issuer"];
            tokenAuthConfig.Audience = _appConfiguration["Authentication:JwtBearer:Audience"];
            tokenAuthConfig.SigningCredentials = new SigningCredentials(tokenAuthConfig.SecurityKey, SecurityAlgorithms.HmacSha256);
            tokenAuthConfig.Expiration = TimeSpan.FromDays(1);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(SkautatinklisWebCoreModule).GetAssembly());
        }
    }
}
