using Abp.AutoMapper;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Modules;
using Abp.Net.Mail;
using Abp.TestBase;
using Abp.Zero.Configuration;
using Abp.Zero.EntityFrameworkCore;
using Castle.MicroKernel.Registration;
using Mindfights.Authorization.Users;
using Mindfights.DTOs;
using Mindfights.EntityFrameworkCore;
using Mindfights.Tests.DependencyInjection;
using NSubstitute;
using System;
using Mindfights.Models;

namespace Mindfights.Tests
{
    [DependsOn(
        typeof(MindfightsApplicationModule),
        typeof(MindfightsEntityFrameworkModule),
        typeof(AbpTestBaseModule)
        )]
    public class MindfightsTestModule : AbpModule
    {
        public MindfightsTestModule(MindfightsEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        }

        public override void PreInitialize()
        {
            Configuration.UnitOfWork.Timeout = TimeSpan.FromMinutes(30);
            Configuration.UnitOfWork.IsTransactional = false;
            Configuration.Modules.AbpAutoMapper().Configurators.Add(config =>
            {
                config.CreateMap<User, PlayerDto>()
                    .ForMember(u => u.IsTeamLeader, options => options.Ignore());

                config.CreateMap<Mindfight, MindfightDto>()
                    .ForMember(m => m.CreatorEmail, options => options.Ignore())
                    .ForMember(m => m.RegisteredTeamsCount, options => options.Ignore())
                    .ForMember(m => m.UsersAllowedToEvaluate, options => options.Ignore())
                    .ForMember(m => m.TeamsAllowedToParticipate, options => options.Ignore());

                config.CreateMap<Mindfight, MindfightPublicDto>()
                    .ForMember(m => m.CreatorEmail, options => options.Ignore())
                    .ForMember(m => m.RegisteredTeamsCount, options => options.Ignore());

                config.CreateMap<Tour, TourDto>()
                    .ForMember(m => m.TotalPoints, options => options.Ignore())
                    .ForMember(m => m.IsLastTour, options => options.Ignore());

                config.CreateMap<TourDto, Tour>()
                    .ForMember(m => m.MindfightId, options => options.Ignore())
                    .ForMember(m => m.Mindfight, options => options.Ignore())
                    .ForMember(m => m.CreationTime, options => options.Ignore())
                    .ForMember(m => m.IsActive, options => options.Ignore())
                    .ForMember(m => m.IsDeleted, options => options.Ignore())
                    .ForMember(m => m.Questions, options => options.Ignore());

                config.CreateMap<Question, QuestionDto>()
                    .ForMember(m => m.IsLastQuestion, options => options.Ignore());

                config.CreateMap<QuestionDto, Question>()
                    .ForMember(m => m.Tour, options => options.Ignore())
                    .ForMember(m => m.CreationTime, options => options.Ignore())
                    .ForMember(m => m.IsActive, options => options.Ignore())
                    .ForMember(m => m.IsDeleted, options => options.Ignore())
                    .ForMember(m => m.TeamAnswers, options => options.Ignore());

                config.CreateMap<Registration, RegistrationDto>()
                    .ForMember(m => m.MindfightName, options => options.Ignore());

                config.CreateMap<TeamAnswer, TeamAnswerDto>()
                    .ForMember(m => m.Answer, options => options.Ignore())
                    .ForMember(m => m.TourOrderNumber, options => options.Ignore())
                    .ForMember(m => m.UserId, options => options.Ignore());

                config.CreateMap<MindfightResult, MindfightResultDto>()
                    .ForMember(m => m.MindfightName, options => options.Ignore())
                    .ForMember(m => m.ToursCount, options => options.Ignore())
                    .ForMember(m => m.TotalPoints, options => options.Ignore())
                    .ForMember(m => m.IsMindfightFinished, options => options.Ignore());
            });
            // Disable static mapper usage since it breaks unit tests (see https://github.com/aspnetboilerplate/aspnetboilerplate/issues/2052)
            Configuration.Modules.AbpAutoMapper().UseStaticMapper = false;

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;

            // Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            RegisterFakeService<AbpZeroDbMigrator<MindfightsDbContext>>();

            Configuration.ReplaceService<IEmailSender, NullEmailSender>(DependencyLifeStyle.Transient);
        }

        public override void Initialize()
        {
            ServiceCollectionRegistrar.Register(IocManager);
        }

        private void RegisterFakeService<TService>() where TService : class
        {
            IocManager.IocContainer.Register(
                Component.For<TService>()
                    .UsingFactoryMethod(() => Substitute.For<TService>())
                    .LifestyleSingleton()
            );
        }
    }
}
