using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Mindfights.Authorization.Users;
using Mindfights.DTOs;
using Mindfights.Models;

namespace Mindfights.Services.TeamAnswerService
{
    public class TeamAnswerService : ITeamAnswerService
    {
        private readonly IRepository<Question, long> _questionRepository;
        private readonly IRepository<TeamAnswer, long> _teamAnswerRepository;
        private readonly IRepository<Mindfight, long> _mindfightRepository;
        private readonly IRepository<Team, long> _teamRepository;
        private readonly IRepository<Tour, long> _tourRepository;
        private readonly UserManager _userManager;

        public TeamAnswerService(
            IRepository<Question, long> questionRepository,
            IRepository<TeamAnswer, long> teamAnswerRepository,
            IRepository<Mindfight, long> mindfightRepository,
            IRepository<Team, long> teamRepository,
            IRepository<Tour, long> tourRepository,
            UserManager userManager)
        {
            _questionRepository = questionRepository;
            _teamAnswerRepository = teamAnswerRepository;
            _mindfightRepository = mindfightRepository;
            _teamRepository = teamRepository;
            _tourRepository = tourRepository;
            _userManager = userManager;
        }

        public async Task<long> CreateTeamAnswer(string enteredAnswer, long questionId, long userId)
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
                .Include(x => x.Registrations).ThenInclude(x => x.Team)
                .FirstOrDefaultAsync(x => x.Id == currentQuestion.TourId);

            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .Include(x => x.Team)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            if (user.Team == null)
                throw new UserFriendlyException("User does not have a team!");

            if (currentMindfight.Registrations.Any(x => x.TeamId != user.Team.Id && x.IsConfirmed))
                throw new UserFriendlyException("User's team is not allowed to play this mindfight!");

            var teamAnswerToInsert = new TeamAnswer(currentQuestion, user.Team, enteredAnswer, false);
            return await _teamAnswerRepository.InsertAndGetIdAsync(teamAnswerToInsert);
        }

        public async Task<TeamAnswerDto> GetTeamAnswer(long questionId, long teamId, long userId)
        {
            var currentQuestion = await _questionRepository
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
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            var teamAnswer = await _teamAnswerRepository.GetAll()
                .FirstOrDefaultAsync(x => x.QuestionId == questionId && x.TeamId == teamId);

            if (teamAnswer == null)
                throw new UserFriendlyException("Team answer does not exist!");

            var teamAnswerDto = new TeamAnswerDto();
            teamAnswer.MapTo(teamAnswerDto);
            return teamAnswerDto;
        }

        public async Task<List<TeamAnswerDto>> GetAllTeamAnswers(long tourId, long teamId, long userId)
        {
            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
                throw new UserFriendlyException("Team with specified id does not exist!");

            var currentTour = await _tourRepository
                .FirstOrDefaultAsync(x => x.Id == tourId);

            if (currentTour == null)
                throw new UserFriendlyException("Tour with specified id does not exist!");

            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .Include(x => x.Team)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            var currentQuestions = await _questionRepository
                .GetAll()
                .Where(x => x.TourId == tourId)
                .ToListAsync();

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

        public async Task UpdateIsCurrentlyEvaluated(long questionId, long teamId, long userId, bool isCurrentlyEvaluated)
        {
            var currentQuestion = await _questionRepository
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
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            var teamAnswer = await _teamAnswerRepository.GetAll()
                .FirstOrDefaultAsync(x => x.QuestionId == questionId && x.TeamId == teamId);

            if (teamAnswer == null)
                throw new UserFriendlyException("Team answer does not exist!");

            teamAnswer.IsCurrentlyEvaluated = isCurrentlyEvaluated;
            await _teamAnswerRepository.UpdateAsync(teamAnswer);
        }

        public async Task UpdateIsEvaluated(long questionId, long teamId, long evaluatorId, 
            string evaluatorComment, int earnedPoints, bool isEvaluated)
        {
            var currentQuestion = await _questionRepository
                .FirstOrDefaultAsync(x => x.Id == questionId);

            if (currentQuestion == null)
                throw new UserFriendlyException("Question with specified id does not exist!");

            var evaluator = await _userManager.Users
                .IgnoreQueryFilters()
                .Include(x => x.Team)
                .FirstOrDefaultAsync(u => u.Id == evaluatorId);

            if (evaluator == null)
                throw new UserFriendlyException("User does not exist!");

            var currentMindfight = await _mindfightRepository
                .GetAllIncluding(x => x.Evaluators)
                .Where(x => x.Evaluators.Any(y => y.UserId == evaluatorId))
                .FirstOrDefaultAsync();

            if (currentMindfight == null)
                throw new UserFriendlyException("User is not allowed to evaluate!");

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
