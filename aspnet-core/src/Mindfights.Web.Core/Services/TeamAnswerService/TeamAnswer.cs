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
        private readonly IPermissionChecker _permissionChecker;
        private readonly UserManager _userManager;

        public TeamAnswer(
            IRepository<Question, long> questionRepository,
            IRepository<Models.TeamAnswer, long> teamAnswerRepository,
            IRepository<Mindfight, long> mindfightRepository,
            IRepository<Team, long> teamRepository,
            IRepository<Tour, long> tourRepository,
            IPermissionChecker permissionChecker,
            UserManager userManager)
        {
            _questionRepository = questionRepository;
            _teamAnswerRepository = teamAnswerRepository;
            _mindfightRepository = mindfightRepository;
            _teamRepository = teamRepository;
            _tourRepository = tourRepository;
            _permissionChecker = permissionChecker;
            _userManager = userManager;
        }

        public async Task<long> CreateTeamAnswer(string enteredAnswer, long questionId, long teamId)
        {
            var currentQuestion = await _questionRepository
                .FirstOrDefaultAsync(x => x.Id == questionId);

            if (currentQuestion == null)
                throw new UserFriendlyException("Question with specified id does not exist!");

            var teamAnswer = await _teamAnswerRepository
                .GetAll()
                .Where(x => questionId == x.QuestionId)
                .FirstOrDefaultAsync();

            if (teamAnswer != null)
                throw new UserFriendlyException("Team has already entered an answer to this question!");

            var currentMindfight = await _mindfightRepository
                .GetAll()
                .Include(x => x.Registrations)
                .ThenInclude(x => x.Team)
                .FirstOrDefaultAsync(x => x.Id == currentQuestion.TourId);

            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .Include(x => x.Team)
                .FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);

            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            if (user.Team == null)
                throw new UserFriendlyException("User does not have a team!");

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam.LeaderId != _userManager.AbpSession.UserId)
                throw new UserFriendlyException("User is not leader of the team!");

            if (currentMindfight.Registrations.Any(x => x.TeamId != user.Team.Id && x.IsConfirmed))
                throw new UserFriendlyException("User's team is not confirmed to play this mindfight!");

            var teamAnswerToInsert = new Models.TeamAnswer(currentQuestion, user.Team, enteredAnswer, false);
            return await _teamAnswerRepository.InsertAndGetIdAsync(teamAnswerToInsert);
        }

        public async Task<TeamAnswerDto> GetTeamAnswer(long questionId, long teamId)
        {
            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .Include(x => x.Team)
                .FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);

            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            var currentQuestion = await _questionRepository
                .GetAll()
                .Include(x => x.Tour)
                .ThenInclude(x => x.Mindfight)
                .ThenInclude(x => x.Evaluators)
                .FirstOrDefaultAsync(x => x.Id == questionId);

            if (currentQuestion == null)
                throw new UserFriendlyException("Question with specified id does not exist!");

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
                throw new UserFriendlyException("Team with specified id does not exist!");

            if (!(currentQuestion.Tour.Mindfight.CreatorId == _userManager.AbpSession.UserId
                || _permissionChecker.IsGranted("ManageMindfights")
                || currentQuestion.Tour.Mindfight.Evaluators.Any(x => x.UserId == _userManager.AbpSession.UserId)
                || user.TeamId == teamId))
                throw new AbpAuthorizationException("Insufficient permissions to get this team answer!");

            var teamAnswer = await _teamAnswerRepository.GetAll()
                .FirstOrDefaultAsync(x => x.QuestionId == questionId && x.TeamId == teamId);

            if (teamAnswer == null)
                throw new UserFriendlyException("Team answer does not exist!");

            var teamAnswerDto = new TeamAnswerDto();
            teamAnswer.MapTo(teamAnswerDto);
            return teamAnswerDto;
        }

        public async Task<List<TeamAnswerDto>> GetAllTeamAnswers(long tourId, long teamId)
        {
            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
                throw new UserFriendlyException("Team with specified id does not exist!");

            var currentTour = await _tourRepository
                .GetAll()
                .Include(x => x.Mindfight)
                .ThenInclude(x => x.Evaluators)
                .FirstOrDefaultAsync(x => x.Id == tourId);

            if (currentTour == null)
                throw new UserFriendlyException("Tour with specified id does not exist!");

            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .Include(x => x.Team)
                .FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);

            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            var currentQuestions = await _questionRepository
                .GetAll()
                .Where(x => x.TourId == tourId)
                .ToListAsync();

            if (!(currentTour.Mindfight.CreatorId == _userManager.AbpSession.UserId
                || _permissionChecker.IsGranted("ManageMindfights")
                || currentTour.Mindfight.Evaluators.Any(x => x.UserId == _userManager.AbpSession.UserId)
                || user.TeamId == teamId))
                throw new AbpAuthorizationException("Insufficient permissions to get this team answer!");

            var teamAnswers = await _teamAnswerRepository.GetAll()
                .Where(x => currentQuestions.Any(y => y.Id == x.QuestionId) && x.TeamId == teamId)
                .ToListAsync();

            var teamAnswersDto = new List<TeamAnswerDto>();
            foreach (var teamAnswer in teamAnswers)
            {
                var teamAnswerDto = new TeamAnswerDto();
                teamAnswer.MapTo(teamAnswerDto);
                teamAnswersDto.Add(teamAnswerDto);
            }
            return teamAnswersDto;
        }

        public async Task UpdateIsCurrentlyEvaluated(long questionId, long teamId, bool isCurrentlyEvaluated)
        {
            var currentQuestion = await _questionRepository
                .GetAll()
                .Include(x => x.Tour)
                .ThenInclude(x => x.Mindfight)
                .FirstOrDefaultAsync(x => x.Id == questionId);
            if (currentQuestion == null)
                throw new UserFriendlyException("Question with specified id does not exist!");

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
                throw new UserFriendlyException("Team with specified id does not exist!");

            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .Include(x => x.Team)
                .FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);

            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            var teamAnswer = await _teamAnswerRepository.GetAll()
                .FirstOrDefaultAsync(x => x.QuestionId == questionId && x.TeamId == teamId);

            if (teamAnswer == null)
                throw new UserFriendlyException("Team answer does not exist!");

            if (!(currentQuestion.Tour.Mindfight.CreatorId == _userManager.AbpSession.UserId
                || _permissionChecker.IsGranted("ManageMindfights")
                || currentQuestion.Tour.Mindfight.Evaluators.Any(x => x.UserId == _userManager.AbpSession.UserId)))
                throw new AbpAuthorizationException("Insufficient permissions!");

            teamAnswer.IsCurrentlyEvaluated = isCurrentlyEvaluated;
            await _teamAnswerRepository.UpdateAsync(teamAnswer);
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
                throw new UserFriendlyException("Question with specified id does not exist!");

            var evaluator = await _userManager.Users
                .IgnoreQueryFilters()
                .Include(x => x.Team)
                .FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);

            if (evaluator == null)
                throw new UserFriendlyException("User does not exist!");

            if (!(currentQuestion.Tour.Mindfight.CreatorId == _userManager.AbpSession.UserId
                || _permissionChecker.IsGranted("ManageMindfights")
                || currentQuestion.Tour.Mindfight.Evaluators.Any(x => x.UserId == _userManager.AbpSession.UserId)))
                throw new AbpAuthorizationException("Insufficient permissions!");

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
                throw new UserFriendlyException("Team with specified id does not exist!");

            var teamAnswer = await _teamAnswerRepository.GetAll()
                .FirstOrDefaultAsync(x => x.QuestionId == questionId && x.TeamId == teamId);

            if (teamAnswer == null)
                throw new UserFriendlyException("Team answer does not exist!");


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
