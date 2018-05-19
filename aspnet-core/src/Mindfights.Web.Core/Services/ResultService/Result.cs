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
using Abp.ObjectMapping;

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
        private readonly IObjectMapper _objectMapper;

        public Result(
            IRepository<Mindfight, long> mindfightRepository,
            IRepository<Team, long> teamRepository,
            IRepository<MindfightResult, long> resultRepository,
            IRepository<Question, long> questionRepository,
            IRepository<Tour, long> tourRepository,
            IRepository<TeamAnswer, long> teamAnswerRepository,
            IRepository<Registration, long> registrationRepository,
            IPermissionChecker permissionChecker,
            UserManager userManager,
            IObjectMapper objectMapper
            )
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
            _objectMapper = objectMapper;
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
                throw new UserFriendlyException("Protmūšis su nurodytu id neegzistuoja!");
            }

            if (!(currentMindfight.CreatorId == _userManager.AbpSession.UserId
                  || _permissionChecker.IsGranted("ManageMindfights")))
            {
                throw new AbpAuthorizationException("Jūs neturite teisių kurti rezultatus!");
            }

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
            {
                throw new UserFriendlyException("Komanda su nurodytu id neegzistuoja!");
            }

            if (!currentMindfight.Registrations.Any(x => x.TeamId == teamId && x.IsConfirmed))
            {
                throw new UserFriendlyException("Nurodytos komandos registracija nėra patvirtinta!");
            }

            var teamRegistration = _registrationRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.TeamId == teamId && x.MindfightId == mindfightId);

            if (teamRegistration == null)
            {
                throw new UserFriendlyException("Komanda neužsiregistravo į protmūšį!");
            }

            var teamResult = await _resultRepository
                .GetAllIncluding(x => x.Team)
                .FirstOrDefaultAsync(x => x.MindfightId == mindfightId && x.TeamId == teamId);

            if (teamResult == null)
            {
                throw new UserFriendlyException("Komanda dar nebaigė žaisti protmūšio!");
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
                .Where(teamAnswer => questions.Any(y => y.Id == teamAnswer.QuestionId) && teamAnswer.TeamId == teamId)
                .ToListAsync();
            
            if (questions.Count != teamAnswers.Count)
            {
                throw new UserFriendlyException("Komanda dar nebaigė žaisti protmūšio!");
            }

            if (teamAnswers.Any(x => !x.IsEvaluated))
            {
                throw new UserFriendlyException("Nevisi komandos klausimai yra įvertinti!");
            }

            var earnedPoints = teamAnswers.Sum(teamAnswer => teamAnswer.EarnedPoints);

            var teamPlayers = await _userManager.Users
                .Include(x => x.Team)
                .Include(u => u.MindfightResults)
                .Where(u => u.TeamId == teamId && u.IsActiveInTeam)
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
                var currentUserMindfightResult = player.MindfightResults.FirstOrDefault(result => result.MindfightResultId == currentResult.Id);
                var userMindfightResult = new UserMindfightResult(player, currentResult);
                if (currentUserMindfightResult != null)
                {
                    currentUserMindfightResult.MindfightResult = currentResult;
                    currentUserMindfightResult.MindfightResultId = currentResult.Id;
                }
                else
                {
                    player.MindfightResults.Add(userMindfightResult);
                }
            }
        }

        public async Task<MindfightResultDto> GetMindfightTeamResult(long mindfightId, long teamId)
        {
            var currentMindfight = await _mindfightRepository
                .GetAllIncluding(mindfight => mindfight.Tours)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);

            if (currentMindfight == null)
            {
                throw new UserFriendlyException("Protmūšis su nurodytu id neegzistuoja!");
            }

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
            {
                throw new UserFriendlyException("Komanda su nurodytu id neegzistuoja!");
            }

            var currentResult = await _resultRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.MindfightId == mindfightId && x.TeamId == teamId);

            if (currentResult == null)
            {
                throw new UserFriendlyException("Protmūšio rezultatas neegizstuoja");
            }

            var mindfightQuestions = await _questionRepository
                .GetAllIncluding(question => question.Tour)
                .Where(question => question.Tour.MindfightId == mindfightId)
                .ToListAsync();

            var totalPoints = mindfightQuestions.Sum(question => question.Points);

            var resultDto = new MindfightResultDto();
            _objectMapper.Map(currentResult, resultDto);
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
                throw new UserFriendlyException("Protmūšis su nurodytu id neegzistuoja!");
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
                _objectMapper.Map(teamResult, resultDto);
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
                throw new UserFriendlyException("Komanda su nurodytu id neegzistuoja!");
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
                _objectMapper.Map(teamResult, resultDto);
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
                throw new UserFriendlyException("Vartotojas neegzistuoja!");
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
                _objectMapper.Map(result, resultDto);
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
