using System;
using System.Collections.Generic;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Mindfights.DTOs;
using Mindfights.Services.TeamService;
using Mindfights.Users;
using Mindfights.Users.Dto;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Mindfights.Authorization.Users;
using Mindfights.EntityFrameworkCore;
using Mindfights.Services.MindfightService;
using Xunit;
using Mindfights.Services.RegistrationService;
using Mindfights.Services.TourService;

namespace Mindfights.Tests.Tours
{
    public class TourServiceTests : MindfightsTestBase
    {
        private readonly IMindfightService _mindfightService;
        private readonly IUserAppService _userAppService;
        private readonly IRepository<Models.Mindfight, long> _mindfightRepository;
        private readonly ITeamService _teamService;
        private readonly IRepository<Models.Team, long> _teamRepository;
        private readonly IRegistrationService _registrationService;
        private readonly ITourService _tourService;
        private readonly IRepository<Models.Tour, long> _tourRepository;

        public TourServiceTests()
        {
            _userAppService = Resolve<IUserAppService>();
            _mindfightService = Resolve<IMindfightService>();
            _mindfightRepository = Resolve<IRepository<Models.Mindfight, long>>();
            _teamService = Resolve<ITeamService>();
            _teamRepository = Resolve<IRepository<Models.Team, long>>();
            _registrationService = Resolve<IRegistrationService>();
            _tourService = Resolve<ITourService>();
            _tourRepository = Resolve<IRepository<Models.Tour, long>>();
        }

        [Fact]
        public async Task CreateTour_Test()
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

                    await CreateDemoTour(createdMindfight.Id);
                    await GetDemoTour();
                }
            });
        }

        [Fact]
        public async Task GetTour_Test()
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

                    await CreateDemoTour(createdMindfight.Id);
                    var createdTour = await GetDemoTour();

                    var tour = await _tourService.GetTour(createdTour.Id);
                    tour.ShouldNotBeNull();
                }
            });
        }

        [Fact]
        public async Task UpdateTour_Test()
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

                    await CreateDemoTour(createdMindfight.Id);
                    var createdTour = await GetDemoTour();

                    var updateTourDto = new TourDto
                    {
                        Title = createdTour.Title,
                        Description = createdTour.Description,
                        TimeToEnterAnswersInSeconds = 20,
                        IntroTimeInSeconds = 20
                    };

                    await _tourService.UpdateTour(updateTourDto, createdTour.Id);
                    var tour = _tourRepository
                        .FirstOrDefaultAsync(t => t.Id == createdTour.Id && t.IntroTimeInSeconds == 20);
                    tour.ShouldNotBeNull();
                }
            });
        }

        [Fact]
        public async Task DeleteTour_Test()
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

                    await CreateDemoTour(createdMindfight.Id);
                    var createdTour = await GetDemoTour();

                    await _tourService.DeleteTour(createdTour.Id);
                    var tour = await _tourRepository.FirstOrDefaultAsync(t => t.Id == createdTour.Id);
                    tour.ShouldBeNull();
                }
            });
        }

        [Fact]
        public async Task GetAllMindfightTours_Test()
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

                    await CreateDemoTour(createdMindfight.Id);

                    var mindfightTours = await _tourService.GetAllMindfightTours(createdMindfight.Id);
                    mindfightTours.Count.ShouldBeGreaterThanOrEqualTo(1);
                }
            });
        }

        [Fact]
        public async Task GetNextTour_Test()
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

                    await CreateDemoTour(createdMindfight.Id);
                    var createdTour = await GetDemoTour();

                    await CreateDemoTeam(johnNashUser.Id);
                    var johnNashTeam = await GetDemoTeam();

                    await _registrationService.CreateRegistration(createdMindfight.Id, johnNashTeam.Id);
                    await _registrationService.UpdateConfirmation(createdMindfight.Id, johnNashTeam.Id, true);

                    var nextTour = await _tourService.GetNextTour(createdMindfight.Id, johnNashTeam.Id);
                    nextTour.Id.ShouldBe(createdTour.Id);
                }
            });
        }

        [Fact]
        public async Task UpdateOrderNumber_Test()
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

                    await CreateDemoTour(createdMindfight.Id);
                    var createdTour = await GetDemoTour();

                    await _tourService.CreateTour(
                        new TourDto
                        {
                            Title = "TestTour2",
                            Description = "Best tour",
                            TimeToEnterAnswersInSeconds = 10,
                            IntroTimeInSeconds = 10
                        }, createdMindfight.Id);

                    var createdTour2 = await _tourRepository
                        .FirstOrDefaultAsync(m => m.Title == "TestTour2");
                    createdTour2.ShouldNotBeNull();

                    await _tourService.UpdateOrderNumber(createdTour.Id, 2);

                    var firstTour = await _tourRepository
                        .FirstOrDefaultAsync(m => m.MindfightId == createdMindfight.Id && m.OrderNumber == 1);

                    var secondTour = await _tourRepository
                        .FirstOrDefaultAsync(m => m.MindfightId == createdMindfight.Id && m.OrderNumber == 2);

                    firstTour.Id.ShouldBe(createdTour2.Id);
                    secondTour.Id.ShouldBe(createdTour.Id);
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

        private async Task CreateDemoTour(long mindfightId)
        {
            await _tourService.CreateTour(
                new TourDto
                {
                    Title = "TestTour",
                    Description = "Best tour",
                    TimeToEnterAnswersInSeconds = 10,
                    IntroTimeInSeconds = 10
                }, mindfightId);
        }

        private async Task<Models.Tour> GetDemoTour()
        {
            var createdTour = await _tourRepository
                .FirstOrDefaultAsync(m => m.Title == "TestTour");
            createdTour.ShouldNotBeNull();
            return createdTour;
        }
    }
}
