using Abp.Domain.Uow;
using Microsoft.EntityFrameworkCore;
using Mindfights.DTOs;
using Mindfights.Services.TeamService;
using Mindfights.Users;
using Mindfights.Users.Dto;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Mindfights.Authorization.Users;
using Xunit;
using Abp.Domain.Repositories;
using Mindfights.EntityFrameworkCore;

namespace Mindfights.Tests.Teams
{
    public class TeamServiceTests : MindfightsTestBase
    {
        private readonly ITeamService _teamService;
        private readonly IUserAppService _userAppService;
        private readonly IRepository<Models.Team, long> _teamRepository;

        public TeamServiceTests()
        {
            _userAppService = Resolve<IUserAppService>();
            _teamService = Resolve<ITeamService>();
            _teamRepository = Resolve<IRepository<Models.Team, long>>();
        }

        [Fact]
        public async Task CreateTeam_Test()
        {
            await CreateDemoUser();

            // Act
            await UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash");
                johnNashUser.ShouldNotBeNull();

                using (AbpSession.Use(null, johnNashUser.Id))
                {
                    await CreateDemoTeam(johnNashUser.Id);
                    await GetDemoTeam();

                    var createdTeam = await context.Teams.FirstOrDefaultAsync(u => u.Name == "Winners");
                    createdTeam.ShouldNotBeNull();
                }
            });
        }

        [Fact]
        public async Task InsertUser_Test()
        {
            await CreateDemoUser();
            await CreateDemoUser2();

            // Act
            await UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash");
                johnNashUser.ShouldNotBeNull();
                var johnNashUser2 = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash2");
                johnNashUser.ShouldNotBeNull();

                using (AbpSession.Use(null, johnNashUser.Id))
                {
                    await CreateDemoTeam(johnNashUser.Id);
                    var johnNashTeam = await GetDemoTeam();

                    await _teamService.InsertUser(johnNashTeam.Id, johnNashUser2.UserName);
                    var lukeNashTeam = await context.Teams
                        .Include(user => user.Players)
                        .FirstOrDefaultAsync(u => u.Name == "Winners" && u.Players.Any(p => p.Id == johnNashUser2.Id));
                    lukeNashTeam.ShouldNotBeNull();
                }
            });
        }

        [Fact]
        public async Task RemoveUser_Test()
        {
            await CreateDemoUser();
            await CreateDemoUser2();

            // Act
            await UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash");
                johnNashUser.ShouldNotBeNull();
                var johnNashUser2 = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash2");
                johnNashUser.ShouldNotBeNull();

                using (AbpSession.Use(null, johnNashUser.Id))
                {
                    await CreateDemoTeam(johnNashUser.Id);
                    var johnNashTeam = await GetDemoTeam();

                    await _teamService.InsertUser(johnNashTeam.Id, johnNashUser2.UserName);
                    await _teamService.RemoveUser(johnNashTeam.Id, johnNashUser2.Id);
                    var lukeNashTeam = await context.Teams
                        .Include(user => user.Players)
                        .FirstOrDefaultAsync(team => team.Name == "Winners" && team.Players.Any(p => p.Id == johnNashUser2.Id));
                    lukeNashTeam.ShouldBeNull();
                }
            });
        }

        [Fact]
        public async Task ChangeTeamLeader_Test()
        {
            await CreateDemoUser();
            await CreateDemoUser2();

            // Act
            await UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash");
                johnNashUser.ShouldNotBeNull();
                var johnNashUser2 = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash2");
                johnNashUser.ShouldNotBeNull();

                using (AbpSession.Use(null, johnNashUser.Id))
                {
                    await CreateDemoTeam(johnNashUser.Id);
                    var johnNashTeam = await GetDemoTeam();

                    await _teamService.InsertUser(johnNashTeam.Id, johnNashUser2.UserName);
                    await _teamService.ChangeTeamLeader(johnNashTeam.Id, johnNashUser2.Id);
                    
                    var lukeNashLeaderTeam = await _teamRepository
                        .FirstOrDefaultAsync(team => team.Name == "Winners" && team.LeaderId == johnNashUser2.Id);
                    lukeNashLeaderTeam.ShouldNotBeNull();
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
    }
}
