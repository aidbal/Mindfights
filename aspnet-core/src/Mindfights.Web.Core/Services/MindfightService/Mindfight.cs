using Abp.AspNetCore.Mvc.Authorization;
using Abp.Authorization;
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

namespace Mindfights.Services.MindfightService
{
    [AbpMvcAuthorize]
    public class Mindfight : IMindfightService
    {
        private readonly IRepository<Models.Mindfight, long> _mindfightRepository;
        private readonly IPermissionChecker _permissionChecker;
        private readonly UserManager _userManager;

        public Mindfight(IRepository<Models.Mindfight, long> mindfightRepository, IPermissionChecker permissionChecker, UserManager userManager)
        {
            _mindfightRepository = mindfightRepository;
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
                .FirstOrDefaultAsync(x => x.Id == mindfightId && x.IsActive);

            if (currentMindfight == null)
            {
                throw new UserFriendlyException("Mindfight with specified id does not exist!");
            }

            var user = _userManager.Users.IgnoreQueryFilters().FirstOrDefault(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
            {
                throw new UserFriendlyException("User does not exist!");
            }

            var creator = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == currentMindfight.CreatorId);
            if (creator == null)
            {
                throw new UserFriendlyException("Creator does not exist!");
            }

            var mindfight = new MindfightDto
            {
                Id = currentMindfight.Id,
                Title = currentMindfight.Title,
                Description = currentMindfight.Description,
                StartTime = currentMindfight.StartTime,
                TeamsLimit = currentMindfight.TeamsLimit,
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
                throw new AbpAuthorizationException("You are not authorized to create a mindfight!");
            }

            var user = _userManager.Users.IgnoreQueryFilters().FirstOrDefault(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
            {
                throw new UserFriendlyException("User does not exist!");
            }

            var mindfightWithSameName = await _mindfightRepository
                .FirstOrDefaultAsync(x => string.CompareOrdinal(x.Title.ToUpper(), mindfight.Title.ToUpper()) == 0);
            if (mindfightWithSameName != null)
            {
                throw new UserFriendlyException("Mindfight with the same title already exists!");
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
                throw new UserFriendlyException("Mindfight with specified id does not exist!");
            }

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
            {
                throw new UserFriendlyException("User does not exist!");
            }

            if (!(currentMindfight.CreatorId == _userManager.AbpSession.UserId ||
                  _permissionChecker.IsGranted("ManageMindfights")))
            {
                throw new AbpAuthorizationException("You are not creator of this mindfight!");
            }

            var mindfightWithSameName = await _mindfightRepository
                .FirstOrDefaultAsync(x => string.CompareOrdinal(x.Title.ToUpper(), mindfight.Title.ToUpper()) == 0 && x.Id != mindfight.Id);
            if (mindfightWithSameName != null)
            {
                throw new UserFriendlyException("Mindfight with the same title already exists!");
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
                throw new UserFriendlyException("Mindfight with specified id does not exist!");
            }

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
            {
                throw new UserFriendlyException("User does not exist!");
            }

            if (!(currentMindfight.CreatorId == _userManager.AbpSession.UserId ||
                  _permissionChecker.IsGranted("ManageMindfights")))
            {
                throw new AbpAuthorizationException("You are not creator of this mindfight!");
            }

            await _mindfightRepository.DeleteAsync(currentMindfight);
        }

        public async Task<List<MindfightDto>> GetMyCreatedMindfights()
        {
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
            {
                throw new UserFriendlyException("There was a problem getting current user!");
            }
            var mindfights = await _mindfightRepository
                .GetAllIncluding(x => x.Registrations)
                .Where(x => x.CreatorId == user.Id).ToListAsync();
            var mindfightsDto = new List<MindfightDto>();
            mindfights.MapTo(mindfightsDto);
            for (var i = 0; i < mindfights.Count; i++)
            {
                mindfightsDto[i].RegisteredTeamsCount = mindfights[i].Registrations.Count;
                mindfightsDto[i].CreatorId = user.Id;
                mindfightsDto[i].CreatorEmail = user.EmailAddress;
            }
            return mindfightsDto;
        }

        public async Task<List<MindfightDto>> GetAllowedToEvaluateMindfights()
        {
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
            {
                throw new UserFriendlyException("There was a problem getting current user!");
            }

            var mindfights = await _mindfightRepository
                .GetAllIncluding(x => x.Registrations, x => x.Evaluators)
                .Where(x => x.Evaluators.Any(y => y.UserId == user.Id) && x.CreatorId != user.Id).ToListAsync();
            var mindfightsDto = new List<MindfightDto>();
            mindfights.MapTo(mindfightsDto);
            for (var i = 0; i < mindfights.Count; i++)
            {
                var creator = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == mindfights[i].CreatorId);
                mindfightsDto[i].RegisteredTeamsCount = mindfights[i].Registrations.Count;
                mindfightsDto[i].CreatorEmail = creator.EmailAddress;
            }
            return mindfightsDto;
        }

        public async Task<List<MindfightPublicDto>> GetUpcomingMindfights()
        {
            var mindfights = await _mindfightRepository
                .GetAllIncluding(x => x.Registrations)
                .Where(x => x.IsActive && x.IsConfirmed && !x.IsFinished).ToListAsync();
            var mindfightsDto = new List<MindfightPublicDto>();
            mindfights.MapTo(mindfightsDto);
            for (var i = 0; i < mindfights.Count; i++)
            {
                var creator = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == mindfights[i].CreatorId);
                mindfightsDto[i].TeamsLimit = mindfights[i].TeamsLimit;
                mindfightsDto[i].RegisteredTeamsCount = mindfights[i].Registrations.Count;
                mindfightsDto[i].CreatorEmail = creator.EmailAddress;
            }
            return mindfightsDto;
        }

        public async Task UpdateActiveStatus(long mindfightId, bool isActive)
        {
            var currentMindfight = await _mindfightRepository
                .FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            if (!(currentMindfight.CreatorId == _userManager.AbpSession.UserId || _permissionChecker.IsGranted("ManageMindfights")))
                throw new AbpAuthorizationException("You are not creator of this mindfight!");

            currentMindfight.IsActive = isActive;
            await _mindfightRepository.UpdateAsync(currentMindfight);
        }

        public async Task UpdateConfirmedStatus(long mindfightId, bool isConfirmed)
        {
            var currentMindfight = await _mindfightRepository
                .FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            if (!(currentMindfight.CreatorId == _userManager.AbpSession.UserId || _permissionChecker.IsGranted("ManageMindfights")))
                throw new AbpAuthorizationException("You are not creator of this mindfight!");

            currentMindfight.IsConfirmed = isConfirmed;
            await _mindfightRepository.UpdateAsync(currentMindfight);
        }

        public async Task UpdateFinishedStatus(long mindfightId, bool isFinished)
        {
            var currentMindfight = await _mindfightRepository
                .FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            if (!(currentMindfight.CreatorId == _userManager.AbpSession.UserId || _permissionChecker.IsGranted("ManageMindfights")))
                throw new UserFriendlyException("You are not creator of this mindfight!");

            currentMindfight.IsFinished = isFinished;
            await _mindfightRepository.UpdateAsync(currentMindfight);
        }

        private async Task UpdateEvaluators(long mindfightId, List<string> evaluatorEmails)
        {
            var currentMindfight = await _mindfightRepository.GetAllIncluding(x => x.Evaluators)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            if (!(currentMindfight.CreatorId != _userManager.AbpSession.UserId || _permissionChecker.IsGranted("ManageMindfights")))
                throw new AbpAuthorizationException("You are not creator of this mindfight!");

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
    }
}
