using Abp.Application.Services;
using Mindfights.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mindfights.Services.ResultService
{
    public interface IResultService : IApplicationService
    {
        Task UpdateResult(long mindfightId, long teamId);
        Task<MindfightResultDto> GetMindfightTeamResult(long mindfightId, long teamId);
        Task<List<MindfightResultDto>> GetMindfightResults(long mindfightId);
        Task<List<MindfightResultDto>> GetTeamResults(long teamId);
        Task<List<MindfightResultDto>> GetUserResults(long userId);
    }
}
