using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Skautatinklis.DTOs;

namespace Skautatinklis.Services.MindfightService
{
    public interface IMindfightService : IApplicationService
    {
        //Task<MindfightDto> GetMindfight(long mindfightId, long userId);
        //Task<long> CreateMindfight(MindfightCreateUpdateDto mindfight, long creatorId);
        //Task UpdateMindfight(MindfightDto mindfight, long mindfightId, long userId);
        //Task DeleteMindfight(long userId, long mindfightId);
        //Task<List<MindfightPublicDto>> GetUpcomingPublicMindfights();
        ////Task<List<MindfightPrivateDto>> GetUpcomingPrivateMindfights(long userId);
        //Task UpdateEvaluators(long userId, long mindfightId, List<string> evaluatorEmails);
        ////Task UpdateAllowedTeams(long userId, long mindfightId, List<string> allowedTeamNames);
        //Task UpdateActiveStatus(long userId, long mindfightId, bool isActive);
        //Task UpdateConfirmedStatus(long userId, long mindfightId, bool isConfirmed);
        //Task UpdateFinishedStatus(long userId, long mindfightId, bool isFinished);
    }
}
