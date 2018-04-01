using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Mindfights.EntityFrameworkCore
{
    public static class MindfightsDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<MindfightsDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<MindfightsDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
