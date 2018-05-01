using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.UI;
using Abp.Zero.Configuration;
using Microsoft.EntityFrameworkCore;
using Mindfights.Authorization.Accounts.Dto;
using Mindfights.Authorization.Users;
using Mindfights.Models;

namespace Mindfights.Authorization.Accounts
{
    public class AccountAppService : MindfightsAppServiceBase, IAccountAppService
    {
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly IRepository<City, long> _cityRepository;

        public AccountAppService(
            UserRegistrationManager userRegistrationManager,
            IRepository<City, long> cityRepository
            )
        {
            _userRegistrationManager = userRegistrationManager;
            _cityRepository = cityRepository;
        }

        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            var tenant = await TenantManager.FindByTenancyNameAsync(input.TenancyName);
            if (tenant == null)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            }

            if (!tenant.IsActive)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
            }

            return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id);
        }

        public async Task<RegisterOutput> Register(RegisterInput input)
        {
            var city = await _cityRepository.FirstOrDefaultAsync(c => c.Id == input.CityId);
            if (city == null)
            {
                throw new UserFriendlyException("Nurodytas miestas neegzistuoja!");
            }

            var user = await _userRegistrationManager.RegisterAsync(
                input.Name,
                input.Surname,
                input.EmailAddress,
                input.Birthdate,
                city,
                input.UserName,
                input.Password,
                true // Assumed email address is always confirmed. Change this if you want to implement email confirmation.
            );

            var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);

            return new RegisterOutput
            {
                CanLogin = user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin)
            };
        }

        public async Task<List<City>> GetRegistrationCities()
        {
            var registrationCities = await _cityRepository
                .GetAll()
                .OrderBy(city => city.Name)
                .ToListAsync();
            return registrationCities;
        }
    }
}
