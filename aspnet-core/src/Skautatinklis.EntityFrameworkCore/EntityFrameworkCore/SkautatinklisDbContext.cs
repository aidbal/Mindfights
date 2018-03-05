using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Skautatinklis.Authorization.Roles;
using Skautatinklis.Authorization.Users;
using Skautatinklis.MultiTenancy;

namespace Skautatinklis.EntityFrameworkCore
{
    public class SkautatinklisDbContext : AbpZeroDbContext<Tenant, Role, User, SkautatinklisDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public SkautatinklisDbContext(DbContextOptions<SkautatinklisDbContext> options)
            : base(options)
        {
        }
    }
}
