using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Skautatinklis.Authorization.Users;
using Skautatinklis.DTOs;
using Skautatinklis.Models;

namespace Skautatinklis.Services.QuestionService
{
    public class QuestionService : IQuestionService
    {
        private readonly IRepository<Mindfight, long> _mindfightRepository;
        private readonly IRepository<MindfightQuestion, long> _mindfightQuestionRepository;
        private readonly IRepository<MindfightQuestionType, long> _mindfightQuestionTypeRepository;
        private readonly IRepository<Team, long> _teamRepository;
        private readonly IRepository<TeamAnswer, long> _teamAnswerRepository;
        private readonly UserManager _userManager;

        public QuestionService(
            IRepository<Mindfight, long> mindfightRepository, 
            IRepository<MindfightQuestion, long> mindfightQuestionRepository, 
            IRepository<MindfightQuestionType, long> mindfightQuestionTypeRepository, 
            IRepository<Team, long> teamRepository, 
            IRepository<TeamAnswer, long> teamAnswerRepository, 
            UserManager userManager)
        {
            _mindfightRepository = mindfightRepository;
            _mindfightQuestionRepository = mindfightQuestionRepository;
            _mindfightQuestionTypeRepository = mindfightQuestionTypeRepository;
            _teamRepository = teamRepository;
            _teamAnswerRepository = teamAnswerRepository;
            _userManager = userManager;
        }

        public async Task<List<MindfightQuestionDto>> GetAllQuestions(long mindfightId, long userId)
        {
            var currentMindfight = await _mindfightRepository
                .GetAllIncluding(x => x.Evaluators)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");
            
            if (currentMindfight.CreatorId != userId || currentMindfight.Evaluators.All(x => x.UserId != userId))
                throw new UserFriendlyException("You are not creator of this mindfight!");

            var questionsDto = new List<MindfightQuestionDto>();
            var questions = await _mindfightQuestionRepository
                .GetAll()
                .Where(x => x.MindfightId == currentMindfight.Id)
                .ToListAsync();
            foreach (var question in questions)
            {
                var questionDto = new MindfightQuestionDto();
                question.MapTo(questionDto);
                questionsDto.Add(questionDto);
            }
            return questionsDto;
        }

        public async Task<MindfightQuestionDto> GetQuestion(long mindfightId, long userId, int orderNumber)
        {
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");
            
            var currentMindfight = await _mindfightRepository
                .GetAllIncluding(x => x.Evaluators)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            if (currentMindfight.CreatorId != userId || currentMindfight.Evaluators.All(x => x.UserId != userId))
                throw new UserFriendlyException("Insufficient permissions to get this question!");

            var currentQuestion = await _mindfightQuestionRepository.FirstOrDefaultAsync(x => x.OrderNumber == orderNumber);
            if (currentQuestion == null)
                throw new UserFriendlyException("Question with specified order number does not exist!");

            var question = new MindfightQuestionDto();
            currentQuestion.MapTo(question);
            return question;
        }

        public async Task<MindfightQuestionDto> GetNextQuestion(long mindfightId, long teamId, long userId)
        {
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            var currentMindfight = await _mindfightRepository.FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            //if (currentMindfight.StartTime.AddMinutes(currentMindfight.PrepareTime ?? 0) > Clock.Now.AddMinutes(-1))
            //    throw new UserFriendlyException("Mindfight has not yet started!");

            //if (currentMindfight.EndTime > Clock.Now)
            //    throw new UserFriendlyException("Mindfight is already over!");

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);
            if (currentTeam == null)
                throw new UserFriendlyException("Team with specified id does not exist!");

            var lastTeamAnswer = await _teamAnswerRepository
                .GetAllIncluding(x => x.Question)
                .OrderByDescending(x => x.Question.OrderNumber)
                .FirstOrDefaultAsync(x => x.TeamId == teamId && x.Question.MindfightId == mindfightId);

            var nextQuestionOrderNumber = 1;

            if (lastTeamAnswer != null)
                nextQuestionOrderNumber = lastTeamAnswer.Question.OrderNumber + 1;

            var mindfightQuestions = await _mindfightQuestionRepository
                .GetAll()
                .OrderBy(x => x.OrderNumber)
                .Where(x => x.MindfightId == mindfightId && x.OrderNumber >= nextQuestionOrderNumber)
                .ToListAsync();

            if (mindfightQuestions.Count == 0)
                throw new UserFriendlyException("There are no more questions left to answer!");

            var currentQuestion = mindfightQuestions.First();

            var questionDto = new MindfightQuestionDto();
            currentQuestion.MapTo(questionDto);

            if (mindfightQuestions.Count == 1)
                questionDto.IsLastQuestion = true;

            return questionDto;
        }

        public async Task<long> CreateQuestion(MindfightQuestionDto question, long mindfightId, long userId)
        {
            var currentMindfight = await _mindfightRepository.FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            if (currentMindfight.CreatorId != userId)
                throw new UserFriendlyException("You are not creator of this mindfight!");

            var questionType = await _mindfightQuestionTypeRepository.FirstOrDefaultAsync(x => x.Id == question.QuestionTypeId);
            if (questionType == null)
                throw new UserFriendlyException("There was a problem getting question type from database!");

            question.OrderNumber = await GetLastOrderNumber(mindfightId);
            question.OrderNumber = question.OrderNumber == 0 ? 1 : question.OrderNumber;
            currentMindfight.QuestionsCount += 1;
            var points = question.Points > 0 ? question.Points : 0;
            currentMindfight.TotalPoints += points;

            var questionToCreate = new MindfightQuestion(
                currentMindfight,
                questionType,
                question.Title,
                question.Description,
                question.TimeToAnswerInSeconds,
                points,
                question.OrderNumber, 
                question.AttachmentLocation);
            return await _mindfightQuestionRepository.InsertAndGetIdAsync(questionToCreate);
        }

        public async Task UpdateQuestion(MindfightQuestionDto question, long questionId, long mindfightId, long userId)
        {
            var currentMindfight = await _mindfightRepository.FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            if (currentMindfight.CreatorId != userId)
                throw new UserFriendlyException("You are not creator of this mindfight!");

            var questionToUpdate = await _mindfightQuestionRepository.FirstOrDefaultAsync(x => x.Id == questionId);
            if (questionToUpdate == null)
                throw new UserFriendlyException("Question with specified id does not exist!");

            var questionType = await _mindfightQuestionTypeRepository.FirstOrDefaultAsync(x => x.Id == question.QuestionTypeId);
            if (questionType == null)
                throw new UserFriendlyException("There was a problem getting question type from database!");

            question.OrderNumber = questionToUpdate.OrderNumber;
            question.MapTo(questionToUpdate);
            questionToUpdate.Id = questionId;
            await _mindfightQuestionRepository.UpdateAsync(questionToUpdate);
        }

        public async Task DeleteQuestion(long questionId, long userId)
        {
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            var questionToDelete = await _mindfightQuestionRepository.FirstOrDefaultAsync(x => x.Id == questionId);
            if (questionToDelete == null)
                throw new UserFriendlyException("Question with specified id does not exist!");

            var currentMindfight = await _mindfightRepository.FirstOrDefaultAsync(x => x.Id == questionToDelete.MindfightId);
            if (currentMindfight == null)
                throw new UserFriendlyException("Error getting question's mindfight!");

            if (currentMindfight.CreatorId != userId)
                throw new UserFriendlyException("You are not creator of this mindfight!");

            currentMindfight.QuestionsCount -= 1;
            var orderNumber = questionToDelete.OrderNumber;
            await UpdateOrderNumbers(orderNumber, questionToDelete.Id, currentMindfight.Id);
            await _mindfightQuestionRepository.DeleteAsync(questionToDelete);
        }

        public async Task UpdateOrderNumber(long questionId, long userId, int newOrderNumber)
        {
            var currentQuestion = await _mindfightQuestionRepository.FirstOrDefaultAsync(x => x.Id == questionId);
            if (currentQuestion == null)
                throw new UserFriendlyException("Question with specified id does not exist!");

            var currentMindfight = await _mindfightRepository.FirstOrDefaultAsync(x => x.MindfightQuestions.Any(y => y.Id == questionId));
            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            var questionWithNewOrderNumber = await _mindfightQuestionRepository.GetAll()
                .Where(x => x.MindfightId == currentQuestion.MindfightId && x.OrderNumber == newOrderNumber)
                .FirstOrDefaultAsync();
            if (questionWithNewOrderNumber == null)
            {
                var lastOrderNumber = await GetLastOrderNumber(currentQuestion.MindfightId);
                var lastQuestion = await _mindfightQuestionRepository.GetAll()
                    .Where(x => x.MindfightId == currentQuestion.MindfightId && x.OrderNumber == lastOrderNumber)
                    .FirstOrDefaultAsync();
                if (lastQuestion != null)
                {
                    lastQuestion.OrderNumber = currentQuestion.OrderNumber;
                }
                currentQuestion.OrderNumber = lastOrderNumber;
            }
            else
            {
                questionWithNewOrderNumber.OrderNumber = currentQuestion.OrderNumber;
                currentQuestion.OrderNumber = newOrderNumber;
            }
        }

        private async Task UpdateOrderNumbers(int deletedOrderNumber, long deletedQuestionId, long mindfightId)
        {
            var questions = await _mindfightQuestionRepository.GetAll()
                .Where(x => x.MindfightId == mindfightId && x.Id != deletedQuestionId)
                .OrderBy(x => x.OrderNumber)
                .ToListAsync();
            var nextOrderNumber = deletedOrderNumber;
            foreach (var question in questions)
            {
                if (question.OrderNumber < deletedOrderNumber) continue;
                question.OrderNumber = nextOrderNumber;
                nextOrderNumber++;
            }
        }

        private async Task<int> GetLastOrderNumber(long mindfightId)
        {
            var lastOrderNumber = 0;
            var lastQuestion = await _mindfightQuestionRepository.GetAll().Where(x => x.MindfightId == mindfightId)
                .OrderByDescending(x => x.OrderNumber).FirstOrDefaultAsync();

            if (lastQuestion != null)
            {
                lastOrderNumber = lastQuestion.OrderNumber;
            }

            return lastOrderNumber;
        }
    }
}
