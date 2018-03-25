using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.ObjectMapping;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Skautatinklis.Authorization.Users;
using Skautatinklis.DTOs;
using Skautatinklis.Models;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Skautatinklis.Services.ScoutGroupService
{
    public class ScoutGroupService : IScoutGroupService
    {
        private readonly IRepository<ScoutGroup, long> _scoutGroupRepository;
        private readonly UserManager _userManager;
        private readonly IObjectMapper _objectMapper;

        public ScoutGroupService(IRepository<ScoutGroup, long> scoutGroupRepository, UserManager userManager, IObjectMapper objectMapper)
        {
            _scoutGroupRepository = scoutGroupRepository;
            _userManager = userManager;
            _objectMapper = objectMapper;
        }

        public async Task<long> CreateScoutGroup(long? leaderId, ScoutGroupDto scoutGroup)
        {
            if (scoutGroup.Name.IsNullOrWhiteSpace())
            {
                throw new UserFriendlyException("Scout group name was not entered.");
            }
            var userToSelectId = leaderId ?? _userManager.AbpSession.UserId;
            var user = _userManager.Users.IgnoreQueryFilters().FirstOrDefault(u => u.Id == userToSelectId);
            if (user == null)
            {
                throw new UserFriendlyException("User does not exist.");
            }
            var currentUserScoutGroup = await _scoutGroupRepository.FirstOrDefaultAsync(x => x.LeaderId == user.Id);
            if (currentUserScoutGroup != null)
            {
                throw new UserFriendlyException("User already has created a scout group!");
            }
            var scoutGroupWithSameName = _scoutGroupRepository.FirstOrDefaultAsync(x => x.Name == scoutGroup.Name);
            if (scoutGroupWithSameName != null)
            {
                throw new UserFriendlyException("Scout group with the same name already exists");
            }
            var scoutGroupToInsert = new ScoutGroup(user, scoutGroup.Name, scoutGroup.Description);
            return await _scoutGroupRepository.InsertAndGetIdAsync(scoutGroupToInsert);
        }

        public async Task<ScoutGroupDto> GetScoutGroup(long scoutGroupId)
        {
            var scoutGroup = await _scoutGroupRepository.FirstOrDefaultAsync(x => x.Id == scoutGroupId);
            if (scoutGroup == null)
            {
                throw new UserFriendlyException("Specified scout group does not exist or is deleted!");
            }
            ScoutGroupDto scoutGroupDto = new ScoutGroupDto();
            scoutGroup.MapTo(scoutGroupDto);
            scoutGroupDto.LeaderName = _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == scoutGroup.LeaderId).Result.Name;
            scoutGroupDto.UsersCount = scoutGroup.UsersCount;
            return scoutGroupDto;
        }

        public async Task<List<ScoutGroupDto>> GetAllScoutGroups()
        {
            var scoutGroups = await _scoutGroupRepository.GetAll().ToListAsync();
            var scoutGroupsDto = new List<ScoutGroupDto>();
            _objectMapper.Map(scoutGroups, scoutGroupsDto);
            for (var i = 0; i < scoutGroups.Count; i++)
            {
                scoutGroupsDto[i].LeaderName = _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == scoutGroups[i].LeaderId).Result.Name;
            }
            return scoutGroupsDto;
        }

        public async Task UpdateScoutGroup(ScoutGroupDto scoutGroup, long scoutGroupId)
        {
            var currentScoutGroup = await _scoutGroupRepository.FirstOrDefaultAsync(x => x.Id == scoutGroupId);
            if (currentScoutGroup == null)
            {
                throw new UserFriendlyException("Specified scout group does not exist!");
            }
            var scoutGroupWithSameName = await _scoutGroupRepository.FirstOrDefaultAsync(x => x.Name == scoutGroup.Name);
            if (scoutGroupWithSameName != null)
            {
                throw new UserFriendlyException("Scout group with the same name already exists!");
            }
            currentScoutGroup.Description = currentScoutGroup.Description;
            currentScoutGroup.Name = currentScoutGroup.Name;
            await _scoutGroupRepository.UpdateAsync(currentScoutGroup);
        }

        public async Task DeleteScoutGroup(long scoutGroupId)
        {
            var currentScoutGroup = await _scoutGroupRepository.GetAllIncluding(x => x.Users).FirstOrDefaultAsync(x => x.Id == scoutGroupId);
            if (currentScoutGroup == null)
            {
                throw new UserFriendlyException("Specified scout group does not exist or is deleted!");
            }
            foreach (var user in currentScoutGroup.Users)
            {
                user.ScoutGroup = null;
            }
            await _scoutGroupRepository.DeleteAsync(currentScoutGroup);
        }

        public async Task InsertUser(long scoutGroupId, long userId)
        {
            var currentScoutGroup = await _scoutGroupRepository.GetAllIncluding(x => x.Users).FirstOrDefaultAsync(x => x.Id == scoutGroupId);
            if (currentScoutGroup == null)
            {
                throw new UserFriendlyException("Specified scout group does not exist or is deleted!");
            }
            var currentLeaderScoutGroup = await _scoutGroupRepository.FirstOrDefaultAsync(x => x.LeaderId == userId);
            if (currentLeaderScoutGroup != null)
            {
                throw new UserFriendlyException("User already has created a scout group!");
            }
            var currentUser = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == userId);
            if (currentUser == null)
            {
                throw new UserFriendlyException("User does not exist or is deleted!");
            }
            if (currentUser.ScoutGroup != null)
            {
                throw new UserFriendlyException("User already has a scout group!");
            }
            currentScoutGroup.Users.Add(currentUser);
            currentScoutGroup.UsersCount += 1;
            await _scoutGroupRepository.UpdateAsync(currentScoutGroup);
        }

        public async Task RemoveUser(long scoutGroupId, long userId)
        {
            var currentScoutGroup = await _scoutGroupRepository.GetAllIncluding(x => x.Users).FirstOrDefaultAsync(x => x.Id == scoutGroupId);
            if (currentScoutGroup == null)
            {
                throw new UserFriendlyException("Specified scout group does not exist or is deleted!");
            }
            var currentLeaderScoutGroup = await _scoutGroupRepository.FirstOrDefaultAsync(x => x.LeaderId == userId);
            if (currentLeaderScoutGroup != null)
            {
                throw new UserFriendlyException("User is leader of the scout group!");
            }
            var currentUser = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == userId);
            if (currentUser == null)
            {
                throw new UserFriendlyException("User does not exist or is deleted!");
            }
            if (currentUser.ScoutGroup == null)
            {
                throw new UserFriendlyException("User does not have any scout group!");
            }
            if (currentScoutGroup.Users.Remove(currentUser))
            {
                currentScoutGroup.UsersCount -= 1;
                await _scoutGroupRepository.UpdateAsync(currentScoutGroup);
            }
        }

        public async Task<List<string>> GetAllUsers(long scoutGroupId)
        {
            var currentScoutGroup = await _scoutGroupRepository.GetAllIncluding(x => x.Users).FirstOrDefaultAsync(x => x.Id == scoutGroupId);
            if (currentScoutGroup == null)
            {
                throw new UserFriendlyException("Specified scoutGroup does not exist or is deleted!");
            }
            var usersInScoutGroup = await _userManager.Users.IgnoreQueryFilters().Where(x => x.ScoutGroupId == scoutGroupId).ToListAsync();
            return usersInScoutGroup.Select(user => user.Name).ToList();
        }
    }
}
