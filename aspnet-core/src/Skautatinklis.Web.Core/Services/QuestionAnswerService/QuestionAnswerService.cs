﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Skautatinklis.Authorization.Users;
using Skautatinklis.DTOs;
using Skautatinklis.Models;

namespace Skautatinklis.Services.QuestionAnswerService
{
    public class QuestionAnswerService : IQuestionAnswerService
    {
        private readonly IRepository<MindfightQuestion, long> _mindfightQuestionRepository;
        private readonly IRepository<MindfightQuestionAnswer, long> _questionAnswerRepository;
        private readonly UserManager _userManager;

        public QuestionAnswerService(
            IRepository<MindfightQuestion, long> mindfightQuestionRepository, 
            IRepository<MindfightQuestionAnswer, long> questionAnswerRepository,
            UserManager userManager)
        {
            _mindfightQuestionRepository = mindfightQuestionRepository;
            _userManager = userManager;
            _questionAnswerRepository = questionAnswerRepository;
        }

        public async Task<List<MindfightQuestionAnswerDto>> GetQuestionAllAnswers(long questionId, long userId)
        {
            var currentQuestion = await _mindfightQuestionRepository
                .GetAllIncluding(x => x.Mindfight)
                .FirstOrDefaultAsync(x => x.Id == questionId);
            if (currentQuestion == null)
            {
                throw new UserFriendlyException("Question with specified id does not exist!");
            }

            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new UserFriendlyException("User does not exist!");
            }

            if (currentQuestion.Mindfight.CreatorId != userId)
            {
                throw new UserFriendlyException("You are not creator of this mindfight!");
            }

            var questionAnswers = new List<MindfightQuestionAnswerDto>();
            var answers = await _questionAnswerRepository.GetAll()
                .Where(x => x.QuestionId == questionId)
                .ToListAsync();

            foreach (var answer in answers)
            {
                var answerDto = new MindfightQuestionAnswerDto();
                answer.MapTo(answerDto);
                questionAnswers.Add(answerDto);
            }
            return questionAnswers;
        }

        public async Task<MindfightQuestionAnswerDto> GetQuestionAnswer(long answerId, long userId)
        {
            var currentAnswer = await _questionAnswerRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == answerId);
            if (currentAnswer == null)
            {
                throw new UserFriendlyException("Answer with specified id does not exist!");
            }

            var currentQuestion = await _mindfightQuestionRepository
                .GetAllIncluding(x => x.Mindfight)
                .FirstOrDefaultAsync(x => x.Id == currentAnswer.QuestionId);
            if (currentQuestion == null)
            {
                throw new UserFriendlyException("Question with specified id does not exist!");
            }

            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new UserFriendlyException("User does not exist!");
            }

            if (currentQuestion.Mindfight.CreatorId != userId)
            {
                throw new UserFriendlyException("You are not creator of this mindfight!");
            }
            var currentAnswerDto = new MindfightQuestionAnswerDto();
            currentAnswer.MapTo(currentAnswerDto);
            return currentAnswerDto;
        }

        public async Task<long> CreateQuestionAnswer(MindfightQuestionAnswerDto answer, long questionId, long userId)
        {
            var currentQuestion = await _mindfightQuestionRepository.GetAllIncluding(x => x.Mindfight).FirstOrDefaultAsync(x => x.Id == questionId);
            if (currentQuestion == null)
            {
                throw new UserFriendlyException("Question with specified id does not exist!");
            }

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new UserFriendlyException("User does not exist!");
            }

            if (currentQuestion.Mindfight.CreatorId != userId)
            {
                throw new UserFriendlyException("You are not creator of this mindfight!");
            }

            var answerToCreate = new MindfightQuestionAnswer(currentQuestion, answer.Answer, answer.IsCorrect);
            return await _questionAnswerRepository.InsertAndGetIdAsync(answerToCreate);
        }

        public async Task UpdateQuestionAnswer(MindfightQuestionAnswerDto answer, long answerId, long userId)
        {
            var currentAnswer = await _questionAnswerRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == answerId);
            if (currentAnswer == null)
            {
                throw new UserFriendlyException("Answer with specified id does not exist!");
            }

            var currentQuestion = await _mindfightQuestionRepository
                .GetAllIncluding(x => x.Mindfight)
                .FirstOrDefaultAsync(x => x.Id == currentAnswer.QuestionId);
            if (currentQuestion == null)
            {
                throw new UserFriendlyException("Question with specified id does not exist!");
            }

            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new UserFriendlyException("User does not exist!");
            }

            if (currentQuestion.Mindfight.CreatorId != userId)
            {
                throw new UserFriendlyException("You are not creator of this mindfight!");
            }

            currentAnswer.Answer = answer.Answer;
            currentAnswer.IsCorrect = answer.IsCorrect;
            await _questionAnswerRepository.UpdateAsync(currentAnswer);
        }

        public async Task DeleteQuestionAnswer(long answerId, long userId)
        {
            var currentAnswer = await _questionAnswerRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == answerId);
            if (currentAnswer == null)
            {
                throw new UserFriendlyException("Answer with specified id does not exist!");
            }

            var currentQuestion = await _mindfightQuestionRepository
                .GetAllIncluding(x => x.Mindfight)
                .FirstOrDefaultAsync(x => x.Id == currentAnswer.QuestionId);
            if (currentQuestion == null)
            {
                throw new UserFriendlyException("Question with specified id does not exist!");
            }

            var user = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new UserFriendlyException("User does not exist!");
            }

            if (currentQuestion.Mindfight.CreatorId != userId)
            {
                throw new UserFriendlyException("You are not creator of this mindfight!");
            }
            
            await _questionAnswerRepository.DeleteAsync(currentAnswer);
        }
    }
}