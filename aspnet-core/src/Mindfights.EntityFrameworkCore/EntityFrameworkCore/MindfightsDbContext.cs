using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mindfights.Authorization.Roles;
using Mindfights.Authorization.Users;
using Mindfights.Models;
using Mindfights.MultiTenancy;

namespace Mindfights.EntityFrameworkCore
{
    public class MindfightsDbContext : AbpZeroDbContext<Tenant, Role, User, MindfightsDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<City> Cities { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Mindfight> Mindfights { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TeamAnswer> TeamAnswers { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<MindfightResult> MindfightResults { get; set; }
        public DbSet<UserMindfightResult> UserMindfightResults { get; set; }

        public MindfightsDbContext(DbContextOptions<MindfightsDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeamAnswer>()
                .HasOne(ta => ta.Question).WithMany(q => q.TeamAnswers).IsRequired().OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TeamAnswer>()
                .HasOne(ta => ta.Team).WithMany(mq => mq.TeamAnswers).IsRequired().OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<MindfightEvaluator>()
                .HasKey(me => new { me.UserId, me.MindfightId });

            modelBuilder.Entity<MindfightEvaluator>()
                .HasOne(me => me.User)
                .WithMany(u => u.Mindfights)
                .HasForeignKey(me => me.UserId);

            modelBuilder.Entity<MindfightEvaluator>()
                .HasOne(me => me.Mindfight)
                .WithMany(m => m.Evaluators)
                .HasForeignKey(me => me.MindfightId);


            modelBuilder.Entity<UserMindfightResult>()
                .HasKey(umr => new { umr.UserId, umr.MindfightResultId });

            modelBuilder.Entity<UserMindfightResult>()
                .HasOne(umr => umr.User)
                .WithMany(u => u.MindfightResults)
                .HasForeignKey(umr => umr.UserId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserMindfightResult>()
                .HasOne(umr => umr.MindfightResult)
                .WithMany(mr => mr.Users)
                .HasForeignKey(umr => umr.MindfightResultId);


            //modelBuilder.Entity<MindfightConfirmedTeam>()
            //    .HasKey(mat => new { mat.MindfightId, mat.TeamId });

            //modelBuilder.Entity<MindfightConfirmedTeam>()
            //    .HasOne(mat => mat.Mindfight)
            //    .WithMany(m => m.AllowedTeams)
            //    .HasForeignKey(mat => mat.MindfightId).OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<MindfightConfirmedTeam>()
            //    .HasOne(mat => mat.Team)
            //    .WithMany(t => t.AllowedPrivateMindfights)
            //    .HasForeignKey(mat => mat.TeamId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
