using System.Linq;
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
        public DbSet<City> Cities { get; set; }
        public DbSet<ScoutAchievementType> ScoutAchievementTypes { get; set; }
        public DbSet<ScoutAchievements> ScoutAchievements { get; set; }
        public DbSet<ScoutGroup> ScoutGroups { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Mindfight> Mindfights { get; set; }
        public DbSet<MindfightQuestionType> MindfightQuestionTypes { get; set; }
        public DbSet<MindfightQuestion> MindfightQuestions { get; set; }
        public DbSet<MindfightQuestionAnswer> MindfightQuestionAnswers { get; set; }
        public DbSet<TeamAnswer> TeamAnswers { get; set; }
        public DbSet<MindfightRegistration> MindfightRegistrations { get; set; }
        public DbSet<MindfightResult> MindfightResults { get; set; }
        public DbSet<UserMindfightResult> UserMindfightResults { get; set; }
        //public DbSet<MindfightAssignedQuestions> MindfightAssignedQuestions { get; set; }


        public SkautatinklisDbContext(DbContextOptions<SkautatinklisDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeamAnswer>()
                .HasOne(ta => ta.Question).WithMany(mq => mq.TeamAnswers).IsRequired().OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TeamAnswer>()
                .HasOne(ta => ta.User).WithMany(mq => mq.TeamAnswers).IsRequired().OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TeamAnswer>()
                .HasOne(ta => ta.Team).WithMany(mq => mq.TeamAnswers).IsRequired().OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MindfightEvaluators>()
                .HasKey(me => new { me.UserId, me.MindfightId });

            modelBuilder.Entity<MindfightEvaluators>()
                .HasOne(me => me.User)
                .WithMany(u => u.Mindfights)
                .HasForeignKey(me => me.UserId);

            modelBuilder.Entity<MindfightEvaluators>()
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

            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Mindfight>()
            //    .HasOne(m => m.Creator)
            //    .WithMany(u => u.CreatedMindfights)
            //    .HasForeignKey(m => m.CreatorId);

            //modelBuilder.Entity<User>()
            //    .HasOne(u => u.)
            //    .WithMany(a => a.Variants).HasForeignKey(x => x.QuestionId).OnDelete(DeleteBehavior.SetNull);

            //modelBuilder.Entity<MindfightResult>()
            //    .HasMany(t => t.Users)
            //    .WithOne()
            //    .HasPrincipalKey(u => u.Id);

            //modelBuilder.Entity<MindfightAssignedQuestions>()
            //    .HasKey(x => new { x.MindfightId, x.MindfightQuestionId});

            //modelBuilder.Entity<MindfightAssignedQuestions>()
            //    .HasOne(t => t.Mindfight)
            //    .WithMany(m => m.Questions)
            //    .HasForeignKey(t => t.MindfightQuestionId);

            //modelBuilder.Entity<MindfightAssignedQuestions>()
            //    .HasOne(t => t.MindfightQuestion)
            //    .WithMany(m => m.Mindfights)
            //    .HasForeignKey(t => t.MindfightId);

            //modelBuilder.Entity<UserMindfightResult>()
            //    .HasKey(x => new { x.UserId, x.MindfightResultId });

            //modelBuilder.Entity<UserMindfightResult>()
            //    .HasOne(t => t.User)
            //    .WithMany(m => m.MindfightResults)
            //    .HasForeignKey(t => t.UserId);

            //modelBuilder.Entity<UserMindfightResult>()
            //    .HasOne(t => t.MindfightResult)
            //    .WithMany(m => m.Users)
            //    .HasForeignKey(t => t.UserId);
        }
    }
}
