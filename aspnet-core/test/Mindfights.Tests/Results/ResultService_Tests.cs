using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Mindfights.Authorization.Users;
using Mindfights.DTOs;
using Mindfights.EntityFrameworkCore;
using Mindfights.Services.MindfightService;
using Mindfights.Services.RegistrationService;
using Mindfights.Services.TeamAnswerService;
using Mindfights.Services.TeamService;
using Mindfights.Users;
using Mindfights.Users.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mindfights.Services.QuestionService;
using Mindfights.Services.ResultService;
using Mindfights.Services.TourService;
using Xunit;

namespace Mindfights.Tests.Results
{
    public class ResultServiceTests : MindfightsTestBase
    {
        private readonly IMindfightService _mindfightService;
        private readonly IUserAppService _userAppService;
        private readonly IRepository<Models.Mindfight, long> _mindfightRepository;
        private readonly ITeamService _teamService;
        private readonly IRepository<Models.Team, long> _teamRepository;
        private readonly IRegistrationService _registrationService;
        private readonly ITeamAnswerService _teamAnswerService;
        private readonly ITourService _tourService;
        private readonly IRepository<Models.Tour, long> _tourRepository;
        private readonly IQuestionService _questionService;
        private readonly IRepository<Models.Question, long> _questionRepository;
        private readonly IResultService _resultService;
        private readonly IRepository<Models.MindfightResult, long> _resultRepository;

        public ResultServiceTests()
        {
            _userAppService = Resolve<IUserAppService>();
            _mindfightService = Resolve<IMindfightService>();
            _mindfightRepository = Resolve<IRepository<Models.Mindfight, long>>();
            _teamService = Resolve<ITeamService>();
            _teamRepository = Resolve<IRepository<Models.Team, long>>();
            _registrationService = Resolve<IRegistrationService>();
            _teamAnswerService = Resolve<ITeamAnswerService>();
            _tourService = Resolve<ITourService>();
            _tourRepository = Resolve<IRepository<Models.Tour, long>>();
            _questionService = Resolve<IQuestionService>();
            _questionRepository = Resolve<IRepository<Models.Question, long>>();
            _resultService = Resolve<IResultService>();
            _resultRepository = Resolve<IRepository<Models.MindfightResult, long>>();
        }

        [Fact]
        public async Task UpdateResult_Test()
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

                    await CreateDemoTour(createdMindfight.Id);
                    var createdTour = await GetDemoTour();

                    await CreateDemoQuestion(createdTour.Id);
                    var createdQuestion = await GetDemoQuestion();

                    await CreateDemoTeam(johnNashUser.Id);
                    var johnNashTeam = await GetDemoTeam();

                    await _registrationService.CreateRegistration(createdMindfight.Id, johnNashTeam.Id);
                    await _registrationService.UpdateConfirmation(createdMindfight.Id, johnNashTeam.Id, true);

                    var teamAnswerDto = new TeamAnswerDto
                    {
                        Answer = "demoAnswer",
                        QuestionId = createdQuestion.Id,
                        TeamId = johnNashTeam.Id
                    };

                    await _teamAnswerService.CreateTeamAnswer(
                        new List<TeamAnswerDto> { teamAnswerDto },
                        createdMindfight.Id);

                    await _teamAnswerService.UpdateIsEvaluated(createdQuestion.Id, johnNashTeam.Id, "Demo", 10, true);
                    await _resultService.UpdateResult(createdMindfight.Id, johnNashTeam.Id);
                    var currentResult = _resultRepository
                        .FirstOrDefaultAsync(x => x.TeamId == johnNashTeam.Id 
                                                  && x.MindfightId == createdMindfight.Id 
                                                  && x.IsEvaluated);
                    currentResult.ShouldNotBeNull();
                }
            });
        }

        [Fact]
        public async Task GetMindfightTeamResult_Test()
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

                    await CreateDemoTour(createdMindfight.Id);
                    var createdTour = await GetDemoTour();

                    await CreateDemoQuestion(createdTour.Id);
                    var createdQuestion = await GetDemoQuestion();

                    await CreateDemoTeam(johnNashUser.Id);
                    var johnNashTeam = await GetDemoTeam();

                    await _registrationService.CreateRegistration(createdMindfight.Id, johnNashTeam.Id);
                    await _registrationService.UpdateConfirmation(createdMindfight.Id, johnNashTeam.Id, true);

                    var teamAnswerDto = new TeamAnswerDto
                    {
                        Answer = "demoAnswer",
                        QuestionId = createdQuestion.Id,
                        TeamId = johnNashTeam.Id
                    };

                    await _teamAnswerService.CreateTeamAnswer(
                        new List<TeamAnswerDto> { teamAnswerDto },
                        createdMindfight.Id);

                    await _teamAnswerService.UpdateIsEvaluated(createdQuestion.Id, johnNashTeam.Id, "Demo", 10, true);
                    var currentResult = _resultService.GetMindfightTeamResult(createdMindfight.Id, johnNashTeam.Id);
                    currentResult.ShouldNotBeNull();
                }
            });
        }

        [Fact]
        public async Task GetMindfightResults_Test()
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

                    await CreateDemoTour(createdMindfight.Id);
                    var createdTour = await GetDemoTour();

                    await CreateDemoQuestion(createdTour.Id);
                    var createdQuestion = await GetDemoQuestion();

                    await CreateDemoTeam(johnNashUser.Id);
                    var johnNashTeam = await GetDemoTeam();

                    await _registrationService.CreateRegistration(createdMindfight.Id, johnNashTeam.Id);
                    await _registrationService.UpdateConfirmation(createdMindfight.Id, johnNashTeam.Id, true);

                    var teamAnswerDto = new TeamAnswerDto
                    {
                        Answer = "demoAnswer",
                        QuestionId = createdQuestion.Id,
                        TeamId = johnNashTeam.Id
                    };

                    await _teamAnswerService.CreateTeamAnswer(
                        new List<TeamAnswerDto> { teamAnswerDto },
                        createdMindfight.Id);

                    await _teamAnswerService.UpdateIsEvaluated(createdQuestion.Id, johnNashTeam.Id, "Demo", 10, true);
                    var currentResult = await _resultService.GetMindfightResults(createdMindfight.Id);
                    currentResult.Count.ShouldBeGreaterThanOrEqualTo(1);
                }
            });
        }

        [Fact]
        public async Task GetTeamResults_Test()
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

                    await CreateDemoTour(createdMindfight.Id);
                    var createdTour = await GetDemoTour();

                    await CreateDemoQuestion(createdTour.Id);
                    var createdQuestion = await GetDemoQuestion();

                    await CreateDemoTeam(johnNashUser.Id);
                    var johnNashTeam = await GetDemoTeam();

                    await _registrationService.CreateRegistration(createdMindfight.Id, johnNashTeam.Id);
                    await _registrationService.UpdateConfirmation(createdMindfight.Id, johnNashTeam.Id, true);

                    var teamAnswerDto = new TeamAnswerDto
                    {
                        Answer = "demoAnswer",
                        QuestionId = createdQuestion.Id,
                        TeamId = johnNashTeam.Id
                    };

                    await _teamAnswerService.CreateTeamAnswer(
                        new List<TeamAnswerDto> { teamAnswerDto },
                        createdMindfight.Id);

                    await _teamAnswerService.UpdateIsEvaluated(createdQuestion.Id, johnNashTeam.Id, "Demo", 10, true);
                    var currentResult = await _resultService.GetTeamResults(johnNashTeam.Id);
                    currentResult.Count.ShouldBeGreaterThanOrEqualTo(1);
                }
            });
        }

        [Fact]
        public async Task GetUserResults_Test()
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

                    await CreateDemoTour(createdMindfight.Id);
                    var createdTour = await GetDemoTour();

                    await CreateDemoQuestion(createdTour.Id);
                    var createdQuestion = await GetDemoQuestion();

                    await CreateDemoTeam(johnNashUser.Id);
                    var johnNashTeam = await GetDemoTeam();

                    await _registrationService.CreateRegistration(createdMindfight.Id, johnNashTeam.Id);
                    await _registrationService.UpdateConfirmation(createdMindfight.Id, johnNashTeam.Id, true);

                    var teamAnswerDto = new TeamAnswerDto
                    {
                        Answer = "demoAnswer",
                        QuestionId = createdQuestion.Id,
                        TeamId = johnNashTeam.Id
                    };

                    await _teamAnswerService.CreateTeamAnswer(
                        new List<TeamAnswerDto> { teamAnswerDto },
                        createdMindfight.Id);

                    await _teamAnswerService.UpdateIsEvaluated(createdQuestion.Id, johnNashTeam.Id, "Demo", 10, true);
                    var currentResult = await _resultService.GetUserResults(johnNashUser.Id);
                    currentResult.Count.ShouldBeGreaterThanOrEqualTo(1);
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

        private async Task CreateDemoQuestion(long tourId)
        {
            await _questionService.CreateQuestion(
                new QuestionDto
                {
                    Title = "TestQuestion",
                    Description = "Best tour",
                    Answer = "Answer",
                    Points = 10,
                    TimeToAnswerInSeconds = 10
                }, tourId);
        }

        private async Task<Models.Question> GetDemoQuestion()
        {
            var createdQuestion = await _questionRepository
                .FirstOrDefaultAsync(m => m.Title == "TestQuestion");
            createdQuestion.ShouldNotBeNull();
            return createdQuestion;
        }
    }
}
