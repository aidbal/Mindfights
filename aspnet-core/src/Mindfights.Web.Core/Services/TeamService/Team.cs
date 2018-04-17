using Abp.AspNetCore.Mvc.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Mindfights.Authorization.Users;
using Mindfights.DTOs;
using Mindfights.Models;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Authorization;

namespace Mindfights.Services.TeamService
{
    [AbpMvcAuthorize]
    public class Team : ITeamService
    {
        private readonly IRepository<Models.Team, long> _teamRepository;
        private readonly IRepository<Registration, long> _registrationRepository;
        private readonly UserManager _userManager;
        private readonly IPermissionChecker _permissionChecker;

        public Team(
            IRepository<Models.Team, long> teamRepository, 
            IRepository<Registration, long> registrationRepository, 
            UserManager userManager, 
            IPermissionChecker permissionChecker)
        {
            _teamRepository = teamRepository;
            _registrationRepository = registrationRepository;
            _userManager = userManager;
            _permissionChecker = permissionChecker;
        }

        public async Task<long> CreateTeam(TeamDto team)
        {
            var user = _userManager.Users.IgnoreQueryFilters().FirstOrDefault(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("User does not exist.");

            var currentUserTeam = await _teamRepository.FirstOrDefaultAsync(x => x.LeaderId == user.Id);
            if (currentUserTeam != null)
                throw new UserFriendlyException("User already has created a team!");
            
            var teamWithSameName = await _teamRepository.FirstOrDefaultAsync(x => string.CompareOrdinal(x.Name.ToUpper(), team.Name.ToUpper()) == 0);
            if (teamWithSameName != null)
                throw new UserFriendlyException("Team with the same name already exists");
            
            var teamToInsert = new Models.Team(user, team.Name, team.Description);
            user.IsActiveInTeam = true;
            return await _teamRepository.InsertAndGetIdAsync(teamToInsert);
        }

        public async Task<TeamDto> GetTeam(long teamId)
        {
            var team = await _teamRepository.FirstOrDefaultAsync(x => x.Id == teamId);
            if (team == null)
                throw new UserFriendlyException("Specified team does not exist or is deleted!");

            var teamDto = new TeamDto();
            team.MapTo(teamDto);
            var leader = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == team.LeaderId);
            teamDto.LeaderName = leader.Name;
            teamDto.UsersCount = team.UsersCount;
            return teamDto;
        }

        public async Task<List<TeamDto>> GetAllTeams()
        {
            var teams = await _teamRepository.GetAll().ToListAsync();
            var teamsDto = new List<TeamDto>();
            teams.MapTo(teamsDto);
            for (var i = 0; i < teams.Count; i++)
                teamsDto[i].LeaderName = _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == teams[i].LeaderId).Result.Name;

            return teamsDto;
        }

        public async Task UpdateTeam(TeamDto team, long teamId)
        {
            var currentTeam = await _teamRepository.FirstOrDefaultAsync(x => x.Id == teamId);
            if (currentTeam == null)
                throw new UserFriendlyException("Specified team does not exist!");

            var teamWithSameName = await _teamRepository.FirstOrDefaultAsync(x => string.CompareOrdinal(x.Name.ToUpper(), team.Name.ToUpper()) == 0 && x.Id != teamId);
            if (teamWithSameName != null)
                throw new UserFriendlyException("Team with the same name already exists!");
            
            currentTeam.Description = team.Description;
            currentTeam.Name = team.Name;
            await _teamRepository.UpdateAsync(currentTeam);
        }

        public async Task DeleteTeam(long teamId)
        {
            var currentTeam = await _teamRepository.GetAllIncluding(x => x.Users).FirstOrDefaultAsync(x => x.Id == teamId);
            if (currentTeam == null)
                throw new UserFriendlyException("Specified team does not exist or is deleted!");

            if (!(currentTeam.LeaderId == _userManager.AbpSession.UserId || _permissionChecker.IsGranted("Pages.Users")))
                throw new AbpAuthorizationException("You don't have the permission to delete this team!");

            var teamPlayers = await _userManager.Users
                .IgnoreQueryFilters()
                .Where(x => x.TeamId == teamId)
                .ToListAsync();

            foreach (var player in teamPlayers)
            {
                player.Team = null;
                player.IsActiveInTeam = false;
            }

            await _registrationRepository.DeleteAsync(x => x.TeamId == teamId);
            await _teamRepository.DeleteAsync(currentTeam);
        }

        public async Task UpdateUserActiveStatus(long userId, bool status)
        {
            var currentUser = await _userManager.Users
                .IgnoreQueryFilters()
                .Include(x => x.Team)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (currentUser == null)
                throw new UserFriendlyException("User does not exist or is deleted!");

            if (currentUser.Team == null)
                throw new UserFriendlyException("User is not in a team!");

            var currentTeam = await _teamRepository.GetAllIncluding(x => x.Users).FirstOrDefaultAsync(x => x.Id == currentUser.Team.Id);
            if (currentTeam == null)
                throw new UserFriendlyException("User is in another team!");

            if (currentUser.Id == currentTeam.LeaderId)
            {
                throw new UserFriendlyException("Cannot change team leader status!");
            }

            if (!(_permissionChecker.IsGranted("Pages.Users") || currentTeam.LeaderId == _userManager.AbpSession.UserId))
                throw new AbpAuthorizationException("You don't have the permission to activate user!");

            currentUser.IsActiveInTeam = status;
        }

        public async Task InsertUser(long teamId, string username)
        {
            var currentTeam = await _teamRepository.GetAllIncluding(x => x.Users).FirstOrDefaultAsync(x => x.Id == teamId);
            if (currentTeam == null)
            {
                throw new UserFriendlyException("Specified team does not exist or is deleted!");
            }
            
            var currentUser = await _userManager.Users.IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => string.CompareOrdinal(x.UserName.ToUpper(), username.ToUpper()) == 0);
            if (currentUser == null)
            {
                throw new UserFriendlyException("User does not exist or is deleted!");
            }

            if (!(currentTeam.LeaderId == _userManager.AbpSession.UserId ||
                  _permissionChecker.IsGranted("Pages.Users")))
            {
                throw new AbpAuthorizationException("You don't have the permission to insert user!");
            }

            var currentLeaderTeam = await _teamRepository.FirstOrDefaultAsync(x => x.LeaderId == currentUser.Id);
            if (currentLeaderTeam != null)
            {
                throw new UserFriendlyException("User already has created a team!");
            }

            if (currentUser.Team != null)
            {
                throw new UserFriendlyException("User already has a team!");
            }

            currentTeam.Users.Add(currentUser);
            currentTeam.UsersCount += 1;
            currentUser.IsActiveInTeam = false;
            await _teamRepository.UpdateAsync(currentTeam);
        }

        public async Task RemoveUser(long teamId, long userId)
        {
            var currentTeam = await _teamRepository.GetAllIncluding(x => x.Users).FirstOrDefaultAsync(x => x.Id == teamId);
            if (currentTeam == null)
                throw new UserFriendlyException("Specified team does not exist or is deleted!");

            if (!(currentTeam.LeaderId == _userManager.AbpSession.UserId || _permissionChecker.IsGranted("Pages.Users")))
                throw new AbpAuthorizationException("You don't have the permission to remove user!");

            var currentLeaderTeam = await _teamRepository.FirstOrDefaultAsync(x => x.LeaderId == userId);
            if (currentLeaderTeam != null)
                throw new UserFriendlyException("User is leader of the team!");

            var currentUser = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == userId);
            if (currentUser == null)
                throw new UserFriendlyException("User does not exist or is deleted!");

            if (currentUser.Team == null)
                throw new UserFriendlyException("User does not have any team!");

            if (currentTeam.Users.Remove(currentUser))
            {
                currentTeam.UsersCount -= 1;
                currentUser.IsActiveInTeam = false;
                await _teamRepository.UpdateAsync(currentTeam);
            }
        }

        public async Task<List<TeamPlayerDto>> GetAllTeamPlayers(long teamId)
        {
            var currentTeam = await _teamRepository.GetAllIncluding(x => x.Users).FirstOrDefaultAsync(x => x.Id == teamId);
            if (currentTeam == null)
                throw new UserFriendlyException("Specified team does not exist or is deleted!");

            var usersInTeam = await _userManager.Users.IgnoreQueryFilters().Where(x => x.TeamId == teamId).ToListAsync();
            return usersInTeam.Select(user => new TeamPlayerDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Points = user.Points,
                    IsActiveInTeam = user.IsActiveInTeam
                })
                .ToList();
        }

        public async Task ChangeTeamLeader(long teamId, long newLeaderId)
        {
            var currentTeam = await _teamRepository.GetAllIncluding(x => x.Users).FirstOrDefaultAsync(x => x.Id == teamId);
            if (currentTeam == null)
                throw new UserFriendlyException("Specified team does not exist or is deleted!");

            var currentLeader = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == currentTeam.LeaderId);
            if (currentLeader == null)
                throw new UserFriendlyException("There was a problem getting current leader!");

            var newLeader = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == newLeaderId);
            if (newLeader == null)
                throw new UserFriendlyException("User does not exist or is deleted!");

            if (newLeader.Team != null && newLeader.Team != currentTeam)
                throw new UserFriendlyException("User is in another team!");

            
            currentTeam.LeaderId = newLeaderId;
            newLeader.IsActiveInTeam = true;
            currentLeader.IsActiveInTeam = false;
            await _teamRepository.UpdateAsync(currentTeam);
        }
    }
}
