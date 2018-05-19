using System;
using Mindfights.Services.PlayerService;
using Mindfights.Users;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mindfights.Authorization.Users;
using Mindfights.DTOs;
using Mindfights.EntityFrameworkCore;
using Mindfights.Users.Dto;
using Shouldly;
using Xunit;

namespace Mindfights.Tests.Players
{
    public class PlayerServiceTests : MindfightsTestBase
    {
        private readonly IPlayerService _playerService;
        private readonly IUserAppService _userAppService;

        public PlayerServiceTests()
        {
            _userAppService = Resolve<IUserAppService>();
            _playerService = Resolve<IPlayerService>();
        }

        [Fact]
        public async Task GetPlayerInfo_Test()
        {
            await CreateDemoUser();

            // Act
            await UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash");
                johnNashUser.ShouldNotBeNull();

                var johnPlayer = await _playerService.GetPlayerInfo(johnNashUser.Id);
                johnPlayer.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task UpdateInfo_Test()
        {
            await CreateDemoUser();

            // Act
            await UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash");
                johnNashUser.ShouldNotBeNull();

                var player = new PlayerDto
                {
                    EmailAddress = johnNashUser.EmailAddress,
                    Name = "John2",
                    Surname = johnNashUser.Surname,
                    UserName = johnNashUser.UserName,
                    Birthdate = DateTime.Now
                };

                await _playerService.UpdatePlayerInfo(player, johnNashUser.Id);

                var johnPlayer = await _playerService.GetPlayerInfo(johnNashUser.Id);
                johnPlayer.ShouldNotBeNull();
                johnPlayer.Name.ShouldBe("John2");
            });
        }

        [Fact]
        public async Task GetAllPlayers_Test()
        {
            await CreateDemoUser();
            await CreateDemoUser2();

            // Act
            var allPlayers = await _playerService.GetAllPlayers();
            allPlayers.Count.ShouldBeGreaterThanOrEqualTo(2);
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
    }
}
