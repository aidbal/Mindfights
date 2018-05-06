using Abp.Application.Services;
using Mindfights.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mindfights.Services.MindfightService
{
    public interface IMindfightService : IApplicationService
    {
        Task<MindfightDto> GetMindfight(long mindfightId);
        Task<long> CreateMindfight(MindfightCreateUpdateDto mindfight);
        Task UpdateMindfight(MindfightDto mindfight);
        Task DeleteMindfight(long mindfightId);
        Task<List<MindfightDto>> GetMyCreatedMindfights();
        Task<List<MindfightDto>> GetAllowedToEvaluateMindfights();
        Task<List<MindfightDto>> GetRegisteredMindfights();
        Task<List<MindfightPublicDto>> GetUpcomingMindfights();
        Task UpdateActiveStatus(long mindfightId, bool isActive);
        Task UpdateConfirmedStatus(long mindfightId, bool isConfirmed);
        Task UpdateFinishedStatus(long mindfightId, bool isFinished);
    }
}
