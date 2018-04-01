using Mindfights.EntityFrameworkCore.Seed.Mindfights;

namespace Mindfights.EntityFrameworkCore.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly MindfightsDbContext _context;

        public InitialHostDbBuilder(MindfightsDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();
            new MindfightCreator(_context).Create();

            _context.SaveChanges();
        }
    }
}
