using Abp.Authorization.Users;
using Abp.Timing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Mindfights.Authorization.Roles;
using Mindfights.Models;
using System.IO;
using System.Linq;
using Abp.Authorization.Roles;

namespace Mindfights.EntityFrameworkCore.Seed.Mindfights
{
    class MindfightCreator
    {
        private readonly MindfightsDbContext _context;

        public MindfightCreator(MindfightsDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            AddDefaultRoles();
            AddDefaultCities();
            CreateMindfight();
            CreateTour();
            CreateQuestion();
            CreateTeam();
            CreateRegistration();
            CreateTeamAnswers();
            CreateMindfightResult();
            CreateUserMindfightResult();
        }

        private void AddDefaultRoles()
        {
            var creatorRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.Name == "Creator");
            var moderatorRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.Name == "Moderator");
            if (creatorRole == null)
            {
                creatorRole = _context.Roles.Add(new Role(1, "Creator", "Creator") { IsStatic = false }).Entity;
                var rolePermissionCreateCreator = new RolePermissionSetting { TenantId = 1, Name = "CreateMindfights", IsGranted = true, RoleId = creatorRole.Id };
                _context.Permissions.Add(rolePermissionCreateCreator);
            }
            if (moderatorRole == null)
            {
                moderatorRole = _context.Roles.Add(new Role(1, "Moderator", "Moderator") { IsStatic = false }).Entity;
                var rolePermissionCreateModerator = new RolePermissionSetting { TenantId = 1, Name = "CreateMindfights", IsGranted = true, RoleId = moderatorRole.Id };
                var rolePermissionManageModerator = new RolePermissionSetting { TenantId = 1, Name = "ManageMindfights", IsGranted = true, RoleId = moderatorRole.Id };
                _context.Permissions.Add(rolePermissionCreateModerator);
                _context.Permissions.Add(rolePermissionManageModerator);
            }
            _context.SaveChanges();

            var adminUsers = _context.Users.IgnoreQueryFilters().Where(u => u.UserName == AbpUserBase.AdminUserName).ToList();

            foreach (var adminUser in adminUsers)
            {
                if (_context.UserRoles.IgnoreQueryFilters().FirstOrDefault(u => u.RoleId == creatorRole.Id && u.UserId == adminUser.Id) == null)
                    _context.UserRoles.Add(new UserRole(1, adminUser.Id, creatorRole.Id));
                if (_context.UserRoles.IgnoreQueryFilters().FirstOrDefault(u => u.RoleId == moderatorRole.Id && u.UserId == adminUser.Id) == null)
                    _context.UserRoles.Add(new UserRole(1, adminUser.Id, moderatorRole.Id));
            }
            _context.SaveChanges();
        }

        private void AddDefaultCities()
        {
            var citiesFromFile = File.ReadAllText("../Mindfights.Core/Models/Cities.json");
            var citiesObject = JObject.Parse(citiesFromFile)["Cities"];
            var cities = citiesObject.ToObject<string[]>();
            foreach (var city in cities)
            {
                var cityInDb = _context.Cities.IgnoreQueryFilters().FirstOrDefault(t => t.Name == city);
                if (cityInDb == null)
                {
                    _context.Cities.Add(new City { Name = city });
                }
            }
            _context.SaveChanges();
        }

        private void CreateMindfight()
        {
            var mindfight = _context.Mindfights.IgnoreQueryFilters().FirstOrDefault(t => t.Title == "Demo");
            if (mindfight == null)
            {
                var user = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == null && u.UserName == AbpUserBase.AdminUserName);
                if (user != null)
                {
                    mindfight = new Mindfight(
                        user,
                        "Demo",
                        "Demo description",
                        10,
                        Clock.Now,
                        null,
                        null,
                        20
                    );

                    _context.Mindfights.Add(mindfight);
                    _context.SaveChanges();
                }
            }
        }

        private void CreateTour()
        {
            var mindfight = _context.Mindfights.IgnoreQueryFilters().FirstOrDefault(t => t.Title == "Demo");
            var tour = _context.Tours.IgnoreQueryFilters().FirstOrDefault(t => t.Title == "DemoTour");
            if (tour == null && mindfight != null)
            {
                tour = new Tour(mindfight, "DemoTour", "DemoDescription", 120, 10, 1);

                _context.Tours.Add(tour);
                _context.SaveChanges();
            }
        }

        private void CreateQuestion()
        {
            var tour = _context.Tours.IgnoreQueryFilters().FirstOrDefault(t => t.Title == "DemoTour");
            var question = _context.Questions.IgnoreQueryFilters().FirstOrDefault(t => t.Title == "DemoQuestion");
            if (tour != null && question == null)
            {
                question = new Question(tour, "DemoQuestion", "DemoDescription", "DemoAnswer", 10, 10, 1, null);

                _context.Questions.Add(question);
                _context.SaveChanges();
            }
        }

        private void CreateTeam()
        {
            var team = _context.Teams.IgnoreQueryFilters().FirstOrDefault(t => t.Name == "Demo");
            var user = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == null && u.UserName == AbpUserBase.AdminUserName);
            if (team == null && user != null)
            {
                team = new Team(user, "Demo", "Best Demo Team!");

                _context.Teams.Add(team);
                _context.SaveChanges();
            }
        }

        private void CreateRegistration()
        {
            var team = _context.Teams.IgnoreQueryFilters().FirstOrDefault(t => t.Name == "Demo");
            var mindfight = _context.Mindfights.IgnoreQueryFilters().FirstOrDefault(t => t.Title == "Demo");
            var registration = _context.Registrations.IgnoreQueryFilters().FirstOrDefault(t => t.Mindfight == mindfight);
            if (team != null && mindfight != null && registration == null)
            {
                registration = new Registration(mindfight, team);
                _context.Registrations.Add(registration);
                _context.SaveChanges();
            }
        }

        private void CreateTeamAnswers()
        {
            var team = _context.Teams.IgnoreQueryFilters().FirstOrDefault(t => t.Name == "Demo");
            var question = _context.Questions.IgnoreQueryFilters().FirstOrDefault(t => t.Title == "DemoQuestion");
            var user = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == null && u.UserName == AbpUserBase.AdminUserName);
            if(question != null) {
                var teamAnswer = _context.TeamAnswers.IgnoreQueryFilters().FirstOrDefault(t => t.Question == question);
                if (team != null && user != null && teamAnswer == null)
                {
                    teamAnswer = new TeamAnswer(question, team, "Team entered this demo answer", false);

                    _context.TeamAnswers.Add(teamAnswer);
                    _context.SaveChanges();
                }
            }
        }

        private void CreateMindfightResult()
        {
            var team = _context.Teams.IgnoreQueryFilters().FirstOrDefault(t => t.Name == "Demo");
            var mindfight = _context.Mindfights.IgnoreQueryFilters().FirstOrDefault(t => t.Title == "Demo");
            var mindfightResult = _context.MindfightResults.IgnoreQueryFilters()
                .FirstOrDefault(t => t.Mindfight == mindfight);
            if (team != null && mindfight != null && mindfightResult == null)
            {
                mindfightResult = new MindfightResult(30, false, team, mindfight);

                _context.MindfightResults.Add(mindfightResult);
                _context.SaveChanges();
            }
        }

        private void CreateUserMindfightResult()
        {
            var mindfight = _context.Mindfights.IgnoreQueryFilters().FirstOrDefault(t => t.Title == "Demo");
            var mindfightResult = _context.MindfightResults.IgnoreQueryFilters()
                .FirstOrDefault(t => t.Mindfight == mindfight);
            var user = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == null && u.UserName == AbpUserBase.AdminUserName);
            var userMindfightResult = _context.UserMindfightResults.IgnoreQueryFilters()
                .FirstOrDefault(t => t.MindfightResult.Mindfight == mindfight);
            if (user != null && mindfightResult != null && userMindfightResult == null)
            {
                userMindfightResult = new UserMindfightResult(user, mindfightResult);

                _context.UserMindfightResults.Add(userMindfightResult);
                _context.SaveChanges();
            }
        }
    }
}
