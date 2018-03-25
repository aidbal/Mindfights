using System;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Skautatinklis.Authorization.Users;
using Skautatinklis.DTOs;
using Skautatinklis.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using Abp.Timing;

namespace Skautatinklis.Services.MindfightService
{
    public class MindfightService : IMindfightService
    {
        private readonly IRepository<Mindfight, long> _mindfightRepository;
        private readonly IRepository<Team, long> _teamRepository;
        private readonly UserManager _userManager;

        public MindfightService(IRepository<Mindfight, long> mindfightRepository, UserManager userManager,
            IRepository<Team, long> teamRepository)
        {
            _mindfightRepository = mindfightRepository;
            _userManager = userManager;
            _teamRepository = teamRepository;
        }

        public async Task<MindfightDto> Get(long mindfightId, long userId)
        {
            //TODO check for permission (_userManager.AbpSession.UserId)
            var currentMindfight = await _mindfightRepository
                .GetAllIncluding(x => x.MindfightRegistrations)
                .Include(x => x.AllowedTeams).ThenInclude(x => x.Team)
                .Include(x => x.Evaluators).ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == mindfightId && !x.IsPrivate && x.IsActive);
            if (currentMindfight == null)
            {
                throw new UserFriendlyException("Mindfight with specified id does not exist!");
            }
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new UserFriendlyException("User does not exist!");
            }
            var mindfight = new MindfightDto
            {
                Id = currentMindfight.Id,
                Title = currentMindfight.Title,
                Description = currentMindfight.Description,
                StartTime = currentMindfight.StartTime,
                EndTime = currentMindfight.EndTime,
                PlayersLimit = currentMindfight.PlayersLimit
            };
            var getPrivateInfo = currentMindfight.CreatorId == userId;
            var getPrivateInfoForEvaluator = getPrivateInfo;
            if (!getPrivateInfo)
            {
                if (user.Team != null && currentMindfight.StartTime >= Clock.Now)
                {
                    var userTeamAllowedToParticipate =
                        currentMindfight.MindfightRegistrations.Any(x => x.Team == user.Team);

                    if (!currentMindfight.IsPrivate)
                    {
                        getPrivateInfo = true;
                    }
                    else if (userTeamAllowedToParticipate)
                    {
                        getPrivateInfo = true;
                    }
                }
                if (currentMindfight.Evaluators.Any(x => x.UserId == userId))
                {
                    getPrivateInfo = true;
                    getPrivateInfoForEvaluator = true;
                }
            }
            if (getPrivateInfo)
            {
                mindfight.PlayersLimit = currentMindfight.PlayersLimit;
                mindfight.PrepareTime = currentMindfight.PrepareTime;
                mindfight.QuestionsCount = currentMindfight.QuestionsCount;
                mindfight.TotalTimeLimitInMinutes = currentMindfight.TotalTimeLimitInMinutes;
                if (getPrivateInfoForEvaluator)
                {
                    mindfight.TeamsAllowedToParticipate = new List<string>();
                    mindfight.UsersAllowedToEvaluate = new List<string>();
                    foreach (var team in currentMindfight.AllowedTeams)
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

        public async Task<long> Create(MindfightCreateUpdateDto mindfight, long creatorId)
        {
            //TODO check for permission (_userManager.AbpSession.UserId)
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == creatorId);
            if (user == null)
            {
                throw new UserFriendlyException("User with creator id does not exist!");
            }
            var mindfightWithSameName = await _mindfightRepository.FirstOrDefaultAsync(x => x.Title == mindfight.Title);
            if (mindfightWithSameName != null)
            {
                throw new UserFriendlyException("Mindfight with the same title already exists!");
            }
            var newMindfight = new Mindfight(user, mindfight.Title, mindfight.Description, mindfight.PlayersLimit, mindfight.StartTime, mindfight.EndTime, mindfight.PrepareTime, mindfight.TotalTimeLimitInMinutes, mindfight.IsPrivate);
            return await _mindfightRepository.InsertAndGetIdAsync(newMindfight);
        }

        public async Task Update(MindfightDto mindfight, long mindfightId, long userId)
        {
            //TODO check for permission (_userManager.AbpSession.UserId)
            var currentMindfight = await _mindfightRepository.FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
            {
                throw new UserFriendlyException("Mindfight with specified id does not exist!");
            }
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new UserFriendlyException("User does not exist!");
            }
            if (currentMindfight.CreatorId != userId)
            {
                throw new UserFriendlyException("You are not creator of this mindfight!");
            }
            var mindfightWithSameName = await _mindfightRepository
                .FirstOrDefaultAsync(x => x.Title == mindfight.Title && x.Id != mindfightId);
            if (mindfightWithSameName != null)
            {
                throw new UserFriendlyException("Mindfight with the same title already exists!");
            }
            mindfight.MapTo(currentMindfight);
            currentMindfight.Id = mindfightId;
            currentMindfight.Evaluators = await GetEvaluatorsFromEmails(mindfight.UsersAllowedToEvaluate, currentMindfight);
            //TODO decide what to do with allowed teams
            await _mindfightRepository.UpdateAsync(currentMindfight);
        }

        public async Task Delete(long userId, long mindfightId)
        {
            var currentMindfight = await _mindfightRepository.FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
            {
                throw new UserFriendlyException("Mindfight with specified id does not exist!");
            }
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new UserFriendlyException("User does not exist!");
            }
            if (currentMindfight.CreatorId != userId)
            {
                throw new UserFriendlyException("You are not creator of this mindfight!");
            }
            await _mindfightRepository.DeleteAsync(currentMindfight);
        }

        public async Task<List<MindfightPublicDto>> GetUpcomingPublic()
        {
            var publicMindfights = await _mindfightRepository.GetAll()
                .Where(x => !x.IsPrivate && x.IsActive && !x.IsDeleted && x.IsConfirmed && !x.IsFinished).ToListAsync();
            var publicMindfightsDto = new List<MindfightPublicDto>();
            publicMindfights.MapTo(publicMindfightsDto);
            return publicMindfightsDto;
        }

        public async Task<List<MindfightPrivateDto>> GetUpcomingPrivateMindfights(long userId)
        {
            //TODO check for permission (_userManager.AbpSession.UserId)
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new UserFriendlyException("User with creator id does not exist!");
            }

            var privateMindfightsDto = new List<MindfightPrivateDto>();
            var privateMindfights = await _mindfightRepository.GetAllIncluding(x => x.AllowedTeams).
                Where(x => x.IsPrivate && x.IsActive && !x.IsDeleted && x.IsConfirmed && !x.IsFinished).ToListAsync();

            foreach (var mindfight in privateMindfights)
            {
                foreach (var allowed in mindfight.AllowedTeams)
                {
                    if (allowed.Team != user.Team && mindfight.CreatorId != user.Id)
                        continue;
                    var temp = new MindfightPrivateDto();
                    allowed.Mindfight.MapTo(temp);
                    privateMindfightsDto.Add(temp);
                }
            }
            return privateMindfightsDto;
        }

        public async Task UpdateEvaluators(long userId, long mindfightId, List<string> evaluatorEmails)
        {
            var currentMindfight = await _mindfightRepository.GetAllIncluding(x => x.Evaluators)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
            {
                throw new UserFriendlyException("Mindfight with specified id does not exist!");
            }
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new UserFriendlyException("User does not exist!");
            }
            if (currentMindfight.CreatorId != userId)
            {
                throw new UserFriendlyException("You are not creator of this mindfight!");
            }
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
                var mindfightEvaluators = new MindfightEvaluators(currentMindfight, evaluator);
                currentMindfight.Evaluators.Add(mindfightEvaluators);
            }
            await _mindfightRepository.UpdateAsync(currentMindfight);
        }

        public async Task UpdateAllowedTeams(long userId, long mindfightId, List<string> allowedTeamNames)
        {
            var currentMindfight = await _mindfightRepository.GetAllIncluding(x => x.AllowedTeams)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
            {
                throw new UserFriendlyException("Mindfight with specified id does not exist!");
            }
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new UserFriendlyException("User does not exist!");
            }
            if (currentMindfight.CreatorId != userId)
            {
                throw new UserFriendlyException("You are not creator of this mindfight!");
            }
            var teams = await _teamRepository.GetAll()
                .Where(team => allowedTeamNames.Any(name => string.Equals(name, team.Name, StringComparison.CurrentCultureIgnoreCase)))
                .ToListAsync();
            for (var i = currentMindfight.AllowedTeams.Count - 1; i >= 0; i--)
            {
                var currentTeam = currentMindfight.AllowedTeams.ElementAt(i);
                if (teams.Contains(currentTeam.Team))
                {
                    teams.Remove(currentTeam.Team);
                }
                else
                {
                    currentMindfight.AllowedTeams.Remove(currentTeam);
                }
            }
            //Add non-existant teams to mindfight
            foreach (var team in teams)
            {
                var allowedTeams = new MindfightAllowedTeam(currentMindfight, team);
                currentMindfight.AllowedTeams.Add(allowedTeams);
            }
            await _mindfightRepository.UpdateAsync(currentMindfight);
        }

        public async Task UpdateActiveStatus(long userId, long mindfightId, bool isActive)
        {
            //TODO check for admin or creator permission
            var currentMindfight = await _mindfightRepository.GetAllIncluding(x => x.AllowedTeams)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
            {
                throw new UserFriendlyException("Mindfight with specified id does not exist!");
            }
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new UserFriendlyException("User does not exist!");
            }
            if (currentMindfight.CreatorId != userId)
            {
                throw new UserFriendlyException("You are not creator of this mindfight!");
            }
            currentMindfight.IsActive = isActive;
            await _mindfightRepository.UpdateAsync(currentMindfight);
        }

        public async Task UpdateConfirmedStatus(long userId, long mindfightId, bool isConfirmed)
        {
            //TODO check for admin permission
            var currentMindfight = await _mindfightRepository.GetAllIncluding(x => x.AllowedTeams)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
            {
                throw new UserFriendlyException("Mindfight with specified id does not exist!");
            }
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new UserFriendlyException("User does not exist!");
            }
            if (currentMindfight.CreatorId != userId)
            {
                throw new UserFriendlyException("You are not creator of this mindfight!");
            }
            currentMindfight.IsConfirmed = isConfirmed;
            await _mindfightRepository.UpdateAsync(currentMindfight);
        }

        public async Task UpdateFinishedStatus(long userId, long mindfightId, bool isFinished)
        {
            //TODO check for admin or creator permission
            var currentMindfight = await _mindfightRepository.GetAllIncluding(x => x.AllowedTeams)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
            {
                throw new UserFriendlyException("Mindfight with specified id does not exist!");
            }
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new UserFriendlyException("User does not exist!");
            }
            if (currentMindfight.CreatorId != userId)
            {
                throw new UserFriendlyException("You are not creator of this mindfight!");
            }
            currentMindfight.IsFinished = isFinished;
            await _mindfightRepository.UpdateAsync(currentMindfight);
        }

        private async Task<List<MindfightEvaluators>> GetEvaluatorsFromEmails(IReadOnlyCollection<string> evaluatorEmails, Mindfight mindfight)
        {
            var users = await _userManager.Users.IgnoreQueryFilters()
                .Where(p => evaluatorEmails.All(p2 => p2.ToUpper() == p.NormalizedEmailAddress)).ToListAsync();
            var evaluators = users.Select(user => new MindfightEvaluators(mindfight, user)).ToList();
            return evaluators;
        }

        //private void getTeamsFromName ()
        //{
        //    var evaluators = new List<MindfightAllowedTeam>();
        //}


        // Move to mindfight result
        //public async Task<List<MindfightPublicDto>> GetPublicFinishedMindfights()
        //{
        //    var publicMindfights = await _mindfightRepository.GetAll().Where(x => !x.IsPrivate && !x.IsDeleted && x.IsConfirmed && x.IsFinished).ToListAsync();
        //    var publicMindfightsDto = new List<MindfightPublicDto>();
        //    publicMindfights.MapTo(publicMindfightsDto);
        //    return publicMindfightsDto;
        //}



        //public async Task<MindfightCreateUpdateDto> GetMindfightEdit()
        //{
        //    //TODO check for permission (_userManager.AbpSession.UserId)
        //}

        //public async Task<MindfightPrivateDto> GetMindfightPlay(long mindfightId, long userId)
        //{
        //    //TODO check for permission (_userManager.AbpSession.UserId)
        //    var currentMindfight = await _mindfightRepository.GetAllIncluding(x => x.AllowedTeams).FirstOrDefaultAsync(x => x.Id == mindfightId && x.IsPrivate && !x.IsDeleted && x.IsConfirmed);
        //    //var privateMindfights = await _mindfightRepository.GetAllIncluding(x => x.AllowedTeams).Where(x => x.IsPrivate && x.IsActive && !x.IsDeleted && x.IsConfirmed && !x.IsFinished).ToListAsync();
        //    if (currentMindfight == null)
        //    {
        //        throw new UserFriendlyException("Mindfight with specified id does not exist!");
        //    }
        //    var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
        //    if (user == null)
        //    {
        //        throw new UserFriendlyException("User does not exist!");
        //    }
        //    if (user.Team == null)
        //    {
        //        throw new UserFriendlyException("User does not have a team!");
        //    }
        //    if (currentMindfight.AllowedTeams.Any(x => x.TeamId != user.Team.Id))
        //    {
        //        throw new UserFriendlyException("User's team is not allowed to play this mindfight!");
        //    }
        //    var mindfight = new MindfightPrivateDto();
        //    currentMindfight.MapTo(mindfight);
        //    return mindfight;
        //}
    }
}
