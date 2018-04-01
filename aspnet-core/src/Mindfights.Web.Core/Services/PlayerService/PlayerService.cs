using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Mindfights.Authorization.Users;
using System.Threading.Tasks;

namespace Mindfights.Services.PlayerService
{
    public class PlayerService : IPlayerService
    {
        private readonly UserManager _userManager;

        public PlayerService(UserManager userManager)
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

        public async Task<string> GetTeam(long userId)
        {
            var player = await _userManager.Users.IgnoreQueryFilters().Include(x => x.Team).FirstOrDefaultAsync(x => x.Id == userId);
            if (player == null)
                throw new UserFriendlyException("The player with specified id does not exist!");

            var team = player.Team;
            if (team == null)
                throw new UserFriendlyException("The player does not have any team!");

            return team.Name;
        }

        //public async Task<string> GetScoutGroup(long userId)
        //{
        //    var player = await _userManager.Users.IgnoreQueryFilters().Include(x => x.ScoutGroup).FirstOrDefaultAsync(x => x.Id == userId);
        //    if (player == null)
        //        throw new UserFriendlyException("The player with specified id does not exist!");

        //    var scoutGroup = player.ScoutGroup;
        //    if (scoutGroup == null)
        //        throw new UserFriendlyException("The player does not have any team!");

        //    return scoutGroup.Name;
        //}

        //public async Task<List<Mindfight>> GetAllowedEvaluateMindfights(long userId)
        //{
        //    var player = await _userManager.Users.IgnoreQueryFilters().Include(x => x.Mindfights).ThenInclude(m => m.Mindfight).FirstOrDefaultAsync(x => x.Id == userId);
        //    if (player == null)
        //    {
        //        throw new UserFriendlyException("The player with specified Id does not exist.");
        //    }
        //    return player.Mindfights.Select(mindfight => mindfight.Mindfight).ToList();
        //}

        //public Task<Mindfight> GetPlayedMindfightsHistory(long userId)
        //{

        //}

        //public Task<List<TeamAnswer>> GetPlayedMindfightAnswers(long userId, long mindfightId)
        //{

        //}
    }
}
