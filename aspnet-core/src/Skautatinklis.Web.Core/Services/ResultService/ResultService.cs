﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.Timing;
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
        private readonly IRepository<Question, long> _questionRepository;
        private readonly IRepository<TeamAnswer, long> _teamAnswerRepository;
        private readonly IRepository<Registration, long> _registrationRepository;
        private readonly UserManager _userManager;

        public ResultService(
            IRepository<Mindfight, long> mindfightRepository,
            IRepository<Team, long> teamRepository,
            IRepository<MindfightResult, long> resultRepository,
            IRepository<Question, long> questionRepository,
            IRepository<TeamAnswer, long> teamAnswerRepository,
            IRepository<Registration, long> registrationRepository,
            UserManager userManager)
        {
            _mindfightRepository = mindfightRepository;
            _teamRepository = teamRepository;
            _resultRepository = resultRepository;
            _questionRepository = questionRepository;
            _teamAnswerRepository = teamAnswerRepository;
            _registrationRepository = registrationRepository;
            _userManager = userManager;
        }

        public async Task CreateResult(long mindfightId, long tourId, long teamId, long userId)
        {
            var currentMindfight = await _mindfightRepository
                .GetAll()
                .Include(x => x.Registrations)
                .ThenInclude(x => x.Team)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);

            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

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

            foreach (var member in teamMembers)
            {
                var isWinner = await GetAndUpdateWinnerStatus(mindfightId, earnedPoints);
                var mindfightResult = new MindfightResult(earnedPoints, true, isWinner, currentTeam, currentMindfight);
                var userMindfightResult = new UserMindfightResult(member, mindfightResult);
                member.MindfightResults.Add(userMindfightResult);
            }
        }

        public async Task<MindfightResultDto> GetMindfightTeamResult(long mindfightId, long teamId, long userId)
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
            resultDto.QuestionsCount = currentMindfight.QuestionsCount;
            resultDto.TeamName = currentTeam.Name;
            resultDto.TotalPoints = currentMindfight.TotalPoints;

            return resultDto;
        }

        public async Task<List<MindfightResultDto>> GetMindfightResults(long mindfightId, long userId)
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
                resultDto.QuestionsCount = currentMindfight.QuestionsCount;
                resultDto.TeamName = teamResult.Team.Name;
                resultDto.TotalPoints = currentMindfight.TotalPoints;
                teamResultsDto.Add(resultDto);
            }

            return teamResultsDto;
        }

        public async Task<List<MindfightResultDto>> GetTeamResults(long teamId, long userId)
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
                resultDto.QuestionsCount = currentMindfight.QuestionsCount;
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
                            .Count(x => x.IsWinner)
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

        public async Task<LeaderBoardDto> GetMindfightWinner(long mindfightId)
        {
            var currentMindfight = await _mindfightRepository
                .GetAllIncluding(x => x.Evaluators)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            if (!currentMindfight.IsFinished)
                throw new UserFriendlyException("Mindfight is not yet over!");

            var mindfightResult = await _resultRepository
                .GetAllIncluding(x => x.Team)
                .OrderByDescending(x => x.EarnedPoints)
                .FirstOrDefaultAsync(x => x.MindfightId == mindfightId && x.IsWinner);

            if (mindfightResult == null)
                throw new UserFriendlyException("Mindfight has no winner yet!");

            var leaderBoardDto = new LeaderBoardDto
            {
                TeamId = mindfightResult.TeamId,
                TeamName = mindfightResult.Team.Name
            };

            return leaderBoardDto;
        }

        private async Task<bool> GetAndUpdateWinnerStatus(long mindfightId, int earnedPoints)
        {
            var results = await _resultRepository
                .GetAll()
                .Where(x => x.MindfightId == mindfightId)
                .ToListAsync();

            var newWinner = false;

            foreach (var result in results)
            {
                if (earnedPoints > result.EarnedPoints)
                {
                    newWinner = true;
                }
            }
            if (newWinner)
            {
                foreach (var result in results)
                {
                    if (result.IsWinner)
                    {
                        result.IsWinner = false;
                    }
                }
            }

            return newWinner;
        }
    }
}