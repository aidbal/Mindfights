using System.Collections.Generic;
using System.Linq;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Mindfights.Authorization.Users;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Mindfights.Models;
using Mindfights.DTOs;

namespace Mindfights.Services.PlayerService
{
    public class Player : IPlayerService
    {
        private readonly UserManager _userManager;
        private readonly IRepository<Mindfight, long> _mindfightRepository;
        private readonly IPermissionChecker _permissionChecker;

        public Player(
            UserManager userManager,
            IRepository<Mindfight, long> mindfightRepository,
            IPermissionChecker permissionChecker
            )
        {
            _userManager = userManager;
            _mindfightRepository = mindfightRepository;
            _permissionChecker = permissionChecker;
        }

        public async Task<PlayerDto> GetPlayerInfo(long userId)
        {
            var playerDto = new PlayerDto();
            var player = await _userManager.Users
                .IgnoreQueryFilters()
                .Include(x => x.Team)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (player == null)
            {
                throw new UserFriendlyException("Player with specified id does not exist!");
            }
            player.MapTo(playerDto);

            if (player.Team != null)
            {
                playerDto.TeamId = player.Team.Id;
                playerDto.IsTeamLeader = player.Team.LeaderId == player.Id;
                playerDto.IsActiveInTeam = player.IsActiveInTeam;
            }
            return playerDto;
        }

        public async Task<List<PlayerDto>> GetMindfightEvaluators(long mindfightId)
        {
            var currentMindfight = await _mindfightRepository
                .GetAllIncluding(x => x.Evaluators)
                .Where(x => x.Id == mindfightId).FirstOrDefaultAsync();

            if (currentMindfight == null)
            {
                throw new UserFriendlyException("Mindfight with specified id does not exist!");
            }

            if (!(currentMindfight.CreatorId == _userManager.AbpSession.UserId ||
                  _permissionChecker.IsGranted("ManageMindfights")))
            {
                throw new AbpAuthorizationException("You are not creator of this mindfight!");
            }

            var evaluators = await _userManager.Users
                .IgnoreQueryFilters()
                .Where(x => currentMindfight.Evaluators.Any(y => x.Id == y.UserId))
                .ToListAsync();

            var evaluatorsDto = new List<PlayerDto>();
            evaluators.MapTo(evaluatorsDto);
            return evaluatorsDto;
        }
    }
}

