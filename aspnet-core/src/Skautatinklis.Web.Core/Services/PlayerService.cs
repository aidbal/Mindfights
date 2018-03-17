using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Skautatinklis.Authorization.Users;
using Skautatinklis.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skautatinklis.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly UserManager _userManager;

        public PlayerService(UserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<int> GetPlayerPoints(long userId)
        {
            var currentUserId = NullAbpSession.Instance.UserId;
            var player = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == userId);
            if (player == null)
            {
                throw new UserFriendlyException("The player with specified Id does not exist.");
            }
            return player.Points;
        }

        public void ConfirmUser(long userId)
        {
            
        }

        public async Task<string> GetTeam(long userId)
        {
            var player = await _userManager.Users.IgnoreQueryFilters().Include(x => x.Team).FirstOrDefaultAsync(x => x.Id == userId);
            if (player == null)
            {
                throw new UserFriendlyException("The player with specified Id does not exist.");
            }
            var team = player.Team;
            if (team == null)
            {
                throw new UserFriendlyException("The player does not have any team.");
            }
            return team.Name;
        }

        public void RemoveFromTeam(long userId)
        {
            var player = _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == userId);
            if (player.Result == null)
            {
                throw new UserFriendlyException("The player with specified Id does not exist.");
            }
            var team = player.Result.Team;
            if (team == null)
            {
                throw new UserFriendlyException("The player does not have any team.");
            }
            team.Users.Remove(player.Result);
        }

        public async Task<List<Mindfight>> GetAllowedEvaluateMindfights(long userId)
        {
            var player = await _userManager.Users.IgnoreQueryFilters().Include(x => x.Mindfights).ThenInclude(m => m.Mindfight).FirstOrDefaultAsync(x => x.Id == userId);
            if (player == null)
            {
                throw new UserFriendlyException("The player with specified Id does not exist.");
            }
            var collection = new List<Mindfight>();
            foreach (var mindfight in player.Mindfights)
            {
                collection.Add(mindfight.Mindfight);
            }
            return collection;
        }

        //public Task<Mindfight> GetPlayedMindfightsHistory(long userId)
        //{

        //}

        //public Task<List<TeamAnswer>> GetPlayedMindfightAnswers(long userId, long mindfightId)
        //{

        //}
    }
}
