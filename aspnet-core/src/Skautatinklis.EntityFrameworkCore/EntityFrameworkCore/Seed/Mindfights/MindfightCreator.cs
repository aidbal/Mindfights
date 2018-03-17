using Abp.Authorization.Users;
using Abp.Timing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Skautatinklis.Authorization.Roles;
using Skautatinklis.Models;
using System.IO;
using System.Linq;

namespace Skautatinklis.EntityFrameworkCore.Seed.Mindfights
{
    class MindfightCreator
    {
        private readonly SkautatinklisDbContext _context;

        public MindfightCreator(SkautatinklisDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            AddDefaultRoles();
            AddDefaultCities();
            //TODO add cities seed
            CreateMindfight();
            CreateQuestionType();
            CreateQuestion();
            CreateAnswers();
            CreateScoutGroup();
            CreateTeam();
            CreateRegistration();
            CreateTeamAnswers();
            CreateMindfightResult();
            CreateUserMindfightResult();
        }

        private void AddDefaultRoles()
        {
            //TODO Add permissions to roles

            var userRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.Name == "User");
            var scoutRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.Name == "Scout");
            var scoutLeaderRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.Name == "ScoutLeader");
            var moderatorRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.Name == "Moderator");
            if (userRole == null)
            {
                userRole = _context.Roles.Add(new Role(null, "User", "User") { IsStatic = true }).Entity;
            }
            if (scoutRole == null)
            {
                scoutRole = _context.Roles.Add(new Role(null, "Scout", "Scout") { IsStatic = true }).Entity;
            }
            if (scoutLeaderRole == null)
            {
                scoutLeaderRole = _context.Roles.Add(new Role(null, "ScoutLeader", "ScoutLeader") { IsStatic = true }).Entity;
            }
            if (moderatorRole == null)
            {
                moderatorRole = _context.Roles.Add(new Role(null, "Moderator", "Moderator") { IsStatic = true }).Entity;
            }
            _context.SaveChanges();

            var adminUsers = _context.Users.IgnoreQueryFilters().Where(u => u.UserName == AbpUserBase.AdminUserName).ToList();

            foreach (var adminUser in adminUsers)
            {
                if(_context.UserRoles.IgnoreQueryFilters().FirstOrDefault(u => u.RoleId == userRole.Id && u.UserId == adminUser.Id) == null)
                    _context.UserRoles.Add(new UserRole(null, adminUser.Id, userRole.Id));
                if (_context.UserRoles.IgnoreQueryFilters().FirstOrDefault(u => u.RoleId == scoutRole.Id && u.UserId == adminUser.Id) == null)
                    _context.UserRoles.Add(new UserRole(null, adminUser.Id, scoutRole.Id));
                if (_context.UserRoles.IgnoreQueryFilters().FirstOrDefault(u => u.RoleId == scoutLeaderRole.Id && u.UserId == adminUser.Id) == null)
                    _context.UserRoles.Add(new UserRole(null, adminUser.Id, scoutLeaderRole.Id));
                if (_context.UserRoles.IgnoreQueryFilters().FirstOrDefault(u => u.RoleId == moderatorRole.Id && u.UserId == adminUser.Id) == null)
                    _context.UserRoles.Add(new UserRole(null, adminUser.Id, moderatorRole.Id));
            }
            _context.SaveChanges();
        }

        private void AddDefaultCities()
        {
            var citiesFromFile = File.ReadAllText("../Skautatinklis.Core/Models/Cities.json");
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
                        20,
                        false
                    );

                    _context.Mindfights.Add(mindfight);
                    _context.SaveChanges();
                }
            }
        }

        private void CreateQuestionType()
        {
            var questionType = _context.MindfightQuestionTypes.IgnoreQueryFilters().FirstOrDefault(t => t.Type == "Test");
            if (questionType == null)
            {
                questionType = new MindfightQuestionType { Type = "Test" };

                _context.MindfightQuestionTypes.Add(questionType);
                _context.SaveChanges();
            }
        }

        private void CreateQuestion()
        {
            var mindfight = _context.Mindfights.IgnoreQueryFilters().FirstOrDefault(t => t.Title == "Demo");
            var questionType = _context.MindfightQuestionTypes.IgnoreQueryFilters().FirstOrDefault(t => t.Type == "Test");
            var question = _context.MindfightQuestions.IgnoreQueryFilters().FirstOrDefault(t => t.Title == "DemoQuestion");
            if (question == null && mindfight != null && questionType != null)
            {
                question = new MindfightQuestion(mindfight, questionType, "DemoQuestion", "DemoDescription", 10, 1, null);

                _context.MindfightQuestions.Add(question);
                _context.SaveChanges();
            }
        }

        private void CreateAnswers()
        {
            var mindfight = _context.Mindfights.IgnoreQueryFilters().FirstOrDefault(t => t.Title == "Demo");
            var question = _context.MindfightQuestions.IgnoreQueryFilters().FirstOrDefault(t => t.Title == "DemoQuestion");
            var answer1 = _context.MindfightQuestionAnswers.IgnoreQueryFilters().FirstOrDefault(t => t.Answer == "DemoAnswer1");
            var answer2 = _context.MindfightQuestionAnswers.IgnoreQueryFilters().FirstOrDefault(t => t.Answer == "DemoAnswer2");
            if (mindfight != null && question != null && answer1 == null && answer2 == null)
            {
                answer1 = new MindfightQuestionAnswer(question, "DemoAnswer1", false);
                answer2 = new MindfightQuestionAnswer(question, "DemoAnswer2", false);

                _context.MindfightQuestionAnswers.Add(answer1);
                _context.MindfightQuestionAnswers.Add(answer2);
                _context.SaveChanges();
            }
        }

        private void CreateScoutGroup()
        {
            var scoutGroup = _context.ScoutGroups.IgnoreQueryFilters().FirstOrDefault(t => t.Name == "Demo");
            var user = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == null && u.UserName == AbpUserBase.AdminUserName);
            if (scoutGroup == null && user != null)
            {
                scoutGroup = new ScoutGroup(user, "Demo", "Best Demo Team!");

                _context.ScoutGroups.Add(scoutGroup);
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
            var registration = _context.MindfightRegistrations.IgnoreQueryFilters().FirstOrDefault(t => t.Mindfight == mindfight);
            if (team != null && mindfight != null && registration == null)
            {
                registration = new MindfightRegistration(mindfight, team);
                _context.MindfightRegistrations.Add(registration);
                _context.SaveChanges();
            }
        }

        private void CreateTeamAnswers()
        {
            var team = _context.Teams.IgnoreQueryFilters().FirstOrDefault(t => t.Name == "Demo");
            var question = _context.MindfightQuestions.IgnoreQueryFilters().FirstOrDefault(t => t.Title == "DemoQuestion");
            var user = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == null && u.UserName == AbpUserBase.AdminUserName);
            var teamAnswer = _context.TeamAnswers.IgnoreQueryFilters().FirstOrDefault(t => t.Question == question);
            if (team != null && question != null && user != null && teamAnswer == null)
            {
                teamAnswer = new TeamAnswer(question, user, team, "Team entered this demo answer", 20, false);

                _context.TeamAnswers.Add(teamAnswer);
                _context.SaveChanges();
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
