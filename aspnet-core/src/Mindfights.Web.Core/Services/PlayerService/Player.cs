using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Mindfights.Authorization.Users;
using System.Threading.Tasks;

namespace Mindfights.Services.PlayerService
{
    public class Player : IPlayerService
    {
        private readonly UserManager _userManager;

        public Player(UserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<int> GetPlayerPoints(long? userId)
        {
            var currentUserId = userId ?? NullAbpSession.Instance.UserId;
            var player = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == currentUserId);
            if (player == null)
                throw new UserFriendlyException("The player with specified id does not exist!");

            return player.Points;
        }

        public async Task<long> GetPlayerTeam(long userId)
        {
            long teamId = 0;
            var player = await _userManager.Users
                .IgnoreQueryFilters()
                .Include(x => x.Team)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (player != null)
            {
                var team = player.Team;
                if (team != null)
                {
                    teamId = team.Id;
                }
            }
            return teamId;
        }
    }
}
