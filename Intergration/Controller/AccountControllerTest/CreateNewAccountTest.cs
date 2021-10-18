using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.AccountDTO;
using kroniiapi.DTO.Email;
using kroniiapi.DTO.Profiles;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace kroniiapiTest.Intergration.Controller.AccountControllerTest
{
    [TestFixture]
    public class CreateNewAccountTest
    {
        private Mock<IEmailService> mockEmailService = new Mock<IEmailService>();
        private DataContext dataContext;
        private IMapper mapper;
        private AccountService accountService;
        private AccountController accountController;
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
        private readonly List<AccountInput> testList = new List<AccountInput>() {
            new AccountInput() {
                Username = "huynhqtienvtag",
                Fullname = "Huynh Quang Tien",
                Email = "huynhqtienvtag@gmail.com",
                Role = "admin"
            },
            new AccountInput() {
                Username = "huynhqtienvk1",
                Fullname = "Huynh Quang Tien",
                Email = "huynhqtienvk1@gmail.com",
                Role = "trainer"
            },
            new AccountInput() {
                Username = "huynhqtienvk2",
                Fullname = "Huynh Quang Tien",
                Email = "huynhqtienvk2@gmail.com",
                Role = "trainee"
            },
            new AccountInput() {
                Username = "huynhqtienvk3",
                Fullname = "Huynh Quang Tien",
                Email = "huynhqtienvk3@gmail.com",
                Role = "company"
            },
            new AccountInput() {
                Username = "huynhqtienvk4",
                Fullname = "Huynh Quang Tien",
                Email = "huynhqtienvk4@gmail.com",
                Role = "administrator"
            }
        };

        [OneTimeSetUp]
        public void setupFirst() {
            var config = new MapperConfiguration(config => {
                config.AddProfile(new AccountProfile());
            });
            mapper = config.CreateMapper();
            mockEmailService.Setup(email => email.SendEmailAsync(It.IsAny<EmailContent>())).Returns(Task.CompletedTask);
        }

        [SetUp]
        public void setup() {
            var option = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase("data").Options;
            dataContext = new DataContext(option);
            dataContext.Roles.AddRange(roleList);
            dataContext.SaveChanges();
            accountService = new AccountService(
                dataContext, 
                mapper, 
                new AdminService(dataContext),
                new AdministratorService(dataContext),
                new CompanyService(dataContext),
                new TraineeService(dataContext),
                new TrainerService(dataContext),
                mockEmailService.Object
            );
            accountController = new AccountController(accountService, mapper, mockEmailService.Object);
        }

        [Test]
        public async Task CreateNewAccount_Success() {
            foreach (var input in testList) {
                var result = await accountController.CreateNewAccount(input) as ObjectResult;
                Assert.AreEqual(201, result.StatusCode);
            }
        }

        [Test]
        public async Task CreateNewAccount_Conflict() {
            foreach (var input in testList) {
                await accountController.CreateNewAccount(input);
                var result = await accountController.CreateNewAccount(input) as ObjectResult;
                Assert.AreEqual(409, result.StatusCode, "Should be failed if insert self twice");
            }
            foreach (var input in testList) {
                var result = await accountController.CreateNewAccount(input) as ObjectResult;
                Assert.AreEqual(409, result.StatusCode, "Should be failed if insert self when filled");
            }
        }
    }
}