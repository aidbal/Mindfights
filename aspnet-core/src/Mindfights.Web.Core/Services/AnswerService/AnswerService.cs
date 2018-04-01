using Abp.AspNetCore.Mvc.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Mindfights.Authorization.Users;
using Mindfights.DTOs;
using Mindfights.Models;

namespace Mindfights.Services.AnswerService
{
    [AbpMvcAuthorize]
    public class AnswerService : IAnswerService
    {
        private readonly IRepository<Question, long> _questionRepository;
        private readonly IRepository<Answer, long> _answerRepository;
        private readonly IPermissionChecker _permissionChecker;
        private readonly UserManager _userManager;

        public AnswerService(
            IRepository<Question, long> questionRepository, 
            IRepository<Answer, long> answerRepository,
            IPermissionChecker permissionChecker,
            UserManager userManager)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _permissionChecker = permissionChecker;
            _userManager = userManager;
        }

        public async Task<MindfightQuestionAnswerDto> GetQuestionAnswer(long questionId)
        {
            var currentQuestion = await _questionRepository
                .GetAll()
                .Include(x => x.Tour)
                .ThenInclude(x => x.Mindfight)
                .FirstOrDefaultAsync(x => x.Id == questionId);
            if (currentQuestion == null)
                throw new UserFriendlyException("Question with specified id does not exist!");

            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);

            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            if (currentQuestion.Tour.Mindfight.CreatorId != _userManager.AbpSession.UserId 
                || !_permissionChecker.IsGranted("ManageMindfights")
                || currentQuestion.Tour.Mindfight.Evaluators.All(x => x.UserId != _userManager.AbpSession.UserId))
                throw new AbpAuthorizationException("You are not creator of this mindfight!");
            
            var answer = await _answerRepository
                .FirstOrDefaultAsync(x => x.QuestionId == questionId);

            var answerDto = new MindfightQuestionAnswerDto();
            answer.MapTo(answerDto);
            return answerDto;
        }

        public async Task<long> CreateQuestionAnswer(MindfightQuestionAnswerDto answer, long questionId)
        {
            var currentQuestion = await _questionRepository
                .GetAll()
                .Include(x => x.Tour)
                .ThenInclude(x => x.Mindfight)
                .FirstOrDefaultAsync(x => x.Id == questionId);
            if (currentQuestion == null)
                throw new UserFriendlyException("Question with specified id does not exist!");

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            if (currentQuestion.Tour.Mindfight.CreatorId != _userManager.AbpSession.UserId
                || !_permissionChecker.IsGranted("ManageMindfights"))
                throw new AbpAuthorizationException("You are not creator of this mindfight!");

            var answerToCreate = new Answer(currentQuestion, answer.Description, answer.IsCorrect);
            return await _answerRepository.InsertAndGetIdAsync(answerToCreate);
        }

        public async Task UpdateQuestionAnswer(MindfightQuestionAnswerDto answer, long answerId)
        {
            var currentAnswer = await _answerRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == answerId);
            if (currentAnswer == null)
                throw new UserFriendlyException("Answer with specified id does not exist!");

            var currentQuestion = await _questionRepository
                .GetAll()
                .Include(x => x.Tour)
                .ThenInclude(x => x.Mindfight)
                .FirstOrDefaultAsync(x => x.Id == currentAnswer.QuestionId);
            if (currentQuestion == null)
                throw new UserFriendlyException("Question with specified id does not exist!");

            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            if (currentQuestion.Tour.Mindfight.CreatorId != _userManager.AbpSession.UserId
                || !_permissionChecker.IsGranted("ManageMindfights"))
                throw new AbpAuthorizationException("You are not creator of this mindfight!");

            currentAnswer.Description = answer.Description;
            currentAnswer.IsCorrect = answer.IsCorrect;
            await _answerRepository.UpdateAsync(currentAnswer);
        }

        public async Task DeleteQuestionAnswer(long answerId)
        {
            var currentAnswer = await _answerRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == answerId);
            if (currentAnswer == null)
                throw new UserFriendlyException("Answer with specified id does not exist!");

            var currentQuestion = await _questionRepository
                .GetAll()
                .Include(x => x.Tour)
                .ThenInclude(x => x.Mindfight)
                .FirstOrDefaultAsync(x => x.Id == currentAnswer.QuestionId);
            if (currentQuestion == null)
                throw new UserFriendlyException("Question with specified id does not exist!");

            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            if (currentQuestion.Tour.Mindfight.CreatorId != _userManager.AbpSession.UserId
                || !_permissionChecker.IsGranted("ManageMindfights"))
                throw new AbpAuthorizationException("You are not creator of this mindfight!");

            await _answerRepository.DeleteAsync(currentAnswer);
        }
    }
}
