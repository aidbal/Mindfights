using System.Collections.Generic;
using System.Linq;
using Abp.AutoMapper;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Mindfights.Authorization.Users;
using Mindfights.DTOs;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Castle.Core.Internal;
using Mindfights.Models;

namespace Mindfights.Services.PlayerService
{
    public class Player : IPlayerService
    {
        private readonly UserManager _userManager;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IRepository<City, long> _cityRepository;

        public Player(
            UserManager userManager,
            IPermissionChecker permissionChecker,
            IRepository<City, long> cityRepository
            )
        {
            _userManager = userManager;
            _permissionChecker = permissionChecker;
            _cityRepository = cityRepository;
        }

        public async Task<PlayerDto> GetPlayerInfo(long userId)
        {
            var playerDto = new PlayerDto();
            var player = await _userManager.Users
                .IgnoreQueryFilters()
                .Include(x => x.Team)
                .Include(x => x.City)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (player == null)
            {
                throw new UserFriendlyException("Vartotojas su nurodytu id neegzistuoja!");
            }
            player.MapTo(playerDto);

            if (player.City != null)
            {
                playerDto.City = player.City.Name;
                playerDto.CityId = player.City.Id;
            }
            else
            {
                playerDto.City = string.Empty;
                playerDto.CityId = 0;
            }

            if (player.Team != null)
            {
                playerDto.TeamId = player.Team.Id;
                playerDto.IsTeamLeader = player.Team.LeaderId == player.Id;
                playerDto.IsActiveInTeam = player.IsActiveInTeam;
            }
            return playerDto;
        }

        public async Task UpdatePlayerInfo(PlayerDto playerInfo, long userId)
        {
            if (!(_permissionChecker.IsGranted("Pages.Users") || _userManager.AbpSession.UserId == userId))
            {
                throw new UserFriendlyException("Neturite teisių redaguoti vartotojo duomenų!");
            }

            var player = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (player == null)
            {
                throw new UserFriendlyException("Vartotojas su nurodytu id neegzistuoja!");
            }

            var playerWithSameUsername = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(user => user.NormalizedUserName == playerInfo.UserName.ToUpper() && userId != user.Id);

            if (playerWithSameUsername != null)
            {
                throw new UserFriendlyException("Vartotojas su tokiu slapyvardžiu jau egzistuoja id neegzistuoja!");
            }

            var playerWithSameEmail = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(user => user.NormalizedEmailAddress == playerInfo.UserName.ToUpper() && userId != user.Id);

            if (playerWithSameEmail != null)
            {
                throw new UserFriendlyException("Vartotojas su tokiu el. paštu jau egzistuoja id neegzistuoja!");
            }

            var city = await _cityRepository.FirstOrDefaultAsync(c => c.Id == playerInfo.CityId);
            if (city == null)
            {
                throw new UserFriendlyException("Nurodytas miestas neegzistuoja!");
            }

            player.Name = playerInfo.Name;
            player.Surname = playerInfo.Surname;
            player.EmailAddress = playerInfo.EmailAddress;
            player.UserName = playerInfo.UserName;
            player.City = city;
            player.Birthdate = playerInfo.Birthdate;

            await _userManager.UpdateAsync(player);
        }

        public async Task<List<PlayerDto>> GetAllPlayers()
        {
            var players = await _userManager.Users
                .IgnoreQueryFilters()
                .Include(player => player.Team)
                .ToListAsync();

            var playersDto = new List<PlayerDto>();

            foreach (var player in players)
            {
                var playerDto = new PlayerDto();
                player.MapTo(playerDto);
                playerDto.TeamName = player.Team?.Name;
                playersDto.Add(playerDto);
            }
            var sortedPlayers = playersDto.OrderByDescending(player => player.Points).ToList();

            return sortedPlayers;
        }
    }
}

