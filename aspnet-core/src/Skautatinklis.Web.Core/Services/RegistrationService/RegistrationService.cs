using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Skautatinklis.Authorization.Users;
using Skautatinklis.DTOs;
using Skautatinklis.Models;

namespace Skautatinklis.Services.RegistrationService
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRepository<Mindfight, long> _mindfightRepository;
        private readonly IRepository<Team, long> _teamRepository;
        private readonly IRepository<MindfightRegistration, long> _registrationRepository;
        private readonly UserManager _userManager;

        public RegistrationService(
            IRepository<Mindfight, long> mindfightRepository,
            IRepository<MindfightRegistration, long> registrationRepository,
            IRepository<Team, long> teamRepository,
            UserManager userManager)
        {
            _mindfightRepository = mindfightRepository;
            _registrationRepository = registrationRepository;
            _teamRepository = teamRepository;
            _userManager = userManager;
        }

        public async Task<long> CreateRegistration(long mindfightId, long teamId, long userId)
        {
            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .Include(x => x.Team)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            var currentMindfight = await _mindfightRepository
                .GetAllIncluding(x => x.AllowedTeams)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);

            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            if (currentMindfight.StartTime < Clock.Now)
                throw new UserFriendlyException("Mindfight has already started!");

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
                throw new UserFriendlyException("Team with specified id does not exist!");

            if (currentMindfight.IsPrivate && currentMindfight.AllowedTeams.All(x => x.TeamId != teamId))
                throw new UserFriendlyException("Team is not allowed to register to this private mindfight!");

            var currentRegistration = await _registrationRepository
                                          .FirstOrDefaultAsync(x => x.MindfightId == mindfightId && x.TeamId == teamId) ??
                                      new MindfightRegistration(currentMindfight, currentTeam);

            currentRegistration.CreationTime = Clock.Now;
            return await _registrationRepository.InsertOrUpdateAndGetIdAsync(currentRegistration);
        }

        public async Task DeleteRegistration(long mindfightId, long teamId, long userId)
        {
            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            var currentMindfight = await _mindfightRepository
                .GetAllIncluding(x => x.AllowedTeams)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);

            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
                throw new UserFriendlyException("Team with specified id does not exist!");

            var currentRegistration = await _registrationRepository
                .FirstOrDefaultAsync(x => x.MindfightId == mindfightId && x.TeamId == teamId);

            if (currentRegistration == null)
                throw new UserFriendlyException("Team is not registered to this mindfight!");

            if (currentMindfight.StartTime.AddDays(-1) < Clock.Now)
                await _registrationRepository.DeleteAsync(currentRegistration);
        }

        public async Task<List<RegistrationDto>> GetTeamRegistrations(long teamId, long userId)
        {
            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
                throw new UserFriendlyException("Team with specified id does not exist!");

            var currentMindfights = await _mindfightRepository
                .GetAllIncluding(x => x.AllowedTeams)
                .Where(x => x.IsActive && !x.IsFinished && x.IsConfirmed 
                    && (!x.IsPrivate || x.AllowedTeams.Any(t => t.TeamId == teamId))
                    && x.StartTime > Clock.Now)
                .ToListAsync();

            var currentRegistrations = await _registrationRepository
                .GetAllIncluding(x => x.Mindfight, x => x.Team)
                .Where(x => currentMindfights.Any(m => m.Id == x.MindfightId) && x.TeamId == teamId)
                .ToListAsync();

            return currentRegistrations.Select(registration => new RegistrationDto
                {
                    MindfightId = registration.MindfightId,
                    CreationTime = registration.CreationTime,
                    IsMindfightPrivate = registration.Mindfight.IsPrivate,
                    MindfightName = registration.Mindfight.Title,
                    MindfightStartTime = registration.Mindfight.StartTime,
                    TeamId = registration.TeamId,
                    TeamName = registration.Team.Name
                })
                .ToList();
        }

        public async Task<List<RegistrationDto>> GetMindfightRegistrations(long mindfightId, long userId)
        {
            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            var currentMindfight = await _mindfightRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == mindfightId);

            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            var currentRegistrations = await _registrationRepository
                .GetAllIncluding(x => x.Mindfight, x => x.Team)
                .Where(x => mindfightId == x.MindfightId)
                .ToListAsync();

            return currentRegistrations.Select(registration => new RegistrationDto
                {
                    MindfightId = registration.MindfightId,
                    CreationTime = registration.CreationTime,
                    IsMindfightPrivate = registration.Mindfight.IsPrivate,
                    MindfightName = registration.Mindfight.Title,
                    MindfightStartTime = registration.Mindfight.StartTime,
                    TeamId = registration.TeamId,
                    TeamName = registration.Team.Name
                })
                .ToList();
        }
    }
}
