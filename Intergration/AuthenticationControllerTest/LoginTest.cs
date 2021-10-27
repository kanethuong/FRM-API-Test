using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.AuthDTO;
using kroniiapi.DTO.Email;
using kroniiapi.DTO.Profiles;
using kroniiapi.Helper;
using kroniiapi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Moq;
using NUnit.Framework;

namespace kroniiapiTest.Intergration.AuthenticationControllerTest
{
    [TestFixture]
    public class LoginTest
    {

        private Mock<IEmailService> mockEmailService = new Mock<IEmailService>();
        private DataContext dataContext;
        private IMapper mapper;
        private IClassService classService;
        private AccountService accountService;
        private AccountController accountController;
        private AuthenticationController authenticationController;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IRefreshToken _refreshToken;
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
        List<Admin> adminsList = new List<Admin>
        {
            new Admin{
                AdminId = 1,
                Username = "fptunisersity",
                Password = "fptunisersity",
                Fullname = "DaiHocFPTCanTho",
                AvatarURL = "none",
                Email = "fptu.cantho@fe.edu.vn",
                Phone = "02927303636",
                DOB = new DateTime(2000, 1 , 1),
                Address = "600, An Binh, Can Tho",
                Gender = "Male",
                Wage = 10000,
                CreatedAt =  new DateTime(2021, 1 , 1),
                IsDeactivated = false,
                RoleId = 2,
            },
            new Admin {
                AdminId = 2,
                Username = "thuytienkhongsaoke",
                Password = "123456",
                Fullname = "TranThuyTien",
                AvatarURL = "none",
                Email = "thuytienkosaoke@gmail.vn",
                Phone = "0786342221",
                DOB = new DateTime(2000, 4 , 7),
                Address = "Nam Long, Quan 7, HCM",
                Gender = "Female",
                Wage = 20000,
                CreatedAt =  new DateTime(2021, 1 , 1),
                IsDeactivated = false,
                RoleId = 2,
            },
            new Admin {
                AdminId = 3,
                Username = "congvinhkosaoke",
                Password = "654321",
                Fullname = "LeCongVinh",
                AvatarURL = "none",
                Email = "congvinhkhongsaoke@gmail.vn",
                Phone = "029288290",
                DOB = new DateTime(2000, 2 , 1),
                Address = "Nam Long, Quan 7, HCM",
                Gender = "Male",
                Wage = 10000,
                CreatedAt =  new DateTime(2021, 1 , 1),
                IsDeactivated = false,
                RoleId = 2,
            }
        };


        [OneTimeSetUp]
        public void setupFirst()
        {
            var option = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase("data").Options;
            dataContext = new DataContext(option);
            dataContext.Roles.AddRange(roleList);
            dataContext.Admins.AddRange(adminsList);
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
            authenticationController = new AuthenticationController(accountService, mapper, _jwtGenerator, _refreshToken);
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
        public static IEnumerable<TestCaseData> LoginTestCaseFail
        {
            get
            {
                //Fail case: Null Login Input
                yield return new TestCaseData(
                    new LoginInput
                    {
                        Email = "",
                        Password = ""
                    },
                    new
                    {
                        localCookie = "ab2bd817-98cd-4cf3-a80a-53ea0cd9c200",
                        serverCookie = "ab2bd817-98cd-4cf3-a80a-53ea0cd9c200"
                    },
                    "$2a$12$v.Hdp7QJKFozspX2LS1kmOzRsdkCdnAO3vYtod32eqhiwrunNPWiu",
                    404
                );
                //Fail case: Wrong password bcrypted 
                yield return new TestCaseData(
                    new LoginInput
                    {
                        Email = "danhlpt@gmail.com",
                        Password = "admin"
                    },
                    new
                    {
                        localCookie = "ab2bd817-98cd-4cf3-a80a-53ea0cd9c200",
                        serverCookie = "ab2bd817-98cd-4cf3-a80a-53ea0cd9c200"
                    },
                    "bcryptSai",
                    404

                );
            }
        }

        [Test]
        [TestCaseSource("LoginTestCaseFail")]
        public async Task LoginTest_ActionResult_Fail_404(LoginInput loginInput, dynamic cookieOptions, string passBcrypt, int staCode)
        {
            string localCookie, serverCookie, serverMail;
            try { localCookie = (string)cookieOptions.localCookie; } catch { localCookie = null; }
            try { serverCookie = (string)cookieOptions.serverCookie; } catch { serverCookie = null; }
            try { serverMail = (string)cookieOptions.email; } catch { serverMail = null; }

            var cookie = new StringValues((localCookie == null) ? "" : "X-Refresh-Token" + "=" + localCookie);
            authenticationController.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            authenticationController.ControllerContext.HttpContext.Request.Headers.Add(HeaderNames.Cookie, cookie);


            var rs = await authenticationController.Login(loginInput);
            var obResult = rs.Result as ObjectResult;

            Assert.AreEqual(staCode, obResult.StatusCode);
        }


        public static IEnumerable<TestCaseData> LoginTestCaseTrue
        {
            get
            {
                yield return new TestCaseData(
                    new LoginInput
                    {
                        Email = "lephamthanhdanh@gmail.com",
                        Password = "admin"
                    },
                    new
                    {
                        localCookie = "ab2bd817-98cd-4cf3-a80a-53ea0cd9c200",
                        serverCookie = "ab2bd817-98cd-4cf3-a80a-53ea0cd9c200"
                    },
                    "$2a$12$v.Hdp7QJKFozspX2LS1kmOzRsdkCdnAO3vYtod32eqhiwrunNPWiu",
                    404
                );
            }
        }

        [Test]
        [TestCaseSource("LoginTestCaseTrue")]
        public async Task LoginTest_ActionResult_True_200(LoginInput loginInput, dynamic cookieOptions, string passBcrypt, int staCode)
        {
            string localCookie, serverCookie, serverMail;
            try { localCookie = (string)cookieOptions.localCookie; } catch { localCookie = null; }
            try { serverCookie = (string)cookieOptions.serverCookie; } catch { serverCookie = null; }
            try { serverMail = (string)cookieOptions.email; } catch { serverMail = null; }

            var cookie = new StringValues((localCookie == null) ? "" : "X-Refresh-Token" + "=" + localCookie);
            authenticationController.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            authenticationController.ControllerContext.HttpContext.Request.Headers.Add(HeaderNames.Cookie, cookie);


            var rs = await authenticationController.Login(loginInput);
            var obResult = rs.Result as ObjectResult;

            Assert.AreEqual(staCode, obResult.StatusCode);
        }

    }
}