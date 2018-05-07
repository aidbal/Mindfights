using Abp.AspNetCore.Mvc.Authorization;
using Abp.Authorization;
using Abp.AutoMapper;
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

namespace Mindfights.Services.MindfightService
{
    [AbpMvcAuthorize]
    public class Mindfight : IMindfightService
    {
        private readonly IRepository<Models.Mindfight, long> _mindfightRepository;
        private readonly IRepository<MindfightResult, long> _resultRepository;
        private readonly IRepository<Team, long> _teamRepository;
        private readonly IPermissionChecker _permissionChecker;
        private readonly UserManager _userManager;

        public Mindfight(
            IRepository<Models.Mindfight, long> mindfightRepository,
            IRepository<MindfightResult, long> resultRepository,
            IRepository<Team, long> teamRepository,
            IPermissionChecker permissionChecker,
            UserManager userManager
            )
        {
            _mindfightRepository = mindfightRepository;
            _resultRepository = resultRepository;
            _teamRepository = teamRepository;
            _permissionChecker = permissionChecker;
            _userManager = userManager;
        }

        public async Task<MindfightDto> GetMindfight(long mindfightId)
        {
            var currentMindfight = await _mindfightRepository
                .GetAll()
                .Include(x => x.Tours)
                .Include(x => x.Registrations).ThenInclude(x => x.Team)
                .Include(x => x.Evaluators).ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);

            if (currentMindfight == null)
            {
                throw new UserFriendlyException("Protmūšis su tokiu id neegzistuoja!");
            }

            var user = _userManager.Users.IgnoreQueryFilters().FirstOrDefault(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
            {
                throw new UserFriendlyException("Vartotojas neegzistuoja!");
            }

            var creator = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == currentMindfight.CreatorId);
            if (creator == null)
            {
                throw new UserFriendlyException("Protmūšio kūrėjas neegzistoja!");
            }

            var mindfight = new MindfightDto
            {
                Id = currentMindfight.Id,
                Title = currentMindfight.Title,
                Description = currentMindfight.Description,
                StartTime = currentMindfight.StartTime,
                TeamsLimit = currentMindfight.TeamsLimit,
                IsActive = currentMindfight.IsActive,
                IsFinished = currentMindfight.IsFinished,
                CreatorId = creator.Id,
                CreatorEmail = creator.EmailAddress,
                PrepareTime = currentMindfight.PrepareTime,
                ToursCount = currentMindfight.Tours.Count,
                RegisteredTeamsCount = currentMindfight.Registrations.Count,
                TeamsAllowedToParticipate = new List<string>(),
                UsersAllowedToEvaluate = new List<string>()
            };
            foreach (var team in currentMindfight.Registrations.Where(x => x.IsConfirmed))
            {
                mindfight.TeamsAllowedToParticipate.Add(team.Team.Name);
            }
            foreach (var allowedUser in currentMindfight.Evaluators)
            {
                mindfight.UsersAllowedToEvaluate.Add(allowedUser.User.EmailAddress);
            }
            return mindfight;
        }

        public async Task<long> CreateMindfight(MindfightCreateUpdateDto mindfight)
        {
            if (!(_permissionChecker.IsGranted("CreateMindfights") ||
                  !_permissionChecker.IsGranted("ManageMindfights")))
            {
                throw new AbpAuthorizationException("Jūs neturite teisės sukurti naują protmūšį!");
            }

            var user = _userManager.Users.IgnoreQueryFilters().FirstOrDefault(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
            {
                throw new UserFriendlyException("Vartotojas neegzistuoja!");
            }

            var newMindfight = new Models.Mindfight(user, mindfight.Title, mindfight.Description, mindfight.TeamsLimit,
                mindfight.StartTime, mindfight.PrepareTime);
            return await _mindfightRepository.InsertAndGetIdAsync(newMindfight);
        }

        public async Task UpdateMindfight(MindfightDto mindfight)
        {
            var currentMindfight = await _mindfightRepository.FirstOrDefaultAsync(x => x.Id == mindfight.Id);
            if (currentMindfight == null)
            {
                throw new UserFriendlyException("Protmūšis su nurodytu id neegzistuoja!");
            }

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
            {
                throw new UserFriendlyException("Vartotojas neegzsituoja!");
            }

            if (!(currentMindfight.CreatorId == _userManager.AbpSession.UserId ||
                  _permissionChecker.IsGranted("ManageMindfights")))
            {
                throw new AbpAuthorizationException("Jūs neturite teisės redaguoti šį protmūšį!");
            }

            currentMindfight.Title = mindfight.Title;
            currentMindfight.Description = mindfight.Description;
            currentMindfight.StartTime = mindfight.StartTime;
            currentMindfight.PrepareTime = mindfight.PrepareTime;
            currentMindfight.TeamsLimit = mindfight.TeamsLimit;
            await UpdateEvaluators(currentMindfight.Id, mindfight.UsersAllowedToEvaluate);
            await _mindfightRepository.UpdateAsync(currentMindfight);
        }

        public async Task DeleteMindfight(long mindfightId)
        {
            var currentMindfight = await _mindfightRepository.FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
            {
                throw new UserFriendlyException("Protmūšis su nurodytu id neegzistuoja!");
            }

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
            {
                throw new UserFriendlyException("Vartotojas neegzistuoja!");
            }

            if (!(currentMindfight.CreatorId == _userManager.AbpSession.UserId ||
                  _permissionChecker.IsGranted("ManageMindfights")))
            {
                throw new AbpAuthorizationException("Jūs neturite teisės ištrinti šio protmūšio!");
            }

            await _mindfightRepository.DeleteAsync(currentMindfight);
        }

        public async Task<List<MindfightDto>> GetMyCreatedMindfights()
        {
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
            {
                throw new UserFriendlyException("Vartotojas neegzistuoja!");
            }

            var isMindfightsManager = _permissionChecker.IsGranted("ManageMindfights");

            var mindfights = await _mindfightRepository
                .GetAllIncluding(mindfight => mindfight.Registrations, mindfight => mindfight.Creator)
                .OrderBy(mindfight => mindfight.IsFinished)
                .ThenBy(mindfight => mindfight.StartTime)
                .Where(x => isMindfightsManager || x.CreatorId == user.Id).ToListAsync();

            var mindfightsDto = new List<MindfightDto>();
            mindfights.MapTo(mindfightsDto);
            for (var i = 0; i < mindfights.Count; i++)
            {
                mindfightsDto[i].RegisteredTeamsCount = mindfights[i].Registrations.Count;
                mindfightsDto[i].CreatorId = mindfights[i].CreatorId;
                mindfightsDto[i].CreatorEmail = mindfights[i].Creator.EmailAddress;
            }
            return mindfightsDto;
        }

        public async Task<List<MindfightDto>> GetAllowedToEvaluateMindfights()
        {
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
            {
                throw new UserFriendlyException("Vartotojas neegzistuoja!");
            }

            var mindfights = await _mindfightRepository
                .GetAllIncluding(mindfight => mindfight.Registrations, mindfight => mindfight.Evaluators, mindfight => mindfight.Creator)
                .OrderBy(mindfight => mindfight.StartTime)
                .Where(x => x.Evaluators.Any(y => y.UserId == user.Id) && x.CreatorId != user.Id && !x.IsFinished).ToListAsync();

            var mindfightsDto = new List<MindfightDto>();
            mindfights.MapTo(mindfightsDto);
            for (var i = 0; i < mindfights.Count; i++)
            {
                mindfightsDto[i].RegisteredTeamsCount = mindfights[i].Registrations.Count;
                mindfightsDto[i].CreatorEmail = mindfights[i].Creator.EmailAddress;
            }
            return mindfightsDto;
        }

        public async Task<List<MindfightDto>> GetRegisteredMindfights()
        {
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
            {
                throw new UserFriendlyException("Vartotojas neegzistuoja!");
            }

            var mindfights = await _mindfightRepository
                .GetAllIncluding(mindfight => mindfight.Registrations, mindfight => mindfight.Creator)
                .OrderBy(mindfight => mindfight.StartTime)
                .Where(mindfight =>
                    mindfight.Registrations.Any(registration => registration.TeamId == user.TeamId)
                    && mindfight.IsConfirmed
                    && !mindfight.IsFinished
                    && mindfight.StartTime > Clock.Now.AddHours(-1)
                ).ToListAsync();

            var mindfightsDto = new List<MindfightDto>();
            mindfights.MapTo(mindfightsDto);
            for (var i = 0; i < mindfights.Count; i++)
            {
                mindfightsDto[i].RegisteredTeamsCount = mindfights[i].Registrations.Count;
                mindfightsDto[i].CreatorEmail = mindfights[i].Creator.EmailAddress;
            }
            return mindfightsDto;
        }

        public async Task<List<MindfightPublicDto>> GetUpcomingMindfights()
        {
            var mindfights = await _mindfightRepository
                .GetAllIncluding(mindfight => mindfight.Registrations, mindfight => mindfight.Creator)
                .OrderBy(mindfight => mindfight.StartTime)
                .Where(
                    mindfight =>
                        mindfight.IsActive
                        && mindfight.IsConfirmed
                        && !mindfight.IsFinished
                        && mindfight.StartTime > Clock.Now.AddHours(-1))
                .ToListAsync();

            var mindfightsDto = new List<MindfightPublicDto>();
            mindfights.MapTo(mindfightsDto);
            for (var i = 0; i < mindfights.Count; i++)
            {
                mindfightsDto[i].TeamsLimit = mindfights[i].TeamsLimit;
                mindfightsDto[i].RegisteredTeamsCount = mindfights[i].Registrations.Count;
                mindfightsDto[i].CreatorEmail = mindfights[i].Creator.EmailAddress;
            }
            return mindfightsDto;
        }

        public async Task UpdateActiveStatus(long mindfightId, bool isActive)
        {
            var currentMindfight = await _mindfightRepository
                .FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
                throw new UserFriendlyException("Protmūšis su nurodytu id neegzistuoja!");

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("Vartotojas neegzistuoja!");

            if (!(currentMindfight.CreatorId == _userManager.AbpSession.UserId || _permissionChecker.IsGranted("ManageMindfights")))
                throw new AbpAuthorizationException("Jūs neturite teisės redaguoti šio protmūšio!");

            currentMindfight.IsActive = isActive;
            await _mindfightRepository.UpdateAsync(currentMindfight);
        }

        public async Task UpdateConfirmedStatus(long mindfightId, bool isConfirmed)
        {
            var currentMindfight = await _mindfightRepository
                .FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
                throw new UserFriendlyException("Protmūšis su nurodytu id neegzistuoja!");

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("Vartotojas neegzistuoja!");

            if (!(currentMindfight.CreatorId == _userManager.AbpSession.UserId || _permissionChecker.IsGranted("ManageMindfights")))
                throw new AbpAuthorizationException("Jūs neturite teisės redaguoti šio protmūšio!");

            currentMindfight.IsConfirmed = isConfirmed;
            await _mindfightRepository.UpdateAsync(currentMindfight);
        }

        public async Task UpdateFinishedStatus(long mindfightId, bool isFinished)
        {
            var currentMindfight = await _mindfightRepository
                .GetAllIncluding(mindfight => mindfight.Evaluators)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
                throw new UserFriendlyException("Protmūšis su nurodytu id neegzistuoja!");

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("Vartotojas neegzistuoja!");

            if (!(currentMindfight.CreatorId == _userManager.AbpSession.UserId
                || _permissionChecker.IsGranted("ManageMindfights")
                || currentMindfight.Evaluators.Any(y => y.UserId == user.Id)))
                throw new UserFriendlyException("Jūs neturite teisės redaguoti šio protmūšio!");

            currentMindfight.IsFinished = isFinished;

            var mindfightResults = await _resultRepository
                .GetAll()
                .Where(result => result.MindfightId == currentMindfight.Id && result.IsEvaluated)
                .ToListAsync();

            foreach (var mindfightResult in mindfightResults)
            {
                var resultPlayers = await _userManager.Users
                    .Where(x => x.MindfightResults.Any(
                        result => result.MindfightResultId == mindfightResult.Id && result.MindfightResult.IsEvaluated))
                    .ToListAsync();

                foreach (var player in resultPlayers)
                {
                    player.Points += mindfightResult.EarnedPoints;
                }
            }

            await UpdateMindfightPlaces(mindfightId);

            await _mindfightRepository.UpdateAsync(currentMindfight);
        }

        private async Task UpdateEvaluators(long mindfightId, List<string> evaluatorEmails)
        {
            var currentMindfight = await _mindfightRepository.GetAllIncluding(x => x.Evaluators)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
                throw new UserFriendlyException("Protmūšis su nurodytu id neegzistuoja!");

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("Vartotojas neegizstuoja!");

            if (!(currentMindfight.CreatorId == _userManager.AbpSession.UserId || _permissionChecker.IsGranted("ManageMindfights")))
                throw new AbpAuthorizationException("Jūs neturite teisės redaguoti šio protmūšio!");

            var evaluators = await _userManager.Users.IgnoreQueryFilters()
                .Where(p => evaluatorEmails.Any(p2 => p2.ToUpper() == p.NormalizedEmailAddress))
                .ToListAsync();
            //Check if evaluator exist in current mindfight
            for (var i = currentMindfight.Evaluators.Count - 1; i >= 0; i--)
            {
                var currentEvaluator = currentMindfight.Evaluators.ElementAt(i);
                if (evaluators.Contains(currentEvaluator.User))
                {
                    evaluators.Remove(currentEvaluator.User);
                }
                else
                {
                    currentMindfight.Evaluators.Remove(currentEvaluator);
                }
            }
            //Add non-existant evaluators to mindfight
            foreach (var evaluator in evaluators)
            {
                var mindfightEvaluators = new MindfightEvaluator(currentMindfight, evaluator);
                currentMindfight.Evaluators.Add(mindfightEvaluators);
            }
            await _mindfightRepository.UpdateAsync(currentMindfight);
        }

        private async Task UpdateMindfightPlaces(long mindfightId)
        {
            var results = await _resultRepository
                .GetAll()
                .Where(x => x.MindfightId == mindfightId)
                .OrderByDescending(x => x.EarnedPoints)
                .ToListAsync();

            if (results.Count > 0)
            {
                var currentPlacePoints = results[0].EarnedPoints;
                var currentPlace = 1;

                foreach (var result in results)
                {
                    if (result.EarnedPoints == currentPlacePoints)
                    {
                        result.Place = currentPlace;
                    }
                    else
                    {
                        currentPlace++;
                        result.Place = currentPlace;
                        currentPlacePoints = result.EarnedPoints;
                    }
                }
            }
        }
    }
}
