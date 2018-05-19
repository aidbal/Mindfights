using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Mindfights.DTOs;
using Mindfights.Services.MindfightService;
using Mindfights.Services.RegistrationService;
using Mindfights.Services.TeamService;
using Mindfights.Users;
using Mindfights.Users.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mindfights.Authorization.Users;
using Mindfights.EntityFrameworkCore;
using Xunit;

namespace Mindfights.Tests.Mindfights
{
    public class MindfightServiceTests : MindfightsTestBase
    {
        private readonly IMindfightService _mindfightService;
        private readonly IUserAppService _userAppService;
        private readonly IRepository<Models.Mindfight, long> _mindfightRepository;
        private readonly ITeamService _teamService;
        private readonly IRepository<Models.Team, long> _teamRepository;
        private readonly IRegistrationService _registrationService;

        public MindfightServiceTests()
        {
            _userAppService = Resolve<IUserAppService>();
            _mindfightService = Resolve<IMindfightService>();
            _mindfightRepository = Resolve<IRepository<Models.Mindfight, long>>();
            _teamService = Resolve<ITeamService>();
            _teamRepository = Resolve<IRepository<Models.Team, long>>();
            _registrationService = Resolve<IRegistrationService>();
        }

        [Fact]
        public async Task CreateMindfight_Test()
        {
            await CreateDemoUser();

            // Act
            await UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash");
                johnNashUser.ShouldNotBeNull();

                using (AbpSession.Use(null, johnNashUser.Id))
                {
                    await CreateDemoMindfight();
                    var createdMindfight = await GetDemoMindfight();
                    createdMindfight.ShouldNotBeNull();
                }
            });
        }

        [Fact]
        public async Task GetMindfight_Test()
        {
            await CreateDemoUser();

            // Act
            await UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash");
                johnNashUser.ShouldNotBeNull();

                using (AbpSession.Use(null, johnNashUser.Id))
                {
                    await CreateDemoMindfight();
                    var createdMindfight = await GetDemoMindfight();

                    var mindfight = await _mindfightService
                        .GetMindfight(createdMindfight.Id);
                    mindfight.ShouldNotBeNull();
                }
            });
        }

        [Fact]
        public async Task UpdateMindfight_Test()
        {
            await CreateDemoUser();

            // Act
            await UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash");
                johnNashUser.ShouldNotBeNull();

                using (AbpSession.Use(null, johnNashUser.Id))
                {
                    await CreateDemoMindfight();
                    var createdMindfight = await GetDemoMindfight();

                    var updatedMindfightDto = new MindfightDto()
                    {
                        Title = "Hello",
                        Description = createdMindfight.Description,
                        StartTime = createdMindfight.StartTime,
                        TeamsLimit = createdMindfight.TeamsLimit,
                        Id = createdMindfight.Id
                    };

                    await _mindfightService.UpdateMindfight(updatedMindfightDto);

                    var mindfight = await _mindfightService
                        .GetMindfight(createdMindfight.Id);
                    mindfight.ShouldNotBeNull();
                    mindfight.Title.ShouldBe("Hello");
                }
            });
        }

        [Fact]
        public async Task DeleteMindfight_Test()
        {
            await CreateDemoUser();

            // Act
            await UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash");
                johnNashUser.ShouldNotBeNull();

                using (AbpSession.Use(null, johnNashUser.Id))
                {
                    await CreateDemoMindfight();
                    var createdMindfight = await GetDemoMindfight();

                    await _mindfightService.DeleteMindfight(createdMindfight.Id);

                    var mindfight = await _mindfightRepository
                        .FirstOrDefaultAsync(m => m.Id == createdMindfight.Id);
                    mindfight.ShouldBeNull();
                }
            });
        }

        [Fact]
        public async Task GetMyCreatedMindfights_Test()
        {
            await CreateDemoUser();

            // Act
            await UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash");
                johnNashUser.ShouldNotBeNull();

                using (AbpSession.Use(null, johnNashUser.Id))
                {
                    await CreateDemoMindfight();
                    var createdMindfight = await GetDemoMindfight();

                    var mindfights = await _mindfightService.GetMyCreatedMindfights();
                    mindfights.Count.ShouldBeGreaterThanOrEqualTo(1);
                }
            });
        }

        [Fact]
        public async Task GetAllowedToEvaluateMindfights_Test()
        {
            await CreateDemoUser();
            await CreateDemoUser2();

            // Act
            await UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash");
                johnNashUser.ShouldNotBeNull();
                var johnNash2User = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash2");
                johnNashUser.ShouldNotBeNull();

                using (AbpSession.Use(null, johnNashUser.Id))
                {
                    await CreateDemoMindfight();
                    var createdMindfight = await GetDemoMindfight();

                    var updatedMindfightDto = new MindfightDto()
                    {
                        Title = createdMindfight.Title,
                        Description = createdMindfight.Description,
                        StartTime = createdMindfight.StartTime,
                        TeamsLimit = createdMindfight.TeamsLimit,
                        Id = createdMindfight.Id,
                        UsersAllowedToEvaluate = new List<string> { johnNash2User.EmailAddress }
                    };

                    await _mindfightService.UpdateMindfight(updatedMindfightDto);
                }

                using (AbpSession.Use(null, johnNash2User.Id))
                {
                    var evaluatingMindfights = await _mindfightService
                         .GetAllowedToEvaluateMindfights();
                    evaluatingMindfights.Count.ShouldBeGreaterThanOrEqualTo(1);
                }
            });
        }

        [Fact]
        public async Task GetRegisteredMindfights_Test()
        {
            await CreateDemoUser();

            // Act
            await UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash");
                johnNashUser.ShouldNotBeNull();

                using (AbpSession.Use(null, johnNashUser.Id))
                {
                    await CreateDemoMindfight();
                    var createdMindfight = await GetDemoMindfight();
                    await UpdateMindfightActiveStatus(createdMindfight);

                    await CreateDemoTeam(johnNashUser.Id);
                    var johnNashTeam = await GetDemoTeam();

                    await _registrationService.CreateRegistration(createdMindfight.Id, johnNashTeam.Id);

                    var registeredMindfights = await _mindfightService.GetRegisteredMindfights();
                    registeredMindfights.Count.ShouldBeGreaterThanOrEqualTo(1);
                }
            });
        }

        [Fact]
        public async Task GetUpcomingMindfights_Test()
        {
            await CreateDemoUser();

            // Act
            await UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash");
                johnNashUser.ShouldNotBeNull();

                using (AbpSession.Use(null, johnNashUser.Id))
                {
                    await CreateDemoMindfight();
                    var createdMindfight = await GetDemoMindfight();

                    await UpdateMindfightActiveStatus(createdMindfight);
                    await _mindfightService.UpdateConfirmedStatus(createdMindfight.Id, true);

                    var upcomingMindfights = await _mindfightService.GetUpcomingMindfights();
                    upcomingMindfights.Count.ShouldBeGreaterThanOrEqualTo(1);
                }
            });
        }

        [Fact]
        public async Task UpdateActiveStatus_Test()
        {
            await CreateDemoUser();

            // Act
            await UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash");
                johnNashUser.ShouldNotBeNull();

                using (AbpSession.Use(null, johnNashUser.Id))
                {
                    await CreateDemoMindfight();
                    var createdMindfight = await GetDemoMindfight();

                    await _mindfightService.UpdateActiveStatus(createdMindfight.Id, true);

                    var updatedMindfight = await _mindfightRepository
                        .FirstOrDefaultAsync(m => m.Id == createdMindfight.Id && m.IsActive);
                    updatedMindfight.ShouldNotBeNull();
                }
            });
        }

        [Fact]
        public async Task UpdateConfirmedStatus_Test()
        {
            await CreateDemoUser();

            // Act
            await UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash");
                johnNashUser.ShouldNotBeNull();

                using (AbpSession.Use(null, johnNashUser.Id))
                {
                    await CreateDemoMindfight();
                    var createdMindfight = await GetDemoMindfight();

                    await _mindfightService.UpdateConfirmedStatus(createdMindfight.Id, true);

                    var updatedMindfight = _mindfightRepository
                        .FirstOrDefaultAsync(m => m.Id == createdMindfight.Id && m.IsConfirmed);
                    updatedMindfight.ShouldNotBeNull();
                }
            });
        }

        [Fact]
        public async Task UpdateFinishedStatus_Test()
        {
            await CreateDemoUser();

            // Act
            await UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash");
                johnNashUser.ShouldNotBeNull();

                using (AbpSession.Use(null, johnNashUser.Id))
                {
                    await CreateDemoMindfight();
                    var createdMindfight = await GetDemoMindfight();

                    await _mindfightService.UpdateFinishedStatus(createdMindfight.Id, true);

                    var updatedMindfight = _mindfightRepository
                        .FirstOrDefaultAsync(m => m.Id == createdMindfight.Id && m.IsConfirmed);
                    updatedMindfight.ShouldNotBeNull();
                }
            });
        }

        private async Task CreateDemoUser()
        {
            await _userAppService.Create(new CreateUserDto
            {
                EmailAddress = "john@volosoft.com",
                IsActive = true,
                Name = "John",
                Surname = "Nash",
                Password = "123qwe",
                UserName = "john.nash"
            });
        }

        private async Task CreateDemoUser2()
        {
            await _userAppService.Create(new CreateUserDto
            {
                EmailAddress = "john2@volosoft.com",
                IsActive = true,
                Name = "John2",
                Surname = "Nash",
                Password = "123qwe",
                UserName = "john.nash2"
            });
        }

        private async Task CreateDemoTeam(long leaderId)
        {
            await _teamService.CreateTeam(
                new TeamDto
                {
                    Name = "Winners",
                    Description = "Best winners",
                    LeaderId = leaderId
                });
        }

        private async Task<Models.Team> GetDemoTeam()
        {
            var team = await _teamRepository.FirstOrDefaultAsync(u => u.Name == "Winners");
            team.ShouldNotBeNull();

            return team;
        }

        private async Task CreateDemoMindfight()
        {
            await _mindfightService.CreateMindfight(
                new MindfightCreateDto()
                {
                    Title = "TestMindfight",
                    Description = "Best mindfight",
                    StartTime = DateTime.Now,
                    TeamsLimit = 0
                });
        }

        private async Task<Models.Mindfight> GetDemoMindfight()
        {
            var createdMindfight = await _mindfightRepository
                .FirstOrDefaultAsync(m => m.Title == "TestMindfight");
            createdMindfight.ShouldNotBeNull();
            return createdMindfight;
        }

        private async Task UpdateMindfightActiveStatus(Models.Mindfight createdMindfight)
        {
            await _mindfightService.UpdateActiveStatus(createdMindfight.Id, true);
        }
    }
}
