using Abp.AspNetCore.Mvc.Authorization;
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
using Abp.Authorization;

namespace Mindfights.Services.ResultService
{
    [AbpMvcAuthorize]
    public class ResultService : IResultService
    {
        private readonly IRepository<Mindfight, long> _mindfightRepository;
        private readonly IRepository<Team, long> _teamRepository;
        private readonly IRepository<MindfightResult, long> _resultRepository;
        private readonly IRepository<Question, long> _questionRepository;
        private readonly IRepository<TeamAnswer, long> _teamAnswerRepository;
        private readonly IRepository<Registration, long> _registrationRepository;
        private readonly IPermissionChecker _permissionChecker;
        private readonly UserManager _userManager;

        public ResultService(
            IRepository<Mindfight, long> mindfightRepository,
            IRepository<Team, long> teamRepository,
            IRepository<MindfightResult, long> resultRepository,
            IRepository<Question, long> questionRepository,
            IRepository<TeamAnswer, long> teamAnswerRepository,
            IRepository<Registration, long> registrationRepository,
            IPermissionChecker permissionChecker,
            UserManager userManager)
        {
            _mindfightRepository = mindfightRepository;
            _teamRepository = teamRepository;
            _resultRepository = resultRepository;
            _questionRepository = questionRepository;
            _teamAnswerRepository = teamAnswerRepository;
            _registrationRepository = registrationRepository;
            _permissionChecker = permissionChecker;
            _userManager = userManager;
        }

        public async Task CreateResult(long mindfightId, long tourId, long teamId)
        {
            var currentMindfight = await _mindfightRepository
                .GetAll()
                .Include(x => x.Registrations)
                .ThenInclude(x => x.Team)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);

            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            if (!(currentMindfight.CreatorId == _userManager.AbpSession.UserId
                || _permissionChecker.IsGranted("ManageMindfights")))
                throw new AbpAuthorizationException("Insufficient permissions to create result!");

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
                throw new UserFriendlyException("Team with specified id does not exist!");

            if (currentMindfight.Registrations.Any(x => x.TeamId == teamId && x.IsConfirmed))
                throw new UserFriendlyException("Team is not allowed to play this mindfight!");

            var teamRegistration = _registrationRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.TeamId == teamId && x.MindfightId == mindfightId);

            if (teamRegistration == null)
                throw new UserFriendlyException("Team was not registered to play!");

            var teamResult = await _resultRepository
                .GetAllIncluding(x => x.Team)
                .FirstOrDefaultAsync(x => x.MindfightId == mindfightId && x.TeamId == teamId);

            if (teamResult != null)
                throw new UserFriendlyException("Result was already created for this team!");

            var questions = await _questionRepository
                .GetAll()
                .Where(x => x.TourId == tourId)
                .OrderByDescending(x => x.OrderNumber)
                .ToListAsync();

            var teamAnswers = await _teamAnswerRepository
                .GetAll()
                .Where(x => questions.Any(y => y.Id == x.QuestionId))
                .ToListAsync();

            if (questions.Count != teamAnswers.Count)
                throw new UserFriendlyException("Team has not yet finished mindfight!");

            if (teamAnswers.Any(x => !x.IsEvaluated))
                throw new UserFriendlyException("Not all team answers has been evaluated!");

            var earnedPoints = teamAnswers.Sum(teamAnswer => teamAnswer.EarnedPoints);

            var teamMembers = await _userManager.Users
                .IgnoreQueryFilters()
                .Include(x => x.Team)
                .Where(u => u.TeamId == teamId)
                .ToListAsync();

            var mindfightResult = new MindfightResult(earnedPoints, true, currentTeam, currentMindfight);
            _resultRepository.InsertOrUpdate(mindfightResult);

            await UpdateMindfightPlaces(mindfightId);

            foreach (var member in teamMembers)
            {
                var userMindfightResult = new UserMindfightResult(member, mindfightResult);
                member.MindfightResults.Add(userMindfightResult);
            }
        }

        public async Task<MindfightResultDto> GetMindfightTeamResult(long mindfightId, long teamId)
        {
            var currentMindfight = await _mindfightRepository
                .FirstOrDefaultAsync(x => x.Id == mindfightId);

            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
                throw new UserFriendlyException("Team with specified id does not exist!");

            var currentResult = await _resultRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.MindfightId == mindfightId && x.TeamId == teamId);

            if (currentResult == null)
                throw new UserFriendlyException("Mindfight result for this team does not exist!");

            var resultDto = new MindfightResultDto();
            currentResult.MapTo(resultDto);
            resultDto.MindfightEndTime = currentMindfight.EndTime;
            resultDto.MindfightStartTime = currentMindfight.StartTime;
            resultDto.MindfightName = currentMindfight.Title;
            resultDto.MindfightId = mindfightId;
            resultDto.TeamId = teamId;
            resultDto.ToursCount = currentMindfight.ToursCount;
            resultDto.TeamName = currentTeam.Name;
            resultDto.TotalPoints = currentMindfight.TotalPoints;

            return resultDto;
        }

        public async Task<List<MindfightResultDto>> GetMindfightResults(long mindfightId)
        {
            var currentMindfight = await _mindfightRepository
                .FirstOrDefaultAsync(x => x.Id == mindfightId);

            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            var registeredTeams = await _registrationRepository
                .GetAll()
                .Where(x => x.MindfightId == mindfightId)
                .ToListAsync();

            var registeredTeamResults = await _resultRepository
                .GetAllIncluding(x => x.Team)
                .Where(x => x.MindfightId == mindfightId && registeredTeams.Any(y => y.TeamId == x.TeamId))
                .ToListAsync();

            var teamResultsDto = new List<MindfightResultDto>();

            foreach (var teamResult in registeredTeamResults)
            {
                var resultDto = new MindfightResultDto();
                teamResult.MapTo(resultDto);
                resultDto.MindfightEndTime = currentMindfight.EndTime;
                resultDto.MindfightStartTime = currentMindfight.StartTime;
                resultDto.MindfightName = currentMindfight.Title;
                resultDto.MindfightId = mindfightId;
                resultDto.TeamId = teamResult.TeamId;
                resultDto.ToursCount = currentMindfight.ToursCount;
                resultDto.TeamName = teamResult.Team.Name;
                resultDto.TotalPoints = currentMindfight.TotalPoints;
                teamResultsDto.Add(resultDto);
            }

            return teamResultsDto;
        }

        public async Task<List<MindfightResultDto>> GetTeamResults(long teamId)
        {
            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
                throw new UserFriendlyException("Team with specified id does not exist!");

            var registeredTeamResults = await _resultRepository
                .GetAllIncluding(x => x.Team)
                .Where(x => x.TeamId == teamId)
                .ToListAsync();

            var teamResultsDto = new List<MindfightResultDto>();
            
            foreach (var teamResult in registeredTeamResults)
            {
                var currentMindfight = await _mindfightRepository
                    .GetAll()
                    .FirstOrDefaultAsync(x => x.Id == teamResult.MindfightId);

                if (currentMindfight == null) continue;
                var resultDto = new MindfightResultDto();
                teamResult.MapTo(resultDto);
                resultDto.MindfightEndTime = currentMindfight.EndTime;
                resultDto.MindfightStartTime = currentMindfight.StartTime;
                resultDto.MindfightName = currentMindfight.Title;
                resultDto.MindfightId = currentMindfight.Id;
                resultDto.TeamId = teamResult.TeamId;
                resultDto.ToursCount = currentMindfight.ToursCount;
                resultDto.TeamName = teamResult.Team.Name;
                resultDto.TotalPoints = currentMindfight.TotalPoints;
                teamResultsDto.Add(resultDto);
            }
            return teamResultsDto;
        }

        public async Task<List<LeaderBoardDto>> GetMonthlyLeaderBoard()
        {
            var leaderBoardDtos = new List<LeaderBoardDto>();
            var mindfightResults = await _resultRepository
                .GetAllIncluding(x => x.Team)
                .OrderByDescending(x => x.EarnedPoints)
                .Where(x => x.Mindfight.StartTime > Clock.Now.AddMonths(-1) && x.Mindfight.StartTime < Clock.Now)
                .ToListAsync();

            foreach (var teamResult in mindfightResults)
            {
                var leaderBoardDto = leaderBoardDtos.FirstOrDefault(x => x.TeamId == teamResult.TeamId);
                if (leaderBoardDto == null)
                {
                    leaderBoardDto = new LeaderBoardDto
                    {
                        TeamId = teamResult.TeamId,
                        TeamName = teamResult.Team.Name,
                        EarnedPoints = teamResult.EarnedPoints,
                        PlayedMindfightsCount = 1,
                        WonMindfightsCount = mindfightResults.Where(x => x.TeamId == teamResult.TeamId)
                            .Count(x => x.Place == 1)
                    };
                    leaderBoardDtos.Add(leaderBoardDto);
                }
                else
                {
                    leaderBoardDto.EarnedPoints += teamResult.EarnedPoints;
                    leaderBoardDto.PlayedMindfightsCount += 1;
                }
            }
            return leaderBoardDtos.OrderByDescending(x => x.EarnedPoints).Take(10).ToList();
        }

        private async Task UpdateMindfightPlaces(long mindfightId)
        {
            var results = await _resultRepository
                .GetAll()
                .Where(x => x.MindfightId == mindfightId)
                .OrderBy(x => x.EarnedPoints)
                .ToListAsync();
            
            const int currentPlacePoints = -1;

            for (var i = 0; i < results.Count; i++)
            {
                if (results[i].EarnedPoints >= currentPlacePoints)
                    results[i].Place = i + 1;
            }
        }
    }
}
