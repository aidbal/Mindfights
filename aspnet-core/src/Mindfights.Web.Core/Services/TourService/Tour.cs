using Abp.AspNetCore.Mvc.Authorization;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Mindfights.Authorization.Users;
using Mindfights.DTOs;
using Mindfights.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.ObjectMapping;

namespace Mindfights.Services.TourService
{
    [AbpMvcAuthorize]
    public class Tour : ITourService
    {
        private readonly IRepository<Mindfight, long> _mindfightRepository;
        private readonly IRepository<Models.Tour, long> _tourRepository;
        private readonly IRepository<Team, long> _teamRepository;
        private readonly IRepository<Question, long> _questionRepository;
        private readonly IPermissionChecker _permissionChecker;
        private readonly UserManager _userManager;
        private readonly IObjectMapper _objectMapper;
        private const int BufferSeconds = 2;

        public Tour(
            IRepository<Mindfight, long> mindfightRepository,
            IRepository<Models.Tour, long> tourRepository,
            IRepository<Team, long> teamRepository,
            IRepository<Question, long> questionRepository,
            IPermissionChecker permissionChecker,
            UserManager userManager,
            IObjectMapper objectMapper
            )
        {
            _mindfightRepository = mindfightRepository;
            _tourRepository = tourRepository;
            _teamRepository = teamRepository;
            _questionRepository = questionRepository;
            _permissionChecker = permissionChecker;
            _userManager = userManager;
            _objectMapper = objectMapper;
        }

        public async Task<List<TourDto>> GetAllMindfightTours(long mindfightId)
        {
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("Vartotojas neegzistuoja!");

            var currentMindfight = await _mindfightRepository
                .GetAllIncluding(x => x.Tours, x => x.Evaluators)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);

            if (currentMindfight == null)
                throw new UserFriendlyException("Protmūšis su nurodytu id neegzistuoja!");

            if (!(currentMindfight.CreatorId == _userManager.AbpSession.UserId
                || _permissionChecker.IsGranted("ManageMindfights")
                || currentMindfight.Evaluators.Any(x => x.UserId == _userManager.AbpSession.UserId)))
                throw new AbpAuthorizationException("jūs neturite visų turų gavimo teisių");

            var toursDto = new List<TourDto>();
            var tours = await _tourRepository
                .GetAllIncluding(tour => tour.Questions)
                .Where(x => x.MindfightId == mindfightId)
                .OrderBy(x => x.OrderNumber)
                .ToListAsync();

            foreach (var tour in tours)
            {
                var tourDto = new TourDto();
                _objectMapper.Map(tour, tourDto);
                tourDto.QuestionsCount = tour.Questions.Count;
                toursDto.Add(tourDto);
            }
            if (toursDto.Count > 0)
            {
                toursDto[toursDto.Count - 1].IsLastTour = true;
            }
            return toursDto;
        }

        public async Task<TourDto> GetTour(long tourId)
        {
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("Vartotojas neegzistuoja!");

            var currentTour = await _tourRepository
                .GetAll()
                .Include(tour => tour.Questions)
                .Include(tour => tour.Mindfight)
                .ThenInclude(tour => tour.Evaluators)
                .FirstOrDefaultAsync(tour => tour.Id == tourId);

            if (currentTour == null)
                throw new UserFriendlyException("Turas su nurodytu id neegzistuoja!");

            if (!(currentTour.Mindfight.CreatorId == _userManager.AbpSession.UserId
                || _permissionChecker.IsGranted("ManageMindfights")
                || currentTour.Mindfight.Evaluators.Any(x => x.UserId == _userManager.AbpSession.UserId)))
                throw new AbpAuthorizationException("Jūs neturite teisių gauti šiam turui!");

            var tourDto = new TourDto();
            _objectMapper.Map(currentTour, tourDto);
            tourDto.QuestionsCount = currentTour.Questions.Count;
            tourDto.TotalPoints = currentTour.Questions.Sum(question => question.Points);
            return tourDto;
        }

        public async Task<TourDto> GetNextTour(long mindfightId, long teamId)
        {
            var currentMindfight = await _mindfightRepository
                .GetAllIncluding(mindfight => mindfight.MindfightStates)
                .FirstOrDefaultAsync(mindfight => mindfight.Id == mindfightId);

            if (currentMindfight == null)
            {
                throw new UserFriendlyException("Protmūšis su nurodytu id neegzistuoja!");
            }

            var currentTeam = await _teamRepository
                .FirstOrDefaultAsync(team => team.Id == teamId);

            if (currentTeam == null)
            {
                throw new UserFriendlyException("Komanda su nurodytu id neegzistuoja!");
            }

            if (currentMindfight.Registrations.Any(x => x.TeamId != teamId && x.IsConfirmed))
            {
                throw new UserFriendlyException("Komandos registracija nėra patvirtinta!");
            }
            if (!(currentMindfight.IsConfirmed || currentMindfight.StartTime > Clock.Now))
            {
                throw new UserFriendlyException("Protmūšis dar neprasidėjo!");
            }

            var teamMindfightState = currentMindfight.MindfightStates
                .FirstOrDefault(state => state.MindfightId == mindfightId && state.TeamId == teamId);

            Models.Tour currentTour;
            int nextTourOrderNumber;
            var addNewMindfightState = false;

            if (teamMindfightState == null)
            {
                currentTour = await GetFirstMindfightTour(mindfightId);
                if (currentTour == null)
                {
                    throw new UserFriendlyException("Problema gaunant turą iš protmūšio statuso!");
                }
                nextTourOrderNumber = currentTour.OrderNumber;
                addNewMindfightState = true;
            }
            else
            {
                currentTour = await _tourRepository
                    .FirstOrDefaultAsync(tour => tour.Id == teamMindfightState.CurrentTourId);
                if (currentTour == null)
                {
                    throw new UserFriendlyException("Problema gaunant turą");
                }

                nextTourOrderNumber = currentTour.OrderNumber;

                var lastQuestionTime = await GetLastQuestionTime(currentTour);
                var tourTotalTime = lastQuestionTime + currentTour.TimeToEnterAnswersInSeconds;

                if ((Clock.Now - teamMindfightState.ChangeTime).TotalSeconds >
                    tourTotalTime - BufferSeconds)
                {
                    nextTourOrderNumber += 1;
                }
            }

            var mindfightTours = await _tourRepository
                .GetAllIncluding(tour => tour.Questions)
                .OrderBy(x => x.OrderNumber)
                .Where(x => x.MindfightId == mindfightId && x.OrderNumber >= nextTourOrderNumber)
                .ToListAsync();

            if (mindfightTours.Count == 0)
            {
                throw new UserFriendlyException("Daugiau nėra jokių turų!");
            }

            var tourToReturn = mindfightTours.First();
            if (addNewMindfightState)
            {
                var newTeamMindfightState = new MindfightState(currentMindfight, currentTeam)
                {
                    ChangeTime = Clock.Now,
                    CurrentTourId = currentTour.Id,
                };
                currentMindfight.MindfightStates.Add(newTeamMindfightState);
            }
            else if (currentTour.Id != tourToReturn.Id)
            {
                currentMindfight.MindfightStates.Remove(teamMindfightState);
                teamMindfightState.CurrentTourId = tourToReturn.Id;
                teamMindfightState.CurrentQuestionId = null;
                teamMindfightState.ChangeTime = Clock.Now;
                currentMindfight.MindfightStates.Add(teamMindfightState);
            }

            var tourDto = new TourDto();
            _objectMapper.Map(tourToReturn, tourDto);
            tourDto.QuestionsCount = tourToReturn.Questions.Count;

            if (mindfightTours.Count == 1)
            {
                tourDto.IsLastTour = true;
            }

            return tourDto;
        }

        public async Task<long> CreateTour(TourDto tour, long mindfightId)
        {
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("Vartotojas neegzistuoja!");

            var currentMindfight = await _mindfightRepository
                .FirstOrDefaultAsync(x => x.Id == mindfightId);

            if (currentMindfight == null)
                throw new UserFriendlyException("Protmūšis su nurodytu id neegzistuoja!");

            if (!(currentMindfight.CreatorId == _userManager.AbpSession.UserId
                || _permissionChecker.IsGranted("ManageMindfights")))
                throw new AbpAuthorizationException("Jūs neturite turo kūrimo teisių!");

            tour.OrderNumber = await GetLastOrderNumber(mindfightId);
            tour.OrderNumber = tour.OrderNumber == 0 ? 1 : tour.OrderNumber + 1;

            var tourToCreate = new Models.Tour(
                currentMindfight,
                tour.Title,
                tour.Description,
                tour.TimeToEnterAnswersInSeconds,
                tour.IntroTimeInSeconds,
                tour.OrderNumber);
            return await _tourRepository.InsertAndGetIdAsync(tourToCreate);
        }

        public async Task UpdateTour(TourDto tour, long tourId)
        {
            var currentTour = await _tourRepository
                .GetAll()
                .Include(x => x.Mindfight)
                .FirstOrDefaultAsync(x => x.Id == tourId);
            if (currentTour == null)
                throw new UserFriendlyException("Turas su nurodytu id neegzistuoja!");

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("Vartotojas neegzistuoja!");

            if (!(currentTour.Mindfight.CreatorId == _userManager.AbpSession.UserId
                || _permissionChecker.IsGranted("ManageMindfights")))
                throw new AbpAuthorizationException("Jūs neturite turo redagavimo teisių!");

            tour.OrderNumber = currentTour.OrderNumber;
            _objectMapper.Map(tour, currentTour);
            currentTour.Id = tourId;
            await _tourRepository.UpdateAsync(currentTour);
        }

        public async Task DeleteTour(long tourId)
        {
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("Vartotojas neegzistuoja!");

            var tourToDelete = await _tourRepository.FirstOrDefaultAsync(x => x.Id == tourId);
            if (tourToDelete == null)
                throw new UserFriendlyException("Turas su nurodytu id neegzistuoja!");

            var currentMindfight = await _mindfightRepository
                .FirstOrDefaultAsync(x => x.Id == tourToDelete.MindfightId);

            if (currentMindfight == null)
                throw new UserFriendlyException("Protmūšis su nurodytu id neegzistuoja!");

            if (!(currentMindfight.CreatorId == _userManager.AbpSession.UserId
                || _permissionChecker.IsGranted("ManageMindfights")))
                throw new AbpAuthorizationException("Jūs neturite turo redagavimo teisių!");
            
            var orderNumber = tourToDelete.OrderNumber;
            await UpdateOrderNumbers(orderNumber, tourToDelete.Id, currentMindfight.Id);
            await _tourRepository.DeleteAsync(tourToDelete);
        }

        public async Task UpdateOrderNumber(long tourId, int newOrderNumber)
        {
            var currentTour = await _tourRepository.FirstOrDefaultAsync(x => x.Id == tourId);
            if (currentTour == null)
                throw new UserFriendlyException("Turas su nurodytu id neegzistuoja!");

            var currentMindfight = await _mindfightRepository
                .FirstOrDefaultAsync(x => x.Id == currentTour.MindfightId);

            if (currentMindfight == null)
                throw new UserFriendlyException("Protmūšis su nurodytu id neegzistuoja!");

            if (!(currentMindfight.CreatorId == _userManager.AbpSession.UserId
                || _permissionChecker.IsGranted("ManageMindfights")))
                throw new AbpAuthorizationException("Jūs neturite turo redagavimo teisių!");

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _userManager.AbpSession.UserId);
            if (user == null)
                throw new UserFriendlyException("Vartotojas neegzistuoja!");

            var tourWithNewOrderNumber = await _tourRepository
                .FirstOrDefaultAsync(x => x.MindfightId == currentTour.MindfightId && x.OrderNumber == newOrderNumber);

            if (tourWithNewOrderNumber != null)
            {
                tourWithNewOrderNumber.OrderNumber = currentTour.OrderNumber;
                currentTour.OrderNumber = newOrderNumber;
            }
        }

        private async Task UpdateOrderNumbers(int deletedOrderNumber, long deletedTourId, long mindfightId)
        {
            var tours = await _tourRepository
                .GetAll()
                .Where(x => x.MindfightId == mindfightId && x.Id != deletedTourId)
                .OrderBy(x => x.OrderNumber)
                .ToListAsync();

            var nextOrderNumber = deletedOrderNumber;
            foreach (var tour in tours)
            {
                if (tour.OrderNumber < deletedOrderNumber) continue;
                tour.OrderNumber = nextOrderNumber;
                nextOrderNumber++;
            }
        }

        private async Task<int> GetLastOrderNumber(long mindfightId)
        {
            var lastOrderNumber = 0;
            var lastTour = await _tourRepository
                .GetAll()
                .Where(x => x.MindfightId == mindfightId)
                .OrderByDescending(x => x.OrderNumber)
                .FirstOrDefaultAsync();

            if (lastTour != null)
            {
                lastOrderNumber = lastTour.OrderNumber;
            }

            return lastOrderNumber;
        }

        private async Task<Models.Tour> GetFirstMindfightTour(long mindfightId)
        {
            var mindfightTours = await _tourRepository
                .GetAll()
                .OrderBy(x => x.OrderNumber)
                .Where(tour => tour.MindfightId == mindfightId)
                .ToListAsync();
            if (mindfightTours.Count < 1)
            {
                throw new UserFriendlyException("Protmūšis neturi turų!");
            }
            return mindfightTours.First();
        }

        private async Task<int> GetLastQuestionTime(Models.Tour tour)
        {
            var lastTourQuestion = await _questionRepository
                .GetAll()
                .OrderByDescending(question => question.OrderNumber)
                .Where(question => question.TourId == tour.Id)
                .FirstOrDefaultAsync();

            if (lastTourQuestion == null)
            {
                throw new UserFriendlyException("Turas neturi klausimų!");
            }

            var questionTime = lastTourQuestion.TimeToAnswerInSeconds;
            return questionTime;
        }
    }
}
