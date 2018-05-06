using Abp.Application.Services;
using Mindfights.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mindfights.Services.RegistrationService
{
    public interface IRegistrationService : IApplicationService
    {
        Task<long> CreateRegistration(long mindfightId, long teamId);
        Task DeleteRegistration(long mindfightId, long teamId);
        Task<List<RegistrationDto>> GetTeamRegistrations(long teamId);
        Task<RegistrationDto> GetRegistration(long registrationId);
        Task<RegistrationDto> GetMindfightTeamRegistration(long mindfightId, long teamId);
        Task<List<RegistrationDto>> GetMindfightRegistrations(long mindfightId);
        Task UpdateConfirmation(long mindfightId, long teamId, bool isConfirmed);
    }
}
