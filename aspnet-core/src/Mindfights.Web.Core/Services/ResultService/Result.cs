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
    public class Result : IResultService
    {
        private readonly IRepository<Mindfight, long> _mindfightRepository;
        private readonly IRepository<Team, long> _teamRepository;
        private readonly IRepository<MindfightResult, long> _resultRepository;
        private readonly IRepository<Question, long> _questionRepository;
        private readonly IRepository<Tour, long> _tourRepository;
        private readonly IRepository<TeamAnswer, long> _teamAnswerRepository;
        private readonly IRepository<Registration, long> _registrationRepository;
        private readonly IPermissionChecker _permissionChecker;
        private readonly UserManager _userManager;

        public Result(
            IRepository<Mindfight, long> mindfightRepository,
            IRepository<Team, long> teamRepository,
            IRepository<MindfightResult, long> resultRepository,
            IRepository<Question, long> questionRepository,
            IRepository<Tour, long> tourRepository,
            IRepository<TeamAnswer, long> teamAnswerRepository,
            IRepository<Registration, long> registrationRepository,
            IPermissionChecker permissionChecker,
            UserManager userManager)
        {
            _mindfightRepository = mindfightRepository;
            _teamRepository = teamRepository;
            _resultRepository = resultRepository;
            _questionRepository = questionRepository;
            _tourRepository = tourRepository;
            _teamAnswerRepository = teamAnswerRepository;
            _registrationRepository = registrationRepository;
            _permissionChecker = permissionChecker;
            _userManager = userManager;
        }

        public async Task UpdateResult(long mindfightId, long teamId)
        {
            var currentMindfight = await _mindfightRepository
                .GetAll()
                .Include(x => x.Registrations)
                .ThenInclude(x => x.Team)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);

            if (currentMindfight == null)
            {
                throw new UserFriendlyException("Mindfight with specified id does not exist!");
            }

            if (!(currentMindfight.CreatorId == _userManager.AbpSession.UserId
                  || _permissionChecker.IsGranted("ManageMindfights")))
            {
                throw new AbpAuthorizationException("Insufficient permissions to create result!");
            }

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
            {
                throw new UserFriendlyException("Team with specified id does not exist!");
            }

            if (!currentMindfight.Registrations.Any(x => x.TeamId == teamId && x.IsConfirmed))
            {
                throw new UserFriendlyException("Team is not allowed to play this mindfight!");
            }

            var teamRegistration = _registrationRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.TeamId == teamId && x.MindfightId == mindfightId);

            if (teamRegistration == null)
            {
                throw new UserFriendlyException("Team was not registered to play!");
            }

            var teamResult = await _resultRepository
                .GetAllIncluding(x => x.Team)
                .FirstOrDefaultAsync(x => x.MindfightId == mindfightId && x.TeamId == teamId);

            if (teamResult == null)
            {
                throw new UserFriendlyException("Team has not yet finished mindfight!");
            }

            var mindfightTours = await _tourRepository
                .GetAll()
                .Where(tour => tour.MindfightId == currentMindfight.Id)
                .ToListAsync();

            if (mindfightTours.Count <= 0)
            {
                throw new UserFriendlyException("Protmūšis neturi nei vieno turo!");
            }

            var questions = await _questionRepository
                .GetAll()
                .Where(question => mindfightTours.Any(tour => tour.Id == question.TourId))
                .OrderByDescending(x => x.OrderNumber)
                .ToListAsync();

            if (questions.Count <= 0)
            {
                throw new UserFriendlyException("Protmūšis neturi nei vieno klausimo!");
            }

            var teamAnswers = await _teamAnswerRepository
                .GetAll()
                .Where(x => questions.Any(y => y.Id == x.QuestionId))
                .ToListAsync();
            
            if (questions.Count != teamAnswers.Count)
            {
                throw new UserFriendlyException("Team has not yet finished mindfight!");
            }

            if (teamAnswers.Any(x => !x.IsEvaluated))
            {
                throw new UserFriendlyException("Not all team answers has been evaluated!");
            }

            var earnedPoints = teamAnswers.Sum(teamAnswer => teamAnswer.EarnedPoints);

            var teamPlayers = await _userManager.Users
                .IgnoreQueryFilters()
                .Include(x => x.Team)
                .Where(u => u.TeamId == teamId)
                .ToListAsync();

            var currentResult = await _resultRepository
                .FirstOrDefaultAsync(result => result.TeamId == currentTeam.Id && result.MindfightId == currentMindfight.Id);

            if (currentResult != null)
            {
                currentResult.EarnedPoints = earnedPoints;
                currentResult.Team = currentTeam;
                currentResult.Mindfight = currentMindfight;
                currentResult.IsEvaluated = true;
            }
            else
            {
                currentResult = new MindfightResult(earnedPoints, true, currentTeam, currentMindfight);
            }
            
            await _resultRepository.InsertOrUpdateAsync(currentResult);

            await UpdateMindfightPlaces(mindfightId);

            foreach (var player in teamPlayers)
            {
                var userMindfightResult = new UserMindfightResult(player, currentResult);
                player.MindfightResults.Add(userMindfightResult);
                player.Points += currentResult.EarnedPoints;
            }
        }

        public async Task<MindfightResultDto> GetMindfightTeamResult(long mindfightId, long teamId)
        {
            var currentMindfight = await _mindfightRepository
                .GetAllIncluding(mindfight => mindfight.Tours)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);

            if (currentMindfight == null)
            {
                throw new UserFriendlyException("Mindfight with specified id does not exist!");
            }

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
            {
                throw new UserFriendlyException("Team with specified id does not exist!");
            }

            var currentResult = await _resultRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.MindfightId == mindfightId && x.TeamId == teamId);

            if (currentResult == null)
            {
                throw new UserFriendlyException("Mindfight result for this team does not exist!");
            }

            var mindfightQuestions = await _questionRepository
                .GetAllIncluding(question => question.Tour)
                .Where(question => question.Tour.MindfightId == mindfightId)
                .ToListAsync();

            var totalPoints = mindfightQuestions.Sum(question => question.Points);

            var resultDto = new MindfightResultDto();
            currentResult.MapTo(resultDto);
            resultDto.MindfightStartTime = currentMindfight.StartTime;
            resultDto.MindfightName = currentMindfight.Title;
            resultDto.MindfightId = mindfightId;
            resultDto.TeamId = teamId;
            resultDto.ToursCount = currentMindfight.Tours.Count;
            resultDto.TeamName = currentTeam.Name;
            resultDto.TotalPoints = totalPoints;
            resultDto.IsMindfightFinished = currentMindfight.IsFinished;
            return resultDto;
        }

        public async Task<List<MindfightResultDto>> GetMindfightResults(long mindfightId)
        {
            var currentMindfight = await _mindfightRepository
                .GetAllIncluding(mindfight => mindfight.Tours)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);

            if (currentMindfight == null)
            {
                throw new UserFriendlyException("Mindfight with specified id does not exist!");
            }

            var mindfightQuestions = await _questionRepository
                .GetAllIncluding(question => question.Tour)
                .Where(question => question.Tour.MindfightId == mindfightId)
                .ToListAsync();

            var totalPoints = mindfightQuestions.Sum(question => question.Points);

            var registeredTeamResults = await _resultRepository
                .GetAllIncluding(x => x.Team)
                .Where(x => x.MindfightId == mindfightId)
                .ToListAsync();

            var teamResultsDto = new List<MindfightResultDto>();

            foreach (var teamResult in registeredTeamResults)
            {
                var resultDto = new MindfightResultDto();
                teamResult.MapTo(resultDto);
                resultDto.MindfightStartTime = currentMindfight.StartTime;
                resultDto.MindfightName = currentMindfight.Title;
                resultDto.MindfightId = mindfightId;
                resultDto.TeamId = teamResult.TeamId;
                resultDto.ToursCount = currentMindfight.Tours.Count;
                resultDto.TeamName = teamResult.Team.Name;
                resultDto.TotalPoints = totalPoints;
                resultDto.IsMindfightFinished = currentMindfight.IsFinished;
                teamResultsDto.Add(resultDto);
            }

            teamResultsDto.Sort((x, y) => x.Place.CompareTo(y.Place));

            return teamResultsDto;
        }

        public async Task<List<MindfightResultDto>> GetTeamResults(long teamId)
        {
            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
            {
                throw new UserFriendlyException("Team with specified id does not exist!");
            }

            var registeredTeamResults = await _resultRepository
                .GetAllIncluding(result => result.Team, result => result.Mindfight)
                .OrderByDescending(result => result.Mindfight.StartTime)
                .Where(x => x.TeamId == teamId)
                .ToListAsync();

            var teamResultsDto = new List<MindfightResultDto>();
            
            foreach (var teamResult in registeredTeamResults)
            {
                var currentMindfight = await _mindfightRepository
                    .GetAllIncluding(mindfight => mindfight.Tours)
                    .FirstOrDefaultAsync(x => x.Id == teamResult.MindfightId);

                if (currentMindfight == null) continue;

                var mindfightQuestions = await _questionRepository
                    .GetAllIncluding(question => question.Tour)
                    .Where(question => question.Tour.MindfightId == currentMindfight.Id)
                    .ToListAsync();

                var totalPoints = mindfightQuestions.Sum(question => question.Points);

                var resultDto = new MindfightResultDto();
                teamResult.MapTo(resultDto);
                resultDto.MindfightStartTime = currentMindfight.StartTime;
                resultDto.MindfightName = currentMindfight.Title;
                resultDto.MindfightId = currentMindfight.Id;
                resultDto.TeamId = teamResult.TeamId;
                resultDto.ToursCount = currentMindfight.Tours.Count;
                resultDto.TeamName = teamResult.Team.Name;
                resultDto.TotalPoints = totalPoints;
                resultDto.IsMindfightFinished = currentMindfight.IsFinished;
                teamResultsDto.Add(resultDto);
            }
            return teamResultsDto;
        }

        public async Task<List<MindfightResultDto>> GetUserResults(long userId)
        {
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new UserFriendlyException("User does not exist!");
            }

            var mindfightResults = await _resultRepository
                .GetAllIncluding(result => result.Team, result => result.Mindfight)
                .OrderByDescending(result => result.Mindfight.StartTime)
                .Where(x => x.Users.Any(player => player.UserId == userId))
                .ToListAsync();

            var resultsDto = new List<MindfightResultDto>();

            foreach (var result in mindfightResults)
            {
                var currentMindfight = await _mindfightRepository
                    .GetAllIncluding(mindfight => mindfight.Tours)
                    .FirstOrDefaultAsync(x => x.Id == result.MindfightId);

                if (currentMindfight == null) continue;

                var mindfightQuestions = await _questionRepository
                    .GetAllIncluding(question => question.Tour)
                    .Where(question => question.Tour.MindfightId == currentMindfight.Id)
                    .ToListAsync();

                var totalPoints = mindfightQuestions.Sum(question => question.Points);

                var resultDto = new MindfightResultDto();
                result.MapTo(resultDto);
                resultDto.MindfightStartTime = currentMindfight.StartTime;
                resultDto.MindfightName = currentMindfight.Title;
                resultDto.MindfightId = currentMindfight.Id;
                resultDto.TeamId = result.TeamId;
                resultDto.ToursCount = currentMindfight.Tours.Count;
                resultDto.TeamName = result.Team.Name;
                resultDto.TotalPoints = totalPoints;
                resultDto.IsMindfightFinished = currentMindfight.IsFinished;
                resultsDto.Add(resultDto);
            }
            return resultsDto;
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
