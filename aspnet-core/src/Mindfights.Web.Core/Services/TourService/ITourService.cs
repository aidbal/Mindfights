using Abp.Application.Services;
using Mindfights.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mindfights.Services.TourService
{
    public interface ITourService : IApplicationService
    {
        Task<List<TourDto>> GetAllMindfightTours(long mindfightId);
        Task<TourDto> GetTour(long tourId);
        Task<TourDto> GetNextTour(long mindfightId, long teamId);
        Task<long> CreateTour(TourDto tour, long mindfightId);
        Task UpdateTour(TourDto tour, long tourId);
        Task DeleteTour(long tourId);
        Task UpdateOrderNumber(long tourId, int newOrderNumber);
    }
}
