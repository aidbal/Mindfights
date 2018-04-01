using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Mindfights.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Abp.Authorization;
using Abp.Runtime.Session;

namespace Mindfights.Services
{
    [AbpAuthorize()]
    public class MindfightsService
    {
        private readonly IRepository<Mindfight, long> _mindfightRepository;


        public MindfightsService(IRepository<Mindfight, long> mindfightRepository)
        {
            _mindfightRepository = mindfightRepository;
        }

        [HttpGet]
        public async Task<List<Mindfight>> GetAllPublicMindfights()
        {
            var currentUserId = NullAbpSession.Instance.UserId;
            var tasks = await _mindfightRepository
                .GetAll()
                .OrderByDescending(t => t.CreationTime)
                .ToListAsync();

            return tasks;
        }
    }
}
