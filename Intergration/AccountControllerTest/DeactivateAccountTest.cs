using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.Profiles;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace kroniiapiTest.Intergration.AccountControllerTest
{
    public class DeactivateAccountTest
    {
        private DataContext _context;
        private IMapper mapper;
        private IClassService classService;
        private AccountService accountService;
        private AccountController accountController;
        private Mock<IEmailService> mockEmailService = new Mock<IEmailService>();
        private readonly List<Role> roleList = new List<Role>() {
            new Role() {
                RoleId = 1,
                RoleName = "Administrator"
            },
            new Role() {
                RoleId = 2,
                RoleName = "Admin"
            },
            new Role() {
                RoleId = 3,
                RoleName = "Trainer"
            },
            new Role() {
                RoleId = 4,
                RoleName = "Trainee"
            },
            new Role() {
                RoleId = 5,
                RoleName = "Company"
            }
        };
        Administrator testAdministrator = new Administrator()
        {
            AdministratorId = 1,
            Username = "thienphu",
            Fullname = "tranthienphu",
            Email = "phu@gmail.com",
            RoleId = 1
        };
        Admin testAdmin = new Admin()
        {
            AdminId = 1,
            Username = "khanhtoan",
            Fullname = "trankhanhtoan",
            Email = "toan@gmail.com",
            RoleId = 2
        };
        Trainer testTrainer = new Trainer()
        {
            TrainerId = 1,
            Username = "duykhang",
            Fullname = "thuongduykhang",
            Email = "khang@gmail.com",
            RoleId = 3
        };
        Trainee testTrainee = new Trainee()
        {
            TraineeId = 1,
            Username = "anhtho",
            Fullname = "tieuanhtho",
            Email = "tho@gmail.com",
            RoleId = 4
        };
        Company testCompany = new Company()
        {
            CompanyId = 1,
            Username = "hailong",
            Fullname = "lehoanghailong",
            Email = "long@gmail.com",
            RoleId = 5
        };

        [SetUp]
        public void Setup()
        {
            var option = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "kroniiapi").Options;

            _context = new DataContext(option);
            _context.Roles.AddRange(roleList);
            _context.Admins.AddRange(testAdmin);
            _context.Administrators.AddRange(testAdministrator);
            _context.Trainers.AddRange(testTrainer);
            _context.Trainees.AddRange(testTrainee);
            _context.Companies.AddRange(testCompany);
            _context.SaveChanges();
            var config = new MapperConfiguration(config =>
            {
                config.AddProfile(new AccountProfile());
            });
            mapper = config.CreateMapper();

            accountService = new AccountService(
                _context,
                mapper,
                new AdminService(_context, classService),
                new AdministratorService(_context),
                new CompanyService(_context),
                new TraineeService(_context),
                new TrainerService(_context, classService),
                mockEmailService.Object
            );
            accountController = new AccountController(accountService, mapper, mockEmailService.Object);
        }

        public static IEnumerable<TestCaseData> DeactivateAccountTestCaseTrue
        {
            get
            {
                //True case: with AccountId, role and expect responseDTO
                yield return new TestCaseData(
                    1,
                    "Admin",
                    new ResponseDTO(200, "Deleted!")
                );

                yield return new TestCaseData(
                    1,
                    "Trainer",
                    new ResponseDTO(200, "Deleted!")
                );

            }
        }
        [Test]
        [TestCaseSource("DeactivateAccountTestCaseTrue")]
        public async Task DeactivateAccountTestTrue(int id, string role, ResponseDTO expect)
        {
            var rs = await accountController.DeactivateAccount(id, role);
            var objResult = (rs as OkObjectResult).Value as ResponseDTO;
            var expectJson = JsonConvert.SerializeObject(expect);
            var actualJson = JsonConvert.SerializeObject(objResult);
            Assert.AreEqual(expectJson, actualJson);
        }

        public static IEnumerable<TestCaseData> DeactivateAccountTestCaseFail
        {
            get
            {
                // False case: with id not found
                yield return new TestCaseData(
                    2,
                    "Company",
                    new ResponseDTO(404, "Id not found!")
                );
                // false case: with admistrator role
                yield return new TestCaseData(
                    1,
                    "Administrator",
                    new ResponseDTO(409, "Can't deactivate administrator")
                );
            }
        }

        [Test]
        [TestCaseSource("DeactivateAccountTestCaseFail")]
        public async Task DeactivateAccountTestFail(int id, string role, ResponseDTO expect)
        {
            var rs = await accountController.DeactivateAccount(id, role);
            var objResult = (rs as ObjectResult).Value as ResponseDTO;
            var expectJson = JsonConvert.SerializeObject(expect);
            var actualJson = JsonConvert.SerializeObject(objResult);
            Assert.AreEqual(expectJson, actualJson);
        }
        [TearDown]
        public void tearDown()
        {
            _context.Administrators.RemoveRange(_context.Administrators);
            _context.Admins.RemoveRange(_context.Admins);
            _context.Companies.RemoveRange(_context.Companies);
            _context.Trainees.RemoveRange(_context.Trainees);
            _context.Trainers.RemoveRange(_context.Trainers);
            _context.Roles.RemoveRange(_context.Roles);
            _context.SaveChanges();
        }

    }
}