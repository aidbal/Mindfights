using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Skautatinklis.Authorization.Roles;
using Skautatinklis.Authorization.Users;
using Skautatinklis.Models;
using Skautatinklis.MultiTenancy;

namespace Skautatinklis.EntityFrameworkCore
{
    public class SkautatinklisDbContext : AbpZeroDbContext<Tenant, Role, User, SkautatinklisDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<ScoutAchievements> ScoutAchievements { get; set; }
        public DbSet<Mindfight> Mindfights { get; set; }
        public DbSet<MindfightQuestion> MindfightQuestions { get; set; }
        public DbSet<MindfightQuestionAnswer> MindfightQuestionAnswers { get; set; }
        public DbSet<TeamAnswer> TeamAnswers { get; set; }
        public DbSet<MindfightResult> MindfightResults { get; set; }
        public DbSet<MindfightRegistration> MindfightRegistrations { get; set; }


        public SkautatinklisDbContext(DbContextOptions<SkautatinklisDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MindfightQuestionMindfight>()
                .HasKey(x => new { x.MindfightId, x.MindfightQuestionId});

            modelBuilder.Entity<UserMindfightResult>()
                .HasKey(x => new { x.UserId, x.MindfightResultId });
        }
    }
}
