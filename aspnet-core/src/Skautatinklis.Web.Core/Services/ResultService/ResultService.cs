using System.Linq;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Skautatinklis.Authorization.Users;
using Skautatinklis.DTOs;
using Skautatinklis.Models;

namespace Skautatinklis.Services.ResultService
{
    public class ResultService : IResultService
    {
        private readonly IRepository<Mindfight, long> _mindfightRepository;
        private readonly IRepository<Team, long> _teamRepository;
        private readonly IRepository<MindfightResult, long> _resultRepository;
        private readonly IRepository<MindfightQuestion, long> _mindfightQuestionRepository;
        private readonly IRepository<TeamAnswer, long> _teamAnswerRepository;
        private readonly IRepository<MindfightRegistration, long> _registrationRepository;
        private readonly UserManager _userManager;

        public ResultService(
            IRepository<Mindfight, long> mindfightRepository,
            IRepository<Team, long> teamRepository,
            IRepository<MindfightResult, long> resultRepository,
            IRepository<MindfightQuestion, long> mindfightQuestionRepository,
            IRepository<TeamAnswer, long> teamAnswerRepository,
            IRepository<MindfightRegistration, long> registrationRepository,
            UserManager userManager)
        {
            _mindfightRepository = mindfightRepository;
            _teamRepository = teamRepository;
            _resultRepository = resultRepository;
            _mindfightQuestionRepository = mindfightQuestionRepository;
            _teamAnswerRepository = teamAnswerRepository;
            _registrationRepository = registrationRepository;
            _userManager = userManager;
        }

        public async Task CreateResult(long mindfightId, long teamId, long userId)
        {
            var currentMindfight = await _mindfightRepository
                .GetAllIncluding(x => x.AllowedTeams)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);

            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (currentTeam == null)
                throw new UserFriendlyException("Team with specified id does not exist!");

            if (currentMindfight.IsPrivate && currentMindfight.AllowedTeams.Any(x => x.TeamId != teamId))
                throw new UserFriendlyException("Team is not allowed to play this mindfight!");

            var teamRegistration = _registrationRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.TeamId == teamId && x.MindfightId == mindfightId);

            if (teamRegistration == null)
                throw new UserFriendlyException("Team was not registered to play!");

            if (currentMindfight.IsPrivate && currentMindfight.AllowedTeams.Any(x => x.TeamId != teamId))
                throw new UserFriendlyException("Team is not allowed to play this mindfight!");

            var questions = await _mindfightQuestionRepository
                .GetAll()
                .Where(x => x.MindfightId == mindfightId)
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

            foreach (var member in teamMembers)
            {
                var mindfightResult = new MindfightResult(earnedPoints, true, currentTeam, currentMindfight);
                var userMindfightResult = new UserMindfightResult(member, mindfightResult);
                member.MindfightResults.Add(userMindfightResult);
            }
        }

        public async Task<MindfightResultDto> GetMindfightTeamResult(long mindfightId, long teamId, long userId)
        {
            var currentMindfight = await _mindfightRepository
                .GetAllIncluding(x => x.AllowedTeams)
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
            resultDto.QuestionsCount = currentMindfight.QuestionsCount;
            resultDto.TeamName = currentTeam.Name;
            resultDto.TotalPoints = currentMindfight.TotalPoints;

            return resultDto;
        }

        //Get Mindfight Winner
    }
}
