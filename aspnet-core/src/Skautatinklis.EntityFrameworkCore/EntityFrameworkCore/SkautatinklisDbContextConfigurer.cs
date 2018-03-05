using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Skautatinklis.EntityFrameworkCore
{
    public static class SkautatinklisDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<SkautatinklisDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<SkautatinklisDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
