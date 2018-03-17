using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.ObjectMapping;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Skautatinklis.Authorization.Users;
using Skautatinklis.DTOs;
using Skautatinklis.Models;

namespace Skautatinklis.Services
{
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

        public async Task<long> CreateTeam(long? userId, TeamDto team)
        {
            if (team.Name.IsNullOrWhiteSpace())
            {
                throw new UserFriendlyException("Team name was not entered.");
            }
            var userToSelectId = userId ?? _userManager.AbpSession.UserId;
            var user = _userManager.Users.IgnoreQueryFilters().FirstOrDefault(u => u.Id == userToSelectId);
            if (user == null)
            {
                throw new UserFriendlyException("User does not exist.");
            }
            var currentUserTeam = await _teamRepository.FirstOrDefaultAsync(x => x.LeaderId == user.Id);
            if (currentUserTeam != null)
            {
                throw new UserFriendlyException("User already has created a team!");
            }
            var teamWithSameName = _teamRepository.FirstOrDefaultAsync(x => x.Name == team.Name);
            if (teamWithSameName != null)
            {
                throw new UserFriendlyException("Team with the same name already exists");
            }
            var teamToInsert = new Team(user, team.Name, team.Description);
            return await _teamRepository.InsertAndGetIdAsync(teamToInsert);
        }

        public async Task<TeamDto> GetTeam(long teamId)
        {
            var team = await _teamRepository.FirstOrDefaultAsync(x => x.Id == teamId);
            TeamDto teamDto = new TeamDto();
            if (team != null)
            {
                team.MapTo(teamDto);
                teamDto.LeaderName = _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == team.LeaderId).Result.Name;
                teamDto.PlayersCount = team.UsersCount;
            }
            return teamDto;
        }

        public async Task<List<TeamDto>> GetAllTeams()
        {
            var teams = await _teamRepository.GetAll().Include(i => i.Users).ToListAsync();
            var teamsDto = new List<TeamDto>();
            _objectMapper.Map(teams, teamsDto);
            return teamsDto;
        }
    }
}
