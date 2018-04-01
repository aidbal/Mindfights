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

namespace Mindfights.Services.TeamService
{
    [AbpMvcAuthorize]
    public class TeamService : ITeamService
    {
        private readonly IRepository<Team, long> _teamRepository;
        private readonly UserManager _userManager;
        private readonly IObjectMapper _objectMapper;

        public TeamService(IRepository<Team, long> teamRepository, UserManager userManager, IObjectMapper objectMapper)
        {
            _teamRepository = teamRepository;
            _userManager = userManager;
            _objectMapper = objectMapper;
        }

        public async Task<long> CreateTeam(TeamDto team)
        {
            var user = _userManager.Users.IgnoreQueryFilters().FirstOrDefault(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("User does not exist.");

            var currentUserTeam = await _teamRepository.FirstOrDefaultAsync(x => x.LeaderId == user.Id);
            if (currentUserTeam != null)
                throw new UserFriendlyException("User already has created a team!");
            
            var teamWithSameName = _teamRepository.FirstOrDefaultAsync(x => x.Name == team.Name);
            if (teamWithSameName != null)
                throw new UserFriendlyException("Team with the same name already exists");
            
            var teamToInsert = new Team(user, team.Name, team.Description);
            return await _teamRepository.InsertAndGetIdAsync(teamToInsert);
        }

        public async Task<TeamDto> GetTeam(long teamId)
        {
            var team = await _teamRepository.FirstOrDefaultAsync(x => x.Id == teamId);
            if (team == null)
                throw new UserFriendlyException("Specified team does not exist or is deleted!");

            var teamDto = new TeamDto();
            team.MapTo(teamDto);
            teamDto.LeaderName = _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == team.LeaderId).Result.Name;
            teamDto.UsersCount = team.UsersCount;
            return teamDto;
        }

        public async Task<List<TeamDto>> GetAllTeams()
        {
            var teams = await _teamRepository.GetAll().ToListAsync();
            var teamsDto = new List<TeamDto>();
            _objectMapper.Map(teams, teamsDto);
            for (var i = 0; i < teams.Count; i++)
                teamsDto[i].LeaderName = _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == teams[i].LeaderId).Result.Name;

            return teamsDto;
        }

        public async Task UpdateTeam(TeamDto team, long teamId)
        {
            var currentTeam = await _teamRepository.FirstOrDefaultAsync(x => x.Id == teamId);
            if (currentTeam == null)
                throw new UserFriendlyException("Specified team does not exist!");

            var teamWithSameName = await _teamRepository.FirstOrDefaultAsync(x => x.Name == team.Name);
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
            
            foreach (var user in currentTeam.Users)
            {
                user.Team = null;
            }
            await _teamRepository.DeleteAsync(currentTeam);
        }

        public async Task InsertUser(long teamId, long userId)
        {
            var currentTeam = await _teamRepository.GetAllIncluding(x => x.Users).FirstOrDefaultAsync(x => x.Id == teamId);
            if (currentTeam == null)
                throw new UserFriendlyException("Specified team does not exist or is deleted!");
            
            var currentLeaderTeam = await _teamRepository.FirstOrDefaultAsync(x => x.LeaderId == userId);
            if (currentLeaderTeam != null)
                throw new UserFriendlyException("User already has created a team!");
            
            var currentUser = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == userId);
            if (currentUser == null)
                throw new UserFriendlyException("User does not exist or is deleted!");
            
            if (currentUser.Team != null)
                throw new UserFriendlyException("User already has a team!");
            
            currentTeam.Users.Add(currentUser);
            currentTeam.UsersCount += 1;
            await _teamRepository.UpdateAsync(currentTeam);
        }

        public async Task RemoveUser(long teamId, long userId)
        {
            var currentTeam = await _teamRepository.GetAllIncluding(x => x.Users).FirstOrDefaultAsync(x => x.Id == teamId);
            if (currentTeam == null)
                throw new UserFriendlyException("Specified team does not exist or is deleted!");

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
                await _teamRepository.UpdateAsync(currentTeam);
            }
        }

        public async Task<List<string>> GetAllTeamPlayers(long teamId)
        {
            var currentTeam = await _teamRepository.GetAllIncluding(x => x.Users).FirstOrDefaultAsync(x => x.Id == teamId);
            if (currentTeam == null)
                throw new UserFriendlyException("Specified team does not exist or is deleted!");

            var usersInTeam = await _userManager.Users.IgnoreQueryFilters().Where(x => x.TeamId == teamId).ToListAsync();
            return usersInTeam.Select(user => user.Name).ToList();
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

            if (newLeader.Team != null)
                throw new UserFriendlyException("User is in another team!");


            if (currentTeam.Users.Remove(currentLeader))
            {
                currentTeam.LeaderId = newLeaderId;
                await _teamRepository.UpdateAsync(currentTeam);
            }
            else
            {
                throw new UserFriendlyException("There was a problem removing leader.!");
            }
        }
    }
}
