using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Skautatinklis.Authorization.Users;
using Skautatinklis.DTOs;
using Skautatinklis.Models;

namespace Skautatinklis.Services.TourService
{
    public class TourService : ITourService
    {

        private readonly IRepository<Mindfight, long> _mindfightRepository;
        private readonly IRepository<Tour, long> _tourRepository;
        private readonly IRepository<Team, long> _teamRepository;
        private readonly IRepository<TeamAnswer, long> _teamAnswerRepository;
        private readonly UserManager _userManager;

        public TourService(
            IRepository<Mindfight, long> mindfightRepository,
            IRepository<Tour, long> tourRepository,
            IRepository<Team, long> teamRepository,
            IRepository<TeamAnswer, long> teamAnswerRepository,
            UserManager userManager)
        {
            _mindfightRepository = mindfightRepository;
            _tourRepository = tourRepository;
            _teamRepository = teamRepository;
            _teamAnswerRepository = teamAnswerRepository;
            _userManager = userManager;
        }

        public async Task<List<TourDto>> GetAllMindfightTours(long mindfightId, long userId)
        {
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            var currentMindfight = await _mindfightRepository
                .GetAllIncluding(x => x.Tours, x => x.Evaluators)
                .FirstOrDefaultAsync(x => x.Id == mindfightId);

            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            if (currentMindfight.CreatorId != userId || currentMindfight.Evaluators.All(x => x.UserId != userId))
                throw new UserFriendlyException("You are not creator of this mindfight!");

            var toursDto = new List<TourDto>();
            var tours = await _tourRepository
                .GetAll()
                .Where(x => x.MindfightId == mindfightId)
                .ToListAsync();

            foreach (var tour in tours)
            {
                var tourDto = new TourDto();
                tour.MapTo(tourDto);
                toursDto.Add(tourDto);
            }
            return toursDto;
        }

        public async Task<TourDto> GetTour(long tourId, long userId)
        {
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            var currentTour = await _tourRepository
                .GetAll()
                .Include(x => x.Mindfight)
                .ThenInclude(x => x.Evaluators)
                .FirstOrDefaultAsync(x => x.Id == tourId);

            if (currentTour == null)
                throw new UserFriendlyException("Tour with specified id does not exist!");

            if (currentTour.Mindfight.CreatorId != userId || currentTour.Mindfight.Evaluators.All(x => x.UserId != userId))
                throw new UserFriendlyException("Insufficient permissions to get this tour!");

            var tour = new TourDto();
            currentTour.MapTo(tour);
            return tour;
        }

        public async Task<TourDto> GetNextTour(long mindfightId, long teamId, long userId)
        {
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            //var currentTour = await _tourRepository.FirstOrDefaultAsync(x => x.Id == tourId);
            //if (currentTour == null)
            //    throw new UserFriendlyException("Tour with specified id does not exist!");

            //if (currentMindfight.StartTime.AddMinutes(currentMindfight.PrepareTime ?? 0) > Clock.Now.AddMinutes(-1))
            //    throw new UserFriendlyException("Mindfight has not yet started!");

            //if (currentMindfight.EndTime > Clock.Now)
            //    throw new UserFriendlyException("Mindfight is already over!");

            var currentTeam = await _teamRepository
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == teamId);
            if (currentTeam == null)
                throw new UserFriendlyException("Team with specified id does not exist!");

            var teamMindfightAnswers = await _teamAnswerRepository
                .GetAll()
                .Include(x => x.Question)
                .ThenInclude(x => x.Tour)
                .OrderByDescending(x => x.Question.Tour.OrderNumber)
                .Where(x => x.TeamId == teamId && x.Question.Tour.MindfightId == mindfightId)
                .ToListAsync();

            var nextTourOrderNumber = 1;

            if (teamMindfightAnswers.Count > 0)
                nextTourOrderNumber = teamMindfightAnswers[0].Question.Tour.OrderNumber + 1;

            var tours = await _tourRepository
                .GetAll()
                .OrderBy(x => x.OrderNumber)
                .Where(x => x.MindfightId == mindfightId && x.OrderNumber >= nextTourOrderNumber)
                .ToListAsync();

            if (tours.Count == 0)
                throw new UserFriendlyException("There are no more questions left to answer!");

            var currentTour = tours.First();

            var tourDto = new TourDto();
            currentTour.MapTo(tourDto);

            if (tours.Count == 1)
                tourDto.IsLastTour = true;

            return tourDto;
        }

        public async Task<long> CreateTour(TourDto tour, long mindfightId, long userId)
        {
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            var currentMindfight = await _mindfightRepository
                .FirstOrDefaultAsync(x => x.Id == mindfightId);

            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with specified id does not exist!");

            if (currentMindfight.CreatorId != userId)
                throw new UserFriendlyException("You are not creator of this mindfight!");

            tour.OrderNumber = await GetLastOrderNumber(mindfightId);
            tour.OrderNumber = tour.OrderNumber == 0 ? 1 : tour.OrderNumber + 1;
            currentMindfight.ToursCount += 1;

            var tourToCreate = new Tour(
                currentMindfight,
                tour.Title,
                tour.Description,
                tour.TimeToEnterAnswersInSeconds,
                tour.OrderNumber);
            return await _tourRepository.InsertAndGetIdAsync(tourToCreate);
        }

        public async Task UpdateTour(TourDto tour, long tourId, long userId)
        {
            var currentTour = await _tourRepository
                .GetAll()
                .Include(x => x.Mindfight)
                .FirstOrDefaultAsync(x => x.Id == tourId);
            if (currentTour == null)
                throw new UserFriendlyException("Tour with specified id does not exist!");

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            if (currentTour.Mindfight.CreatorId != userId)
                throw new UserFriendlyException("You are not creator of this mindfight!");

            tour.OrderNumber = currentTour.OrderNumber;
            tour.MapTo(currentTour);
            currentTour.Id = tourId;
            await _tourRepository.UpdateAsync(currentTour);
        }

        public async Task DeleteTour(long tourId, long userId)
        {
            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

            var tourToDelete = await _tourRepository.FirstOrDefaultAsync(x => x.Id == tourId);
            if (tourToDelete == null)
                throw new UserFriendlyException("Tour with specified id does not exist!");

            var currentMindfight = await _mindfightRepository
                .FirstOrDefaultAsync(x => x.Id == tourToDelete.MindfightId);

            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with id specified in tour does not exist!");

            if (currentMindfight.CreatorId != userId)
                throw new UserFriendlyException("You are not creator of this mindfight!");

            currentMindfight.ToursCount -= 1;
            var orderNumber = tourToDelete.OrderNumber;
            await UpdateOrderNumbers(orderNumber, tourToDelete.Id, currentMindfight.Id);
            await _tourRepository.DeleteAsync(tourToDelete);
        }

        public async Task UpdateOrderNumber(long tourId, long userId, int newOrderNumber)
        {
            var currentTour = await _tourRepository.FirstOrDefaultAsync(x => x.Id == tourId);
            if (currentTour == null)
                throw new UserFriendlyException("Tour with specified id does not exist!");

            var currentMindfight = await _mindfightRepository
                .FirstOrDefaultAsync(x => x.Id == currentTour.MindfightId);

            if (currentMindfight == null)
                throw new UserFriendlyException("Mindfight with id specified in tour does not exist!");

            if (currentMindfight.CreatorId != userId)
                throw new UserFriendlyException("You are not creator of this mindfight!");

            var user = await _userManager.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new UserFriendlyException("User does not exist!");

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
    }
}
