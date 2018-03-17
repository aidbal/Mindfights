using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Skautatinklis.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Abp.Authorization;
using Abp.Runtime.Session;

namespace Skautatinklis.Services
{
    [AbpAuthorize()]
    public class SkautatinklisService
    {
        private readonly IRepository<Mindfight, long> _mindfightRepository;


        public SkautatinklisService(IRepository<Mindfight, long> mindfightRepository)
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
