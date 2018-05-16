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
using Mindfights.Authentication.JwtBearer;
using Mindfights.Configuration;
using Mindfights.DTOs;
using Mindfights.EntityFrameworkCore;
using Mindfights.Services.MindfightService;
using Mindfights.Services.PlayerService;
using Mindfights.Services.QuestionService;
using Mindfights.Services.RegistrationService;
using Mindfights.Services.ResultService;
using Mindfights.Services.TeamAnswerService;
using Mindfights.Services.TeamService;
using Mindfights.Services.TourService;
#if FEATURE_SIGNALR
using Abp.Web.SignalR;
#elif FEATURE_SIGNALR_ASPNETCORE
using Abp.AspNetCore.SignalR;
#endif

namespace Mindfights
{
    [DependsOn(
         typeof(MindfightsApplicationModule),
         typeof(MindfightsEntityFrameworkModule),
         typeof(AbpAspNetCoreModule)
#if FEATURE_SIGNALR 
        ,typeof(AbpWebSignalRModule)
#elif FEATURE_SIGNALR_ASPNETCORE
        , typeof(AbpAspNetCoreSignalRModule)
#endif
     )]
    public class MindfightsWebCoreModule : AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public MindfightsWebCoreModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                MindfightsConsts.ConnectionStringName
            );

            Configuration.Modules.AbpAutoMapper().Configurators.Add(config =>
            {
                config.CreateMap<MindfightCreateDto, Models.Mindfight>()
                    .ForMember(u => u.Id, (AutoMapper.IMemberConfigurationExpression<MindfightCreateDto, Models.Mindfight, long> options) => options.Ignore());
            });

            // Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            Configuration.Modules.AbpAspNetCore()
                 .CreateControllersForAppServices(
                     typeof(MindfightsApplicationModule).GetAssembly()
                 );

            ConfigureTokenAuth();

            Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(typeof(IPlayerService).Assembly, moduleName: "mindfights", useConventionalHttpVerbs: true);
            Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(typeof(ITeamService).Assembly, moduleName: "mindfights", useConventionalHttpVerbs: true);
            Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(typeof(IMindfightService).Assembly, moduleName: "mindfights", useConventionalHttpVerbs: true);
            Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(typeof(IQuestionService).Assembly, moduleName: "mindfights", useConventionalHttpVerbs: true);
            Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(typeof(ITeamAnswerService).Assembly, moduleName: "mindfights", useConventionalHttpVerbs: true);
            Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(typeof(IRegistrationService).Assembly, moduleName: "mindfights", useConventionalHttpVerbs: true);
            Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(typeof(IResultService).Assembly, moduleName: "mindfights", useConventionalHttpVerbs: true);
            Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(typeof(ITourService).Assembly, moduleName: "mindfights", useConventionalHttpVerbs: true);
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
            IocManager.RegisterAssemblyByConvention(typeof(MindfightsWebCoreModule).GetAssembly());
        }
    }
}
