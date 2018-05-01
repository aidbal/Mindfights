using Abp.AspNetCore.Mvc.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Mindfights.Authorization.Users;
using Mindfights.DTOs;
using Mindfights.Models;
using System.Collections.Generic;
using System.Linq;
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
                throw new UserFriendlyException("Vartotojas neegzistuoja!");

            var currentUserTeam = await _teamRepository.FirstOrDefaultAsync(x => x.LeaderId == user.Id);
            if (currentUserTeam != null)
                throw new UserFriendlyException("Vartotojas jau turi komandą!");
            
            var teamWithSameName = await _teamRepository.FirstOrDefaultAsync(x => string.CompareOrdinal(x.Name.ToUpper(), team.Name.ToUpper()) == 0);
            if (teamWithSameName != null)
                throw new UserFriendlyException("Komanda su tokiu pačiu pavadinimu jau egzistuoja!");
            
            var teamToInsert = new Models.Team(user, team.Name, team.Description);
            user.IsActiveInTeam = true;
            return await _teamRepository.InsertAndGetIdAsync(teamToInsert);
        }

        public async Task<TeamDto> GetTeam(long teamId)
        {
            var currentTeam = await _teamRepository
                .GetAllIncluding(team => team.Players)
                .FirstOrDefaultAsync(x => x.Id == teamId);
            if (currentTeam == null)
                throw new UserFriendlyException("Komanda su tokiu id neegizstuoja1");

            var teamDto = new TeamDto();
            currentTeam.MapTo(teamDto);
            var leader = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == currentTeam.LeaderId);
            teamDto.LeaderName = leader.Name;
            teamDto.PlayersCount = currentTeam.Players.Count;
            teamDto.GamePoints = currentTeam.Players.Sum(player => player.Points);
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
                throw new UserFriendlyException("Komanda su tokiu id neegzistuoja!");

            var teamWithSameName = await _teamRepository.FirstOrDefaultAsync(x => string.CompareOrdinal(x.Name.ToUpper(), team.Name.ToUpper()) == 0 && x.Id != teamId);
            if (teamWithSameName != null)
                throw new UserFriendlyException("Komanda su tokiu pačiu pavadinimu jau egzistuoja!");
            
            currentTeam.Description = team.Description;
            currentTeam.Name = team.Name;
            await _teamRepository.UpdateAsync(currentTeam);
        }

        public async Task DeleteTeam(long teamId)
        {
            var currentTeam = await _teamRepository.GetAllIncluding(x => x.Players).FirstOrDefaultAsync(x => x.Id == teamId);
            if (currentTeam == null)
                throw new UserFriendlyException("Komanda su tokiu id neegzistuoja!");

            if (!(currentTeam.LeaderId == _userManager.AbpSession.UserId || _permissionChecker.IsGranted("Pages.Users")))
                throw new AbpAuthorizationException("Jūs neturite teisių trinti šios komandos!");

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
                throw new UserFriendlyException("Vartotojas neegzistuoja!");

            if (currentUser.Team == null)
                throw new UserFriendlyException("Vartotojas neturi komandos!");

            var currentTeam = await _teamRepository.GetAllIncluding(x => x.Players).FirstOrDefaultAsync(x => x.Id == currentUser.Team.Id);
            if (currentTeam == null)
                throw new UserFriendlyException("Vartotojas yra kitoje komandoje!");

            if (currentUser.Id == currentTeam.LeaderId)
            {
                throw new UserFriendlyException("Negalima keisti kapitono statuso!");
            }

            if (!(_permissionChecker.IsGranted("Pages.Users") || currentTeam.LeaderId == _userManager.AbpSession.UserId))
                throw new AbpAuthorizationException("Jūs neturite komandos redagavimo teisių!");

            currentUser.IsActiveInTeam = status;
        }

        public async Task InsertUser(long teamId, string username)
        {
            var currentTeam = await _teamRepository.GetAllIncluding(x => x.Players).FirstOrDefaultAsync(x => x.Id == teamId);
            if (currentTeam == null)
            {
                throw new UserFriendlyException("Komanda su tokiu id neegzistuoja");
            }
            
            var currentUser = await _userManager.Users.IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => string.CompareOrdinal(x.UserName.ToUpper(), username.ToUpper()) == 0);
            if (currentUser == null)
            {
                throw new UserFriendlyException("Vartotojas neegzistuoja!");
            }

            if (!(currentTeam.LeaderId == _userManager.AbpSession.UserId ||
                  _permissionChecker.IsGranted("Pages.Users")))
            {
                throw new AbpAuthorizationException("Jūs neturite komandos redagavimo teisių!");
            }

            if (currentUser.Team != null)
            {
                throw new UserFriendlyException("Vartotojas yra kitoje komandoje!");
            }

            currentTeam.Players.Add(currentUser);
            currentUser.IsActiveInTeam = false;
            await _teamRepository.UpdateAsync(currentTeam);
        }

        public async Task RemoveUser(long teamId, long userId)
        {
            var currentTeam = await _teamRepository.GetAllIncluding(x => x.Players).FirstOrDefaultAsync(x => x.Id == teamId);
            if (currentTeam == null)
                throw new UserFriendlyException("Nurodyta komanda neegzistuoja!");

            if (!(currentTeam.LeaderId == _userManager.AbpSession.UserId || _permissionChecker.IsGranted("Pages.Users")))
                throw new AbpAuthorizationException("Jūs neturite komandos redagavimo teisių!");

            var currentLeaderTeam = await _teamRepository.FirstOrDefaultAsync(x => x.LeaderId == userId);
            if (currentLeaderTeam != null)
                throw new UserFriendlyException("Vartotojas yra komandos kapitonas!");

            var currentUser = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == userId);
            if (currentUser == null)
                throw new UserFriendlyException("Vartotojas neegzistuoja!");

            if (currentUser.Team == null)
                throw new UserFriendlyException("Vartotojas neturi komandos!");

            if (currentTeam.Players.Remove(currentUser))
            {
                currentUser.IsActiveInTeam = false;
                await _teamRepository.UpdateAsync(currentTeam);
            }
        }

        public async Task<List<TeamPlayerDto>> GetAllTeamPlayers(long teamId)
        {
            var currentTeam = await _teamRepository.GetAllIncluding(x => x.Players).FirstOrDefaultAsync(x => x.Id == teamId);
            if (currentTeam == null)
                throw new UserFriendlyException("Komanda su nurodytu id neegzistuoja!");

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
            var currentTeam = await _teamRepository.GetAllIncluding(x => x.Players).FirstOrDefaultAsync(x => x.Id == teamId);
            if (currentTeam == null)
                throw new UserFriendlyException("Komanda su nurodytu id neegzistuoja!");

            var currentLeader = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == currentTeam.LeaderId);
            if (currentLeader == null)
                throw new UserFriendlyException("Klaida gaunant komandos kapitoną!");

            var newLeader = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == newLeaderId);
            if (newLeader == null)
                throw new UserFriendlyException("Vartotojas neegzistuoja");

            if (newLeader.Team != null && newLeader.Team != currentTeam)
                throw new UserFriendlyException("Vartotojas yra kitoje komandoje!");

            
            currentTeam.LeaderId = newLeaderId;
            newLeader.IsActiveInTeam = true;
            currentLeader.IsActiveInTeam = false;
            await _teamRepository.UpdateAsync(currentTeam);
        }

        public async Task LeaveCurrentTeam()
        {
            var currentUser = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == _userManager.AbpSession.UserId);
            if (currentUser == null)
            {
                throw new UserFriendlyException("Problema gaunant vartotoją!");
            }

            if (currentUser.TeamId == null)
            {
                throw new UserFriendlyException("Vartotojas neturi komandos!");
            }

            var currentTeam = await _teamRepository
                .GetAllIncluding(x => x.Players)
                .FirstOrDefaultAsync(x => x.Id == currentUser.TeamId);

            if (currentTeam == null)
            {
                throw new UserFriendlyException("Problema gaunant komandą!");
            }

            if (currentUser.Id == currentTeam.LeaderId)
            {
                throw new UserFriendlyException("Vartotojas yra komandos kapitonas!");
            }

            if (currentTeam.Players.Remove(currentUser))
            {
                currentUser.IsActiveInTeam = false;
                await _teamRepository.UpdateAsync(currentTeam);
            }
        }
    }
}
