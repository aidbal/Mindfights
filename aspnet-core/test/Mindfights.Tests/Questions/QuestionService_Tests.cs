using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Mindfights.DTOs;
using Mindfights.Services.MindfightService;
using Mindfights.Services.QuestionService;
using Mindfights.Services.RegistrationService;
using Mindfights.Services.TeamService;
using Mindfights.Services.TourService;
using Mindfights.Users;
using Mindfights.Users.Dto;
using Shouldly;
using System;
using System.Threading.Tasks;
using Mindfights.Authorization.Users;
using Mindfights.EntityFrameworkCore;
using Xunit;

namespace Mindfights.Tests.Questions
{
    public class QuestionServiceTests : MindfightsTestBase
    {
        private readonly IMindfightService _mindfightService;
        private readonly IUserAppService _userAppService;
        private readonly IRepository<Models.Mindfight, long> _mindfightRepository;
        private readonly ITeamService _teamService;
        private readonly IRepository<Models.Team, long> _teamRepository;
        private readonly IRegistrationService _registrationService;
        private readonly ITourService _tourService;
        private readonly IRepository<Models.Tour, long> _tourRepository;
        private readonly IQuestionService _questionService;
        private readonly IRepository<Models.Question, long> _questionRepository;

        public QuestionServiceTests()
        {
            _userAppService = Resolve<IUserAppService>();
            _mindfightService = Resolve<IMindfightService>();
            _mindfightRepository = Resolve<IRepository<Models.Mindfight, long>>();
            _teamService = Resolve<ITeamService>();
            _teamRepository = Resolve<IRepository<Models.Team, long>>();
            _registrationService = Resolve<IRegistrationService>();
            _tourService = Resolve<ITourService>();
            _tourRepository = Resolve<IRepository<Models.Tour, long>>();
            _questionService = Resolve<IQuestionService>();
            _questionRepository = Resolve<IRepository<Models.Question, long>>();
        }

        [Fact]
        public async Task CreateQuestion_Test()
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

                    await CreateDemoQuestion(createdTour.Id);
                    await GetDemoQuestion();
                }
            });
        }

        [Fact]
        public async Task GetQuestion_Test()
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

                    await CreateDemoQuestion(createdTour.Id);
                    var createdQuestion = await GetDemoQuestion();

                    var questionToGet = await _questionService.GetQuestion(createdQuestion.Id);
                    questionToGet.ShouldNotBeNull();
                }
            });
        }

        [Fact]
        public async Task GetAllTourQuestions_Test()
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

                    await CreateDemoQuestion(createdTour.Id);

                    var tourQuestions = await _questionService.GetAllTourQuestions(createdTour.Id);
                    tourQuestions.Count.ShouldBeGreaterThanOrEqualTo(1);
                }
            });
        }

        [Fact]
        public async Task DeleteQuestion_Test()
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

                    await CreateDemoQuestion(createdTour.Id);
                    var createdQuestion = await GetDemoQuestion();

                    await _questionService.DeleteQuestion(createdQuestion.Id);
                    var deletedQuestion = await _questionRepository
                        .FirstOrDefaultAsync(m => m.Id == createdQuestion.Id);

                    deletedQuestion.ShouldBeNull();
                }
            });
        }

        [Fact]
        public async Task UpdateQuestion_Test()
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

                    await CreateDemoQuestion(createdTour.Id);
                    var createdQuestion = await GetDemoQuestion();

                    await _questionService.UpdateQuestion(
                        new QuestionDto
                        {
                            Title = "Question",
                            Description = "Description",
                            Answer = "Answer",
                            Points = 10,
                            TimeToAnswerInSeconds = 10
                        }, createdQuestion.Id);

                    var updatedQuestion = await _questionRepository
                        .FirstOrDefaultAsync(m => m.Id == createdQuestion.Id && m.Title == "Question");
                    updatedQuestion.ShouldNotBeNull();
                }
            });
        }

        [Fact]
        public async Task GetNextQuestion_Test()
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

                    await CreateDemoQuestion(createdTour.Id);
                    var createdQuestion = await GetDemoQuestion();

                    await CreateDemoTeam(johnNashUser.Id);
                    var johnNashTeam = await GetDemoTeam();

                    await _registrationService.CreateRegistration(createdMindfight.Id, johnNashTeam.Id);
                    await _registrationService.UpdateConfirmation(createdMindfight.Id, johnNashTeam.Id, true);

                    await _tourService.GetNextTour(createdMindfight.Id, johnNashTeam.Id);
                    var nextQuestion = await _questionService.GetNextQuestion(createdMindfight.Id, johnNashTeam.Id);
                    nextQuestion.Id.ShouldBe(createdQuestion.Id);
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

                    await CreateDemoQuestion(createdTour.Id);
                    var createdQuestion = await GetDemoQuestion();

                    await _questionService.CreateQuestion(
                        new QuestionDto
                        {
                            Title = "TestQuestion2",
                            Description = "Best tour",
                            Answer = "Answer",
                            Points = 10,
                            TimeToAnswerInSeconds = 10
                        }, createdTour.Id);

                    var question2 = await _questionRepository
                        .FirstOrDefaultAsync(m => m.Title == "TestQuestion2");
                    question2.ShouldNotBeNull();

                    await _questionService.UpdateOrderNumber(createdQuestion.Id, 2);

                    var firstQuestion = await _questionRepository
                        .FirstOrDefaultAsync(m => m.TourId == createdTour.Id && m.OrderNumber == 1);

                    var secondQuestion = await _questionRepository
                        .FirstOrDefaultAsync(m => m.TourId == createdTour.Id && m.OrderNumber == 2);

                    firstQuestion.Id.ShouldBe(question2.Id);
                    secondQuestion.Id.ShouldBe(createdQuestion.Id);
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
