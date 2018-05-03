using Abp.AspNetCore.Mvc.Authorization;
using Abp.Domain.Repositories;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Mindfights.Authorization.Users;
using Mindfights.DTOs;
using Mindfights.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.AutoMapper;

namespace Mindfights.Services.RegistrationService
{
    [AbpMvcAuthorize]
    public class Registration : IRegistrationService
    {
        private readonly IRepository<Mindfight, long> _mindfightRepository;
        private readonly IRepository<Team, long> _teamRepository;
        private readonly IRepository<Models.Registration, long> _registrationRepository;
        private readonly IPermissionChecker _permissionChecker;
        private readonly UserManager _userManager;

        public Registration(
            IRepository<Mindfight, long> mindfightRepository,
            IRepository<Team, long> teamRepository,
            IRepository<Models.Registration, long> registrationRepository,
            IPermissionChecker permissionChecker,
            UserManager userManager)
        {
            _mindfightRepository = mindfightRepository;
            _teamRepository = teamRepository;
            _registrationRepository = registrationRepository;
            _permissionChecker = permissionChecker;
            _userManager = userManager;
        }

        public async Task<long> CreateRegistration(long mindfightId, long teamId)
        {
            var currentMindfight = await _mindfightRepository
                .FirstOrDefaultAsync(x => x.Id == mindfightId);

            if (currentMindfight == null)
                throw new UserFriendlyException("Protmūšis su nurodyti id neegzistuoja!");

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
                throw new UserFriendlyException("Komanda su nurodytu id neegzistuoja!");

            if(_userManager.AbpSession.UserId != currentTeam.LeaderId)
                throw new UserFriendlyException("Jūs nesate komandos kapitonas!");

            var currentRegistration = await _registrationRepository
                .FirstOrDefaultAsync(x => x.MindfightId == mindfightId && x.TeamId == teamId);

            if (currentRegistration != null)
            {
                throw new UserFriendlyException("Registracija jau egzistuoja!");
            }

            currentRegistration = new Models.Registration(currentMindfight, currentTeam)
            {
                CreationTime = Clock.Now
            };
            return await _registrationRepository.InsertOrUpdateAndGetIdAsync(currentRegistration);
        }

        public async Task DeleteRegistration(long mindfightId, long teamId)
        {
            var currentMindfight = await _mindfightRepository
                .FirstOrDefaultAsync(x => x.Id == mindfightId);

            if (currentMindfight == null)
                throw new UserFriendlyException("Protmūšis su nurodytu id neegzistuoja!");

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
                throw new UserFriendlyException("Komanda su nurodytu id neegzistuoja!");

            if (_userManager.AbpSession.UserId != currentTeam.LeaderId)
                throw new UserFriendlyException("Jūs nesate komandos kapitonas!");

            var currentRegistration = await _registrationRepository
                .FirstOrDefaultAsync(x => x.MindfightId == mindfightId && x.TeamId == teamId);

            if (currentRegistration == null)
                throw new UserFriendlyException("Komanda nėra užsiregistravusi į protmūšį!");

            //if (currentMindfight.StartTime.AddDays(-1) < Clock.Now)
                await _registrationRepository.DeleteAsync(currentRegistration);
        }

        public async Task<List<RegistrationDto>> GetTeamRegistrations(long teamId)
        {
            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
                throw new UserFriendlyException("Komanda su nurodytu id neegzistuoja!");

            var currentMindfights = await _mindfightRepository
                .GetAllIncluding(x => x.Registrations)
                .Where(x => x.IsActive && !x.IsFinished)
                .ToListAsync();

            var currentRegistrations = await _registrationRepository
                .GetAllIncluding(x => x.Mindfight, x => x.Team)
                .Where(x => currentMindfights.Any(m => m.Id == x.MindfightId) && x.TeamId == teamId)
                .ToListAsync();

            return currentRegistrations.Select(registration => new RegistrationDto
                {
                    Id = registration.Id,
                    MindfightId = registration.MindfightId,
                    CreationTime = registration.CreationTime,
                    MindfightName = registration.Mindfight.Title,
                    MindfightStartTime = registration.Mindfight.StartTime,
                    TeamId = registration.TeamId,
                    TeamName = registration.Team.Name,
                    IsConfirmed = registration.IsConfirmed
                })
                .ToList();
        }

        public async Task<RegistrationDto> GetRegistration(long registrationId)
        {
            var currentRegistration = await _registrationRepository
                .GetAllIncluding(x => x.Mindfight, x => x.Team)
                .FirstOrDefaultAsync(x => x.Id == registrationId);

            if (currentRegistration == null) {
                throw new UserFriendlyException("Registracija su nurodytu id neegzistuoja!");
            }

            var registrationDto = new RegistrationDto();
            currentRegistration.MapTo(registrationDto);
            registrationDto.MindfightName = currentRegistration.Mindfight.Title;

            return registrationDto;
        }

        public async Task<RegistrationDto> GetMindfightTeamRegistration(long mindfightId, long teamId)
        {
            var currentMindfight = await _mindfightRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == mindfightId);

            if (currentMindfight == null)
                throw new UserFriendlyException("Protmūšis su nurodytu id neegzistuoja!");

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
                throw new UserFriendlyException("Komanda su nurodytu id neegzistuoja!");

            var currentRegistration = await _registrationRepository
                .GetAllIncluding(x => x.Mindfight, x => x.Team)
                .FirstOrDefaultAsync(x => mindfightId == x.MindfightId && teamId == x.TeamId);

            var registrationDto = new RegistrationDto();

            if (currentRegistration != null)
            {
                currentRegistration.MapTo(registrationDto);
                registrationDto.MindfightName = currentMindfight.Title;
            }
            return registrationDto;
        }

        public async Task<List<RegistrationDto>> GetMindfightRegistrations(long mindfightId)
        {
            var currentMindfight = await _mindfightRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == mindfightId);

            if (currentMindfight == null)
                throw new UserFriendlyException("Protmūšis su nurodytu id neegzistuoja!");

            var currentRegistrations = await _registrationRepository
                .GetAllIncluding(x => x.Mindfight, x => x.Team)
                .Where(x => mindfightId == x.MindfightId)
                .ToListAsync();

            return currentRegistrations.Select(registration => new RegistrationDto
                {
                    MindfightId = registration.MindfightId,
                    CreationTime = registration.CreationTime,
                    MindfightName = registration.Mindfight.Title,
                    MindfightStartTime = registration.Mindfight.StartTime,
                    TeamId = registration.TeamId,
                    TeamName = registration.Team.Name,
                    IsConfirmed = registration.IsConfirmed
                })
                .ToList();
        }

        public async Task UpdateConfirmation(long mindfightId, long teamId, bool isConfirmed)
        {

            var currentMindfight = await _mindfightRepository
                .FirstOrDefaultAsync(x => x.Id == mindfightId);

            if (currentMindfight == null)
                throw new UserFriendlyException("Protmūšis su nurodytu id neegzistuoja!");

            if (!(currentMindfight.CreatorId == _userManager.AbpSession.UserId 
                || _permissionChecker.IsGranted("ManageMindfights")))
                throw new AbpAuthorizationException("Jūs neturite teisių patvirtinti komandas!");

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
                throw new UserFriendlyException("Komanda su nurodytu id neegzistuoja!");

            var currentRegistration = await _registrationRepository
                .FirstOrDefaultAsync(x => x.MindfightId == mindfightId && x.TeamId == teamId);

            if (currentRegistration == null)
                throw new UserFriendlyException("Komanda nėra užsiregistravusi į protmūšį!");

            currentRegistration.IsConfirmed = isConfirmed;
            await _registrationRepository.UpdateAsync(currentRegistration);
        }
    }
}
