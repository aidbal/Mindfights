using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Castle.MicroKernel.Registration;
using Castle.Windsor.MsDependencyInjection;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Mindfights.Authorization.Users;
using Mindfights.EntityFrameworkCore;
using Mindfights.Identity;
using Mindfights.Models;
using Mindfights.Services.MindfightService;
using Mindfights.Services.PlayerService;
using Mindfights.Services.QuestionService;
using Mindfights.Services.RegistrationService;
using Mindfights.Services.ResultService;
using Mindfights.Services.TeamAnswerService;
using Mindfights.Services.TeamService;
using Mindfights.Services.TourService;
using Mindfight = Mindfights.Services.MindfightService.Mindfight;
using Question = Mindfights.Services.QuestionService.Question;
using Registration = Mindfights.Services.RegistrationService.Registration;
using Team = Mindfights.Services.TeamService.Team;
using TeamAnswer = Mindfights.Services.TeamAnswerService.TeamAnswer;
using Tour = Mindfights.Services.TourService.Tour;

namespace Mindfights.Tests.DependencyInjection
{
    public static class ServiceCollectionRegistrar
    {
        public static void Register(IIocManager iocManager)
        {
            var services = new ServiceCollection();

            IdentityRegistrar.Register(services);

            services.AddEntityFrameworkInMemoryDatabase();

            var serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(iocManager.IocContainer, services);

            var builder = new DbContextOptionsBuilder<MindfightsDbContext>();
            builder.UseInMemoryDatabase(Guid.NewGuid().ToString()).UseInternalServiceProvider(serviceProvider);

            iocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<MindfightsDbContext>>()
                    .Instance(builder.Options)
                    .LifestyleSingleton()
            );

            iocManager.Register(typeof(IPlayerService), typeof(Player), DependencyLifeStyle.Transient);
            iocManager.Register(typeof(ITeamService), typeof(Team), DependencyLifeStyle.Transient);
            iocManager.Register(typeof(IMindfightService), typeof(Mindfight), DependencyLifeStyle.Transient);
            iocManager.Register(typeof(IQuestionService), typeof(Question), DependencyLifeStyle.Transient);
            iocManager.Register(typeof(ITeamAnswerService), typeof(TeamAnswer), DependencyLifeStyle.Transient);
            iocManager.Register(typeof(IRegistrationService), typeof(Registration), DependencyLifeStyle.Transient);
            iocManager.Register(typeof(IResultService), typeof(Result), DependencyLifeStyle.Transient);
            iocManager.Register(typeof(ITourService), typeof(Tour), DependencyLifeStyle.Transient);

            iocManager.Register(typeof(IRepository<Models.City, long>), typeof(Models.City), DependencyLifeStyle.Transient);
            iocManager.Register(typeof(IRepository<Models.Team, long>), typeof(Models.Team), DependencyLifeStyle.Transient);
            iocManager.Register(typeof(IRepository<Models.Mindfight, long>), typeof(Models.Mindfight), DependencyLifeStyle.Transient);
            iocManager.Register(typeof(IRepository<Models.Question, long>), typeof(Models.Question), DependencyLifeStyle.Transient);
            iocManager.Register(typeof(IRepository<Models.TeamAnswer, long>), typeof(Models.TeamAnswer), DependencyLifeStyle.Transient);
            iocManager.Register(typeof(IRepository<Models.Registration, long>), typeof(Models.Registration), DependencyLifeStyle.Transient);
            iocManager.Register(typeof(IRepository<Models.MindfightResult, long>), typeof(Models.MindfightResult), DependencyLifeStyle.Transient);
            iocManager.Register(typeof(IRepository<Models.Tour, long>), typeof(Models.Tour), DependencyLifeStyle.Transient);
        }
    }
}
