using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Mindfights.Authorization.Users;
using Mindfights.DTOs;
using Mindfights.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mindfights.Services.PlayerService
{
    public class Player : IPlayerService
    {
        private readonly UserManager _userManager;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IRepository<City, long> _cityRepository;
        private readonly IObjectMapper _objectMapper;

        public Player(
            UserManager userManager,
            IPermissionChecker permissionChecker,
            IRepository<City, long> cityRepository,
            IObjectMapper objectMapper
            )
        {
            _userManager = userManager;
            _permissionChecker = permissionChecker;
            _cityRepository = cityRepository;
            _objectMapper = objectMapper;
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

            _objectMapper.Map(player, playerDto);

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
                _objectMapper.Map(player, playerDto);
                playerDto.TeamName = player.Team?.Name;
                playersDto.Add(playerDto);
            }
            var sortedPlayers = playersDto.OrderByDescending(player => player.Points).ToList();

            return sortedPlayers;
        }
    }
}

