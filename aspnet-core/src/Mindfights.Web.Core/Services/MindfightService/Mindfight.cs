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
                .Include(x => x.Registrations).ThenInclude(x => x.Team)
                .Include(x => x.Evaluators).ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == mindfightId && x.IsActive);

            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");
            
            var user = _userManager.Users.IgnoreQueryFilters().FirstOrDefault(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            var mindfight = new MindfightDto
            {
                Id = currentMindfight.Id,
                Title = currentMindfight.Title,
                Description = currentMindfight.Description,
                StartTime = currentMindfight.StartTime,
                EndTime = currentMindfight.EndTime,
                PlayersLimit = currentMindfight.PlayersLimit
            };

            var getPrivateInfo = currentMindfight.CreatorId == _userManager.AbpSession.UserId;
            var getPrivateInfoForEvaluator = getPrivateInfo;
            if (!getPrivateInfo)
            {
                if (user.Team != null && currentMindfight.StartTime >= Clock.Now)
                {
                    var userTeamAllowedToParticipate =
                        currentMindfight.Registrations.Any(x => x.Team == user.Team);

                    if (userTeamAllowedToParticipate)
                    {
                        getPrivateInfo = true;
                    }
                }
                if (currentMindfight.Evaluators.Any(x => x.UserId == _userManager.AbpSession.UserId))
                {
                    getPrivateInfo = true;
                    getPrivateInfoForEvaluator = true;
                }
            }
            if (getPrivateInfo)
            {
                mindfight.PlayersLimit = currentMindfight.PlayersLimit;
                mindfight.PrepareTime = currentMindfight.PrepareTime;
                mindfight.ToursCount = currentMindfight.ToursCount;
                mindfight.TotalTimeLimitInMinutes = currentMindfight.TotalTimeLimitInMinutes;
                if (getPrivateInfoForEvaluator)
                {
                    mindfight.TeamsAllowedToParticipate = new List<string>();
                    mindfight.UsersAllowedToEvaluate = new List<string>();
                    foreach (var team in currentMindfight.Registrations.Where(x => x.IsConfirmed))
                    {
                        mindfight.TeamsAllowedToParticipate.Add(team.Team.Name);
                    }
                    foreach (var allowedUser in currentMindfight.Evaluators)
                    {
                        mindfight.UsersAllowedToEvaluate.Add(allowedUser.User.EmailAddress);
                    }
                }
            }
            return mindfight;
        }

        public async Task<long> CreateMindfight(MindfightCreateUpdateDto mindfight)
        {
            if (!(_permissionChecker.IsGranted("CreateMindfights") || !_permissionChecker.IsGranted("ManageMindfights")))
                throw new AbpAuthorizationException("You are not authorized to create a mindfight!");
            
            var user = _userManager.Users.IgnoreQueryFilters().FirstOrDefault(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            var mindfightWithSameName = await _mindfightRepository.FirstOrDefaultAsync(x => x.Title == mindfight.Title);
            if (mindfightWithSameName != null)
                throw new UserFriendlyException("Mindfight with the same title already exists!");

            var newMindfight = new Models.Mindfight(user, mindfight.Title, mindfight.Description, mindfight.PlayersLimit, mindfight.StartTime, mindfight.EndTime, mindfight.PrepareTime, mindfight.TotalTimeLimitInMinutes);
            return await _mindfightRepository.InsertAndGetIdAsync(newMindfight);
        }

        public async Task UpdateMindfight(MindfightDto mindfight, long mindfightId)
        {
            var currentMindfight = await _mindfightRepository.FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            if (!(currentMindfight.CreatorId == _userManager.AbpSession.UserId || _permissionChecker.IsGranted("ManageMindfights")))
                throw new AbpAuthorizationException("You are not creator of this mindfight!");

            var mindfightWithSameName = await _mindfightRepository
                .FirstOrDefaultAsync(x => x.Title == mindfight.Title && x.Id != mindfightId);
            if (mindfightWithSameName != null)
                throw new UserFriendlyException("Mindfight with the same title already exists!");

            mindfight.MapTo(currentMindfight);
            currentMindfight.Id = mindfightId;
            currentMindfight.Evaluators = await GetEvaluatorsFromEmails(mindfight.UsersAllowedToEvaluate, currentMindfight);
            //TODO decide what to do with allowed teams
            await _mindfightRepository.UpdateAsync(currentMindfight);
        }

        public async Task DeleteMindfight(long mindfightId)
        {
            var currentMindfight = await _mindfightRepository.FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            if (!(currentMindfight.CreatorId == _userManager.AbpSession.UserId || _permissionChecker.IsGranted("ManageMindfights")))
                throw new AbpAuthorizationException("You are not creator of this mindfight!");

            await _mindfightRepository.DeleteAsync(currentMindfight);
        }

        public async Task<List<MindfightPublicDto>> GetUpcomingMindfights()
        {
            var publicMindfights = await _mindfightRepository.GetAll()
                .Where(x => x.IsActive && !x.IsDeleted && x.IsConfirmed && !x.IsFinished).ToListAsync();
            var publicMindfightsDto = new List<MindfightPublicDto>();
            publicMindfights.MapTo(publicMindfightsDto);
            return publicMindfightsDto;
        }

        public async Task UpdateEvaluators(long mindfightId, List<string> evaluatorEmails)
        {
            var currentMindfight = await _mindfightRepository.GetAllIncluding(x => x.Evaluators)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            if (!(currentMindfight.CreatorId == _userManager.AbpSession.UserId || _permissionChecker.IsGranted("ManageMindfights")))
                throw new AbpAuthorizationException("You are not creator of this mindfight!");

            var evaluators = await _userManager.Users.IgnoreQueryFilters()
                .Where(p => evaluatorEmails.Any(p2 => p2.ToUpper() == p.NormalizedEmailAddress))
                .ToListAsync();
            //Check if evaluator exist in current mindfight
            for (var i = currentMindfight.Evaluators.Count - 1; i >= 0; i--)
            {
                var currentEvaluator = currentMindfight.Evaluators.ElementAt(i);
                if (evaluators.Contains(currentEvaluator.User) || currentEvaluator.UserId == currentMindfight.CreatorId)
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

            if (currentMindfight.CreatorId != _userManager.AbpSession.UserId)
                throw new UserFriendlyException("You are not creator of this mindfight!");

            currentMindfight.IsFinished = isFinished;
            await _mindfightRepository.UpdateAsync(currentMindfight);
        }

        private async Task<List<MindfightEvaluator>> GetEvaluatorsFromEmails(IReadOnlyCollection<string> evaluatorEmails, Models.Mindfight mindfight)
        {
            var users = await _userManager.Users.IgnoreQueryFilters()
                .Where(p => evaluatorEmails.All(p2 => p2.ToUpper() == p.NormalizedEmailAddress)).ToListAsync();
            var evaluators = users.Select(user => new MindfightEvaluator(mindfight, user)).ToList();
            return evaluators;
        }
    }
}
