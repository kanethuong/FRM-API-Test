using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.AccountDTO;
using kroniiapi.DTO.Email;
using kroniiapi.DTO.Profiles;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace kroniiapiTest.Intergration.AccountControllerTest
{
    public class ForgotPasswordTest
    {
        private Mock<IEmailService> mockEmailService = new Mock<IEmailService>();
        private DataContext dataContext;
        private IMapper mapper;
        private IClassService classService;
        private AccountService accountService;
        private AccountController accountController;

        private readonly List<Role> roleList = new List<Role>()
        {
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

        private readonly Administrator administrator = new Administrator()
        {
            AdministratorId = 1,
            Username = "duykhang",
            Fullname = "Duy Khang",
            Email = "duykhang@gmail.com",
            RoleId = 1
        };

        private readonly Admin admin = new Admin()
        {
            AdminId = 1,
            Username = "anhtho",
            Fullname = "Anh Tho",
            Email = "anhtho@gmail.com",
            RoleId = 2
        };

        private readonly Trainer trainer = new Trainer()
        {
            TrainerId = 1,
            Username = "khanhtoan",
            Fullname = "Khanh Toan",
            Email = "khanhtoan@gmail.com",
            RoleId = 3
        };

        private readonly Trainee trainee = new Trainee()
        {
            TraineeId = 1,
            Username = "thanhdat",
            Fullname = "Thanh Dat",
            Email = "thanhdat@gmail.com",
            RoleId = 4
        };

        private readonly Company company = new Company()
        {
            CompanyId = 1,
            Username = "fsoft",
            Fullname = "FPT Software",
            Email = "fsoft@gmail.com",
            RoleId = 5
        };

        [OneTimeSetUp]
        public void setupFirst()
        {
            var option = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase("data").Options;
            dataContext = new DataContext(option);
            dataContext.Roles.AddRange(roleList);
            dataContext.Administrators.AddRange(administrator);
            dataContext.Admins.AddRange(admin);
            dataContext.Trainers.AddRange(trainer);
            dataContext.Trainees.AddRange(trainee);
            dataContext.Companies.AddRange(company);
            dataContext.SaveChanges();

            var config = new MapperConfiguration(config =>
            {
                config.AddProfile(new AccountProfile());
            });
            mapper = config.CreateMapper();

            mockEmailService.Setup(email => email.SendEmailAsync(It.IsAny<EmailContent>())).Returns(Task.CompletedTask);

            accountService = new AccountService(
                dataContext,
                mapper,
                new AdminService(dataContext, classService),
                new AdministratorService(dataContext),
                new CompanyService(dataContext),
                new TraineeService(dataContext),
                new TrainerService(dataContext, classService),
                mockEmailService.Object
            );
            accountController = new AccountController(accountService, mapper, mockEmailService.Object);
        }

        [TearDown]
        public void tearDown()
        {
            dataContext.Administrators.RemoveRange(dataContext.Administrators);
            dataContext.Admins.RemoveRange(dataContext.Admins);
            dataContext.Companies.RemoveRange(dataContext.Companies);
            dataContext.Trainees.RemoveRange(dataContext.Trainees);
            dataContext.Trainers.RemoveRange(dataContext.Trainers);
            dataContext.Roles.RemoveRange(dataContext.Roles);
            dataContext.SaveChanges();
        }

        public static IEnumerable<TestCaseData> ForgotPasswordTestCases
        {
            get
            {
                //True case
                yield return new TestCaseData(
                    new EmailInput
                    {
                        Email = "anhtho@gmail.com"
                    },
                    200
                );

                //False case: cannot change password of role administrator
                yield return new TestCaseData(
                    new EmailInput
                    {
                        Email = "duykhang@gmail.com"
                    },
                    404
                );

                //False case: wrond email format
                yield return new TestCaseData(
                    new EmailInput
                    {
                        Email = "abcxyz"
                    },
                    404
                );

                //False case: email not exist in database
                yield return new TestCaseData(
                    new EmailInput
                    {
                        Email = "notexist@gmail.com"
                    },
                    404
                );
            }
        }

        [Test]
        [TestCaseSource("ForgotPasswordTestCases")]
        public async Task ForgotPassword_Test(EmailInput emailInput, int expStatus)
        {
            // Act
            var rs = await accountController.ForgotPassword(emailInput) as ObjectResult;
            var response = rs.Value as ResponseDTO;

            // Assert
            Assert.True(
                expStatus == rs.StatusCode &&
                expStatus == response.Status
            );
        }

    }
}