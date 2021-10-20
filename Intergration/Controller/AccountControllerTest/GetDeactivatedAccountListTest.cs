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
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.DTO.Profiles;
using kroniiapi.Helper;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace kroniiapiTest.Intergration.Controller.AccountControllerTest
{
    public class GetDeactivatedAccountListTest
    {
        private Mock<IEmailService> mockEmailService = new Mock<IEmailService>();
        private Mock<IAccountService> mockAccount = new Mock<IAccountService>();
        private DataContext dataContext;
        private IMapper mapper;
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
        Admin adminsList = new Admin
        {
                AdminId = 1,
                Username = "fptunise454264344rsity",
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
                IsDeactivated = true,
                RoleId = 2,
        };
        Trainee traineeList = new Trainee
        {            
                TraineeId = 1,
                Username = "congvinhkosaoaaaaaaaaaaaaaake",
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
                IsDeactivated = true,
                RoleId = 4,
        };
        Trainer trainerList = new Trainer
        {        
                TrainerId = 1,
                Username = "congvinhkosnsajndaoke",
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
                IsDeactivated = true,
                RoleId = 3,            
        };

        Company companyList = new Company
        {            
                CompanyId = 1,
                Username = "fptunisersadadwaedsity",
                Password = "fptunisersity",
                Fullname = "DaiHocFPTCanTho",
                AvatarURL = "none",
                Email = "fptu.cantho@fe.edu.vn",
                Phone = "02927303636",
                Address = "600, An Binh, Can Tho",
                Gender = "Male",
                CreatedAt =  new DateTime(2021, 1 , 1),
                IsDeactivated = true,
                RoleId = 5,        
            
        };



        

        [OneTimeSetUp]
        public void setupFirst() {
            var option = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase("data").Options;
            dataContext = new DataContext(option);
            dataContext.Roles.AddRange(roleList);
            dataContext.Admins.AddRange(adminsList);
            dataContext.Trainees.AddRange(traineeList);
            dataContext.Trainers.AddRange(trainerList);
            dataContext.Companies.AddRange(companyList);
            dataContext.SaveChanges();

            var config = new MapperConfiguration(config => {
                config.AddProfile(new AccountProfile());
            });
            mapper = config.CreateMapper();

            mockEmailService.Setup(email => email.SendEmailAsync(It.IsAny<EmailContent>())).Returns(Task.CompletedTask);
            
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

        [OneTimeTearDown]
        public void tearDown() {
            dataContext.Administrators.RemoveRange(dataContext.Administrators);
            dataContext.Admins.RemoveRange(dataContext.Admins);
            dataContext.Companies.RemoveRange(dataContext.Companies);
            dataContext.Trainees.RemoveRange(dataContext.Trainees);
            dataContext.Trainers.RemoveRange(dataContext.Trainers);
            dataContext.Roles.RemoveRange(dataContext.Roles);
            dataContext.SaveChanges();
        }

        public static IEnumerable<TestCaseData> GetDeletedAccountListTestCaseTrue
        {
            get
            {
                // True case: with PageNumber, PageSize and SearchName
                yield return new TestCaseData(
                    new PaginationParameter
                    {
                        PageNumber = 1,
                        PageSize = 1,
                        SearchName = ""
                    },
                    200
                );
                // True case: with PageNumber, PageSize and SearchName
                yield return new TestCaseData(
                    new PaginationParameter
                    {
                        PageNumber = 1,
                        PageSize = 1,
                        SearchName = ""
                    },
                    200
                );
            }
        }
        [Test]
        [TestCaseSource("GetDeletedAccountListTestCaseTrue")]
        public async Task GetAccountListTestTrue(PaginationParameter paginationParameter, int expectedStatus)
        {          
            var rs = await accountController.GetDeactivatedAccountList(paginationParameter);
            var obResult = rs as ObjectResult;
            Assert.AreEqual(expectedStatus, obResult.StatusCode);
        }
    }
}