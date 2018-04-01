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

namespace Mindfights.Services.QuestionService
{
    [AbpMvcAuthorize]
    public class QuestionService : IQuestionService
    {
        private readonly IRepository<Question, long> _questionRepository;
        private readonly IRepository<Team, long> _teamRepository;
        private readonly IRepository<TeamAnswer, long> _teamAnswerRepository;
        private readonly IRepository<Tour, long> _tourRepository;
        private readonly IPermissionChecker _permissionChecker;
        private readonly UserManager _userManager;

        public QuestionService(
            IRepository<Question, long> questionRepository, 
            IRepository<Team, long> teamRepository, 
            IRepository<TeamAnswer, long> teamAnswerRepository, 
            IRepository<Tour, long> tourRepository,
            IPermissionChecker permissionChecker,
            UserManager userManager)
        {
            _questionRepository = questionRepository;
            _teamRepository = teamRepository;
            _teamAnswerRepository = teamAnswerRepository;
            _tourRepository = tourRepository;
            _permissionChecker = permissionChecker;
            _userManager = userManager;
        }

        public async Task<List<QuestionDto>> GetAllTourQuestions(long tourId)
        {
            var currentTour = await _tourRepository
                .GetAll()
                .Include(x => x.Mindfight)
                .ThenInclude(x => x.Evaluators)
                .FirstOrDefaultAsync(x => x.Id == tourId);

            if (currentTour == null)
                throw new UserFriendlyException("Tour with specified id does not exist!");
            
            if (currentTour.Mindfight.CreatorId != _userManager.AbpSession.UserId
                || !_permissionChecker.IsGranted("ManageMindfights")
                || currentTour.Mindfight.Evaluators.All(x => x.UserId != _userManager.AbpSession.UserId))
                throw new AbpAuthorizationException("Insufficient permissions to get this question!");

            var questionsDto = new List<QuestionDto>();
            var questions = await _questionRepository
                .GetAll()
                .Where(x => x.TourId == currentTour.Id)
                .ToListAsync();

            foreach (var question in questions)
            {
                var questionDto = new QuestionDto();
                question.MapTo(questionDto);
                questionsDto.Add(questionDto);
            }
            return questionsDto;
        }

        public async Task<QuestionDto> GetQuestion(long tourId, int orderNumber)
        {            
            var currentTour = await _tourRepository
                .GetAll()
                .Include(x => x.Mindfight)
                .ThenInclude(x => x.Evaluators)
                .FirstOrDefaultAsync(x => x.Id == tourId);

            if (currentTour == null)
                throw new UserFriendlyException("Tour with specified id does not exist!");

            if (currentTour.Mindfight.CreatorId != _userManager.AbpSession.UserId
                || !_permissionChecker.IsGranted("ManageMindfights")
                || currentTour.Mindfight.Evaluators.All(x => x.UserId != _userManager.AbpSession.UserId))
                throw new AbpAuthorizationException("Insufficient permissions to get this question!");

            var currentQuestion = await _questionRepository.FirstOrDefaultAsync(x => x.OrderNumber == orderNumber);
            if (currentQuestion == null)
                throw new UserFriendlyException("Question with specified order number does not exist!");

            var question = new QuestionDto();
            currentQuestion.MapTo(question);
            return question;
        }

        public async Task<QuestionDto> GetNextQuestion(long tourId, long teamId)
        {
            var currentTour = await _tourRepository.FirstOrDefaultAsync(x => x.Id == tourId);
            if (currentTour == null)
                throw new UserFriendlyException("Tour with specified id does not exist!");

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
                .FirstOrDefaultAsync(x => x.TeamId == teamId && x.Question.TourId == tourId);

            var nextQuestionOrderNumber = 1;

            if (lastTeamAnswer != null)
                nextQuestionOrderNumber = lastTeamAnswer.Question.OrderNumber + 1;

            var mindfightQuestions = await _questionRepository
                .GetAll()
                .OrderBy(x => x.OrderNumber)
                .Where(x => x.TourId == tourId && x.OrderNumber >= nextQuestionOrderNumber)
                .ToListAsync();

            if (mindfightQuestions.Count == 0)
                throw new UserFriendlyException("There are no more questions left to answer!");

            var currentQuestion = mindfightQuestions.First();

            var questionDto = new QuestionDto();
            currentQuestion.MapTo(questionDto);

            if (mindfightQuestions.Count == 1)
                questionDto.IsLastQuestion = true;

            return questionDto;
        }

        public async Task<long> CreateQuestion(QuestionDto question, long tourId)
        {
            var currentTour = await _tourRepository
                .GetAll()
                .Include(x => x.Mindfight)
                .FirstOrDefaultAsync(x => x.Id == tourId);
            if (currentTour == null)
                throw new UserFriendlyException("Tour with specified id does not exist!");

            if (currentTour.Mindfight.CreatorId != _userManager.AbpSession.UserId
                || !_permissionChecker.IsGranted("ManageMindfights"))
                throw new AbpAuthorizationException("Insufficient permissions to create question!");

            question.OrderNumber = await GetLastOrderNumber(tourId);
            question.OrderNumber = question.OrderNumber == 0 ? 1 : question.OrderNumber;
            currentTour.QuestionsCount += 1;
            var points = question.Points > 0 ? question.Points : 0;
            currentTour.TotalPoints += points;

            var questionToCreate = new Question(
                currentTour,
                question.Title,
                question.Description,
                question.TimeToAnswerInSeconds,
                points,
                question.OrderNumber, 
                question.AttachmentLocation);
            return await _questionRepository.InsertAndGetIdAsync(questionToCreate);
        }

        public async Task UpdateQuestion(QuestionDto question, long questionId, long mindfightId)
        {
            var currentTour = await _tourRepository
                .GetAll()
                .Include(x => x.Mindfight)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);
            if (currentTour == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            if (currentTour.Mindfight.CreatorId != _userManager.AbpSession.UserId
                || !_permissionChecker.IsGranted("ManageMindfights"))
                throw new AbpAuthorizationException("Insufficient permissions to update question!");

            var questionToUpdate = await _questionRepository.FirstOrDefaultAsync(x => x.Id == questionId);
            if (questionToUpdate == null)
                throw new UserFriendlyException("Question with specified id does not exist!");
            
            question.OrderNumber = questionToUpdate.OrderNumber;
            question.MapTo(questionToUpdate);
            questionToUpdate.Id = questionId;
            await _questionRepository.UpdateAsync(questionToUpdate);
        }

        public async Task DeleteQuestion(long questionId)
        {
            var questionToDelete = await _questionRepository.FirstOrDefaultAsync(x => x.Id == questionId);
            if (questionToDelete == null)
                throw new UserFriendlyException("Question with specified id does not exist!");

            var currentTour = await _tourRepository
                .GetAll()
                .Include(x => x.Mindfight)
                .FirstOrDefaultAsync(x => x.Id == questionToDelete.TourId);

            if (currentTour == null)
                throw new UserFriendlyException("Error getting question's tour!");
            
            if (currentTour.Mindfight.CreatorId != _userManager.AbpSession.UserId
                || !_permissionChecker.IsGranted("ManageMindfights"))
                throw new AbpAuthorizationException("Insufficient permissions to delete question!");

            currentTour.QuestionsCount -= 1;
            var orderNumber = questionToDelete.OrderNumber;
            await UpdateOrderNumbers(orderNumber, questionToDelete.Id, currentTour.Id);
            await _questionRepository.DeleteAsync(questionToDelete);
        }

        public async Task UpdateOrderNumber(long questionId, int newOrderNumber)
        {
            var currentQuestion = await _questionRepository.FirstOrDefaultAsync(x => x.Id == questionId);
            if (currentQuestion == null)
                throw new UserFriendlyException("Question with specified id does not exist!");

            var currentTour = await _tourRepository.FirstOrDefaultAsync(x => x.Questions.Any(y => y.Id == questionId));
            if (currentTour == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            if (currentTour.Mindfight.CreatorId != _userManager.AbpSession.UserId
                || !_permissionChecker.IsGranted("ManageMindfights"))
                throw new AbpAuthorizationException("Insufficient permissions to update order number!");

            var questionWithNewOrderNumber = await _questionRepository
                .GetAll()
                .Where(x => x.TourId == currentQuestion.TourId && x.OrderNumber == newOrderNumber)
                .FirstOrDefaultAsync();

            if (questionWithNewOrderNumber == null)
            {
                var lastOrderNumber = await GetLastOrderNumber(currentQuestion.TourId);
                var lastQuestion = await _questionRepository.GetAll()
                    .Where(x => x.TourId == currentQuestion.TourId && x.OrderNumber == lastOrderNumber)
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

        private async Task UpdateOrderNumbers(int deletedOrderNumber, long deletedQuestionId, long tourId)
        {
            var questions = await _questionRepository.GetAll()
                .Where(x => x.TourId == tourId && x.Id != deletedQuestionId)
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

        private async Task<int> GetLastOrderNumber(long tourId)
        {
            var lastOrderNumber = 0;
            var lastQuestion = await _questionRepository
                .GetAll()
                .Where(x => x.TourId == tourId)
                .OrderByDescending(x => x.OrderNumber)
                .FirstOrDefaultAsync();

            if (lastQuestion != null)
            {
                lastOrderNumber = lastQuestion.OrderNumber;
            }

            return lastOrderNumber;
        }
    }
}
