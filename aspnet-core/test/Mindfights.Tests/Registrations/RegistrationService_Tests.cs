using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Mindfights.Authorization.Users;
using Mindfights.DTOs;
using Mindfights.EntityFrameworkCore;
using Mindfights.Services.MindfightService;
using Mindfights.Services.RegistrationService;
using Mindfights.Services.TeamService;
using Mindfights.Users;
using Mindfights.Users.Dto;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Mindfights.Tests.Registrations
{
    public class RegistrationServiceTests : MindfightsTestBase
    {
        private readonly IMindfightService _mindfightService;
        private readonly IUserAppService _userAppService;
        private readonly IRepository<Models.Mindfight, long> _mindfightRepository;
        private readonly ITeamService _teamService;
        private readonly IRepository<Models.Team, long> _teamRepository;
        private readonly IRegistrationService _registrationService;
        private readonly IRepository<Models.Registration, long> _registrationRepository;

        public RegistrationServiceTests()
        {
            _userAppService = Resolve<IUserAppService>();
            _mindfightService = Resolve<IMindfightService>();
            _mindfightRepository = Resolve<IRepository<Models.Mindfight, long>>();
            _teamService = Resolve<ITeamService>();
            _teamRepository = Resolve<IRepository<Models.Team, long>>();
            _registrationService = Resolve<IRegistrationService>();
            _registrationRepository = Resolve<IRepository<Models.Registration, long>>();
        }

        [Fact]
        public async Task CreateRegistration_Test()
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
                    var registration = await _registrationRepository
                        .FirstOrDefaultAsync(x => x.TeamId == johnNashTeam.Id && x.MindfightId == createdMindfight.Id);

                    registration.ShouldNotBeNull();
                }
            });
        }

        [Fact]
        public async Task DeleteRegistration_Test()
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

                    var registration = await _registrationRepository
                        .FirstOrDefaultAsync(x => x.TeamId == johnNashTeam.Id && x.MindfightId == createdMindfight.Id);
                    registration.ShouldNotBeNull();

                    await _registrationService.DeleteRegistration(createdMindfight.Id, johnNashTeam.Id);

                    var deletedRegistration = await _registrationRepository
                        .FirstOrDefaultAsync(x => x.TeamId == johnNashTeam.Id && x.MindfightId == createdMindfight.Id);
                    deletedRegistration.ShouldBeNull();
                }
            });
        }

        [Fact]
        public async Task GetTeamRegistrations_Test()
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

                    var registration = await _registrationRepository
                        .FirstOrDefaultAsync(x => x.TeamId == johnNashTeam.Id && x.MindfightId == createdMindfight.Id);
                    registration.ShouldNotBeNull();

                    var registrations = await _registrationService.GetTeamRegistrations(johnNashTeam.Id);
                    registrations.Count.ShouldBe(1);
                }
            });
        }

        [Fact]
        public async Task GetRegistration_Test()
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

                    var registration = await _registrationRepository
                        .FirstOrDefaultAsync(x => x.TeamId == johnNashTeam.Id && x.MindfightId == createdMindfight.Id);
                    registration.ShouldNotBeNull();

                    var createdRegistration = await _registrationService.GetRegistration(registration.Id);
                    createdRegistration.ShouldNotBeNull();
                }
            });
        }

        [Fact]
        public async Task GetMindfightTeamRegistration_Test()
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

                    var registration = await _registrationRepository
                        .FirstOrDefaultAsync(x => x.TeamId == johnNashTeam.Id && x.MindfightId == createdMindfight.Id);
                    registration.ShouldNotBeNull();

                    var createdRegistration = await _registrationService
                        .GetMindfightTeamRegistration(createdMindfight.Id, registration.Id);

                    createdRegistration.ShouldNotBeNull();
                }
            });
        }

        [Fact]
        public async Task GetMindfightRegistrations_Test()
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

                    var registration = await _registrationRepository
                        .FirstOrDefaultAsync(x => x.TeamId == johnNashTeam.Id && x.MindfightId == createdMindfight.Id);
                    registration.ShouldNotBeNull();

                    var createdRegistration = await _registrationService
                        .GetMindfightRegistrations(createdMindfight.Id);

                    createdRegistration.ShouldNotBeNull();
                }
            });
        }

        [Fact]
        public async Task UpdateConfirmation_Test()
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

                    var registration = await _registrationRepository
                        .FirstOrDefaultAsync(x => x.TeamId == johnNashTeam.Id && x.MindfightId == createdMindfight.Id);
                    registration.ShouldNotBeNull();

                    await _registrationService.UpdateConfirmation(createdMindfight.Id, johnNashTeam.Id, true);

                    var confirmedRegistration = await _registrationRepository
                        .FirstOrDefaultAsync(x => x.TeamId == johnNashTeam.Id 
                                                  && x.MindfightId == createdMindfight.Id
                                                  && x.IsConfirmed);

                    confirmedRegistration.ShouldNotBeNull();
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
