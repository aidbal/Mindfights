using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Skautatinklis.Configuration;
using Skautatinklis.Web;

namespace Skautatinklis.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class SkautatinklisDbContextFactory : IDesignTimeDbContextFactory<SkautatinklisDbContext>
    {
        public SkautatinklisDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<SkautatinklisDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            SkautatinklisDbContextConfigurer.Configure(builder, configuration.GetConnectionString(SkautatinklisConsts.ConnectionStringName));

            return new SkautatinklisDbContext(builder.Options);
        }
    }
}
