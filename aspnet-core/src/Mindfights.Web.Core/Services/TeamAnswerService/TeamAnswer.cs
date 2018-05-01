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

namespace Mindfights.Services.TeamAnswerService
{
    [AbpMvcAuthorize]
    public class TeamAnswer : ITeamAnswerService
    {
        private readonly IRepository<Question, long> _questionRepository;
        private readonly IRepository<Models.TeamAnswer, long> _teamAnswerRepository;
        private readonly IRepository<Mindfight, long> _mindfightRepository;
        private readonly IRepository<Team, long> _teamRepository;
        private readonly IRepository<Tour, long> _tourRepository;
        private readonly IRepository<MindfightResult, long> _resultRepository;
        private readonly IPermissionChecker _permissionChecker;
        private readonly UserManager _userManager;

        public TeamAnswer(
            IRepository<Question, long> questionRepository,
            IRepository<Models.TeamAnswer, long> teamAnswerRepository,
            IRepository<Mindfight, long> mindfightRepository,
            IRepository<Team, long> teamRepository,
            IRepository<Tour, long> tourRepository,
            IRepository<MindfightResult, long> resultRepository,
            IPermissionChecker permissionChecker,
            UserManager userManager)
        {
            _questionRepository = questionRepository;
            _teamAnswerRepository = teamAnswerRepository;
            _mindfightRepository = mindfightRepository;
            _teamRepository = teamRepository;
            _tourRepository = tourRepository;
            _resultRepository = resultRepository;
            _permissionChecker = permissionChecker;
            _userManager = userManager;
        }

        public async Task<List<long>> CreateTeamAnswer(List<TeamAnswerDto> teamAnswers, long mindfightId)
        {
            var insertedTeamAnswerIds = new List<long>();
            var currentMindfight = await _mindfightRepository
                .GetAll()
                .Include(x => x.Registrations)
                .ThenInclude(x => x.Team)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);

            if (currentMindfight == null)
            {
                throw new UserFriendlyException("Protmūšis su nurodytu id neegzistuoja!");
            }

            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .Include(x => x.Team)
                .FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);

            if (user == null)
            {
                throw new UserFriendlyException("Vartotojas neegzistuoja!");
            }

            if (user.Team == null)
            {
                throw new UserFriendlyException("Vartotojas neturi komandos!");
            }

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == user.Team.Id);

            if (currentTeam.LeaderId != _userManager.AbpSession.UserId)
            {
                throw new UserFriendlyException("Vartotojas nėra komadnos kapitonas!");
            }

            if (!currentMindfight.Registrations.Any(x => x.TeamId == user.Team.Id && x.IsConfirmed))
            {
                throw new UserFriendlyException("Komandos registracija nėra patvirtinta");
            }

            foreach (var answer in teamAnswers)
            {
                var currentQuestion = await _questionRepository
                    .FirstOrDefaultAsync(x => x.Id == answer.QuestionId);
                if (currentQuestion == null) continue;

                var teamAnswer = await _teamAnswerRepository
                    .GetAll()
                    .Where(x => answer.QuestionId == x.QuestionId && answer.TeamId == user.TeamId)
                    .FirstOrDefaultAsync();
                if (teamAnswer != null) continue;

                var teamAnswerToInsert = new Models.TeamAnswer(currentQuestion, user.Team, answer.EnteredAnswer, false);
                var insertedTeamAnswerId = await _teamAnswerRepository.InsertAndGetIdAsync(teamAnswerToInsert);
                insertedTeamAnswerIds.Add(insertedTeamAnswerId);
            }

            var currentMindfightResult = await _resultRepository
                .FirstOrDefaultAsync(result => result.TeamId == currentTeam.Id && result.MindfightId == currentMindfight.Id);

            if (currentMindfightResult == null)
            {
                currentMindfightResult = new MindfightResult(0, false, currentTeam, currentMindfight);
                await _resultRepository.InsertOrUpdateAsync(currentMindfightResult);
            }

            var teamPlayers = await _userManager.Users
                .IgnoreQueryFilters()
                .Include(x => x.Team)
                .Include(u => u.MindfightResults)
                .Where(u => u.TeamId == user.Team.Id && u.IsActiveInTeam)
                .ToListAsync();

            foreach (var player in teamPlayers)
            {
                var currentUserMindfightResult = player.MindfightResults.FirstOrDefault(result => result.MindfightResultId == currentMindfightResult.Id);
                var userMindfightResult = new UserMindfightResult(player, currentMindfightResult);
                if (currentUserMindfightResult == null)
                {
                    player.MindfightResults.Add(userMindfightResult);
                }
            }

            return insertedTeamAnswerIds;
        }

        public async Task<TeamAnswerDto> GetTeamAnswer(long questionId, long teamId)
        {
            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .Include(x => x.Team)
                .FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);

            if (user == null)
            {
                throw new UserFriendlyException("Vartotojas neegzistuoja!");
            }

            var currentQuestion = await _questionRepository
                .GetAll()
                .Include(x => x.Tour)
                .ThenInclude(x => x.Mindfight)
                .ThenInclude(x => x.Evaluators)
                .FirstOrDefaultAsync(x => x.Id == questionId);

            if (currentQuestion == null)
            {
                throw new UserFriendlyException("Klausimas su nurodytu id neegzistuoja!");
            }

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
            {
                throw new UserFriendlyException("Komanda su nurodytu id neegzistuoja!");
            }

            if (!(currentQuestion.Tour.Mindfight.CreatorId == _userManager.AbpSession.UserId
                  || _permissionChecker.IsGranted("ManageMindfights")
                  || currentQuestion.Tour.Mindfight.Evaluators.Any(x => x.UserId == _userManager.AbpSession.UserId)
                  || user.TeamId == teamId))
            {
                throw new AbpAuthorizationException("Jūs neturite teisių gauti šiuos atsakymus!");
            }

            var teamAnswer = await _teamAnswerRepository
                .GetAll()
                .Include(answer => answer.Evaluator)
                .Include(answer => answer.Question)
                .ThenInclude(question => question.Tour)
                .FirstOrDefaultAsync(x => x.QuestionId == questionId && x.TeamId == teamId);

            if (teamAnswer == null)
            {
                throw new UserFriendlyException("Komandos atsakymas neegizstuoja!");
            }

            var teamAnswerDto = new TeamAnswerDto();
            teamAnswer.MapTo(teamAnswerDto);
            teamAnswerDto.Evaluator = teamAnswer.Evaluator?.EmailAddress.ToLower();
            teamAnswerDto.TourOrderNumber = teamAnswer.Question.Tour.OrderNumber;
            return teamAnswerDto;
        }

        public async Task<List<TeamAnswerDto>> GetAllTeamAnswers(long mindfightId, long teamId)
        {
            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .Include(x => x.Team)
                .FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);

            if (user == null)
            {
                throw new UserFriendlyException("Vartotojas neegzistuoja!");
            }

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
            {
                throw new UserFriendlyException("Komanda su nurodytu id neegzistuoja!");
            }

            var currentMindfight = await _mindfightRepository
                .FirstOrDefaultAsync(mindfight => mindfight.Id == mindfightId);

            if (currentMindfight == null)
            {
                throw new UserFriendlyException("Protmūšis su nurodytu id neegzistuoja!");
            }

            var currentTours = await _tourRepository
                .GetAll()
                .Include(x => x.Mindfight)
                .ThenInclude(x => x.Evaluators)
                .Where(tour => tour.MindfightId == mindfightId)
                .ToListAsync();

            var currentQuestions = await _questionRepository
                .GetAll()
                .Where(question => currentTours.Any(tour => question.TourId == tour.Id))
                .ToListAsync();

            if (!(
                currentMindfight.CreatorId == _userManager.AbpSession.UserId
                  || _permissionChecker.IsGranted("ManageMindfights")
                  || currentMindfight.Evaluators.Any(x => x.UserId == _userManager.AbpSession.UserId)
                  || user.TeamId == teamId
                  ))
            {
                throw new AbpAuthorizationException("Jūs neturite teisių gauti šiam atsakymui");
            }

            var teamAnswers = await _teamAnswerRepository
                .GetAll()
                .Include(answer => answer.Evaluator)
                .Include(answer => answer.Question)
                .ThenInclude(question => question.Tour)
                .OrderBy(answer => answer.Question.Tour.OrderNumber)
                .Where(x => currentQuestions.Any(y => y.Id == x.QuestionId) && x.TeamId == teamId)
                .ToListAsync();

            var teamAnswersDto = new List<TeamAnswerDto>();
            foreach (var teamAnswer in teamAnswers)
            {
                var teamAnswerDto = new TeamAnswerDto();
                teamAnswer.MapTo(teamAnswerDto);
                teamAnswerDto.Evaluator = teamAnswer.Evaluator?.EmailAddress.ToLower();
                teamAnswerDto.TourOrderNumber = teamAnswer.Question.Tour.OrderNumber;
                teamAnswersDto.Add(teamAnswerDto);
            }
            return teamAnswersDto;
        }

        public async Task UpdateIsEvaluated(long questionId, long teamId,
            string evaluatorComment, int earnedPoints, bool isEvaluated)
        {
            var currentQuestion = await _questionRepository
                .GetAll()
                .Include(x => x.Tour)
                .ThenInclude(x => x.Mindfight)
                .ThenInclude(x => x.Evaluators)
                .FirstOrDefaultAsync(x => x.Id == questionId);

            if (currentQuestion == null)
            {
                throw new UserFriendlyException("Klausimas su nurodytu id neegzistuoja!");
            }

            var evaluator = await _userManager.Users
                .IgnoreQueryFilters()
                .Include(x => x.Team)
                .FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);

            if (evaluator == null)
            {
                throw new UserFriendlyException("Vartotojas neegzistuoja!");
            }

            if (!(currentQuestion.Tour.Mindfight.CreatorId == _userManager.AbpSession.UserId
                  || _permissionChecker.IsGranted("ManageMindfights")
                  || currentQuestion.Tour.Mindfight.Evaluators.Any(x => x.UserId == _userManager.AbpSession.UserId)))
            {
                throw new AbpAuthorizationException("Jūs neturite teisių redaguoti atsakymus!");
            }

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
            {
                throw new UserFriendlyException("Komanda su nurodytu id neegzistuoja!");
            }

            var teamAnswer = await _teamAnswerRepository.GetAll()
                .FirstOrDefaultAsync(x => x.QuestionId == questionId && x.TeamId == teamId);

            if (teamAnswer == null)
            {
                throw new UserFriendlyException("Komandos atsakymas neegzistuoja!");
            }

            teamAnswer.IsEvaluated = isEvaluated;
            teamAnswer.Evaluator = evaluator;
            teamAnswer.EvaluatorComment = evaluatorComment;
            if (earnedPoints > 0)
            {
                teamAnswer.EarnedPoints = earnedPoints > currentQuestion.Points ? currentQuestion.Points : earnedPoints;
            }
            else
            {
                teamAnswer.EarnedPoints = 0;
            }
            await _teamAnswerRepository.UpdateAsync(teamAnswer);
        }
    }
}
