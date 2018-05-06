using Abp.Application.Services;
using Mindfights.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mindfights.Services.TeamAnswerService
{
    public interface ITeamAnswerService : IApplicationService
    {
        Task<List<long>> CreateTeamAnswer(List<TeamAnswerDto> teamAnswers, long mindfightId);
        Task<TeamAnswerDto> GetTeamAnswer(long questionId, long teamId);
        Task<List<TeamAnswerDto>> GetAllTeamAnswers(long mindfightId, long teamId);
        Task UpdateIsEvaluated(long questionId, long teamId,
            string evaluatorComment, int earnedPoints, bool isEvaluated);
    }
}
