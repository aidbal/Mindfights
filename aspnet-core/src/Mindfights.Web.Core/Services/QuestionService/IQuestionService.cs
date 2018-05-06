using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Mindfights.DTOs;

namespace Mindfights.Services.QuestionService
{
    public interface IQuestionService : IApplicationService
    {
        Task<List<QuestionDto>> GetAllTourQuestions(long tourId);
        Task<QuestionDto> GetQuestion(long questionId);
        Task<QuestionDto> GetNextQuestion(long mindfightId, long teamId);
        Task<long> CreateQuestion(QuestionDto question, long tourId);
        Task UpdateQuestion(QuestionDto question, long questionId);
        Task DeleteQuestion(long questionId);
        Task UpdateOrderNumber(long questionId, int newOrderNumber);
    }
}
