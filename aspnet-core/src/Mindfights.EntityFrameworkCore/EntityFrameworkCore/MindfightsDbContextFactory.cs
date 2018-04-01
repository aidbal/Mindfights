using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Mindfights.Configuration;
using Mindfights.Web;

namespace Mindfights.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class MindfightsDbContextFactory : IDesignTimeDbContextFactory<MindfightsDbContext>
    {
        public MindfightsDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<MindfightsDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            MindfightsDbContextConfigurer.Configure(builder, configuration.GetConnectionString(MindfightsConsts.ConnectionStringName));

            return new MindfightsDbContext(builder.Options);
        }
    }
}
