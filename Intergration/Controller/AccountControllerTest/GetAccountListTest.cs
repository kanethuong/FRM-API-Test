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
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.DTO.Profiles;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace kroniiapiTest.Intergration.Controller.AccountControllerTest
{
    public class GetAccountListTest
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
            Username = "khanhtoan",
            Fullname = "trankhanhtoan",
            Email = "toan@gmail.com",
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
                new AdminService(_context,classService),
                new AdministratorService(_context),
                new CompanyService(_context),
                new TraineeService(_context),
                new TrainerService(_context,classService),
                mockEmailService.Object
            );
            accountController = new AccountController(accountService, mapper, mockEmailService.Object);
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
        public static IEnumerable<TestCaseData> GetAccountListTestCaseTrue
        {
            get
            {
                // True case: with PageNumber, PageSize and SearchName
                yield return new TestCaseData(
                    new PaginationParameter
                    {
                        PageNumber = 1,
                        PageSize = 4,
                        SearchName = ""
                    },
                    new PaginationResponse<IEnumerable<AccountResponse>>(4,
                    new List<AccountResponse>
                    {
                        new AccountResponse
                        {
                            AccountId = 1,
                            Username = "khanhtoan",
                            Fullname = "trankhanhtoan",
                            Email = "toan@gmail.com",
                            Role = "Admin"
                        },

                        new AccountResponse
                        {
                            AccountId = 1,
                            Username = "duykhang",
                            Fullname = "thuongduykhang",
                            Email = "khang@gmail.com",
                            Role = "Trainer"
                        },
                        new AccountResponse
                        {
                            AccountId = 1,
                            Username = "anhtho",
                            Fullname = "tieuanhtho",
                            Email = "tho@gmail.com",
                            Role = "Trainee"
                        },
                        new AccountResponse
                        {
                            AccountId = 1,
                            Username = "hailong",
                            Fullname = "lehoanghailong",
                            Email = "long@gmail.com",
                            Role = "Company"
                        }
                    })
                );
                yield return new TestCaseData(
                   new PaginationParameter
                   {
                       PageNumber = 2,
                       PageSize = 2,
                       SearchName = ""
                   },
                   new PaginationResponse<IEnumerable<AccountResponse>>(4,
                   new List<AccountResponse>
                   {
                        new AccountResponse
                        {
                            AccountId = 1,
                            Username = "anhtho",
                            Fullname = "tieuanhtho",
                            Email = "tho@gmail.com",
                            Role = "Trainee"
                        },
                        new AccountResponse
                        {
                            AccountId = 1,
                            Username = "hailong",
                            Fullname = "lehoanghailong",
                            Email = "long@gmail.com",
                            Role = "Company"
                        }
                   })
               );
                // True case: with SearchName
                yield return new TestCaseData(
                    new PaginationParameter
                    {

                        SearchName = "toan@gmail.com"
                    },
                    new PaginationResponse<IEnumerable<AccountResponse>>(1,
                    new List<AccountResponse>
                    {
                        new AccountResponse
                        {
                            AccountId = 1,
                            Username = "khanhtoan",
                            Fullname = "trankhanhtoan",
                            Email = "toan@gmail.com",
                            Role = "Admin"
                        }
                    })
                );
                // True case: with SearchName
                yield return new TestCaseData(
                    new PaginationParameter
                    {
                        PageNumber = 2,
                        PageSize = 1,
                        SearchName = ""
                    },
                    new PaginationResponse<IEnumerable<AccountResponse>>(4,
                        new List<AccountResponse>
                        {
                            new AccountResponse
                            {
                                AccountId = 1,
                                Username = "duykhang",
                                Fullname = "thuongduykhang",
                                Email = "khang@gmail.com",
                                Role = "Trainer"
                            }
                        })
                );
            }
        }
        [Test]
        [TestCaseSource("GetAccountListTestCaseTrue")]
        public async Task GetAccountListTestTrue(PaginationParameter paginationParameter, PaginationResponse<IEnumerable<AccountResponse>> expect)
        {
            var rs = await accountController.GetAccountList(paginationParameter);
            var objResult = ((rs.Result as OkObjectResult).Value as PaginationResponse<IEnumerable<AccountResponse>>);
            var expectJson = JsonConvert.SerializeObject(expect);
            var actualJson = JsonConvert.SerializeObject(objResult);
            Assert.AreEqual(expectJson, actualJson);
        }

        public static IEnumerable<TestCaseData> GetAccountListTestCaseFail
        {
            get
            {
                //True case: with PageNumber, PageSize and SearchName
                yield return new TestCaseData(
                    // this page is null
                    new PaginationParameter
                    {
                        PageNumber = 3,
                        PageSize = 4,
                        SearchName = "toan@gmail.com"
                    },
                    new PaginationResponse<IEnumerable<AccountResponse>>(1,
                        // Expect null list
                        new List<AccountResponse>
                        {

                        })
                );

                yield return new TestCaseData(
                    new PaginationParameter
                    {
                        PageNumber = 5,
                        PageSize = 1,
                        SearchName = ""
                    },
                    new PaginationResponse<IEnumerable<AccountResponse>>(4,
                        new List<AccountResponse>
                        {

                        })
                );
            }
        }
        [Test]
        [TestCaseSource("GetAccountListTestCaseFail")]
        public async Task GetAccountListTestFail(PaginationParameter paginationParameter, PaginationResponse<IEnumerable<AccountResponse>> expect)
        {
            var rs = await accountController.GetAccountList(paginationParameter);
            var objResult = ((rs.Result as OkObjectResult).Value as PaginationResponse<IEnumerable<AccountResponse>>);
            var expectJson = JsonConvert.SerializeObject(expect);
            var actualJson = JsonConvert.SerializeObject(objResult);
            Assert.AreEqual(expectJson, actualJson);
        }

        public static IEnumerable<TestCaseData> GetAccountListTestCaseFailSearchEmail
        {
            get
            {
                yield return new TestCaseData(
                    new PaginationParameter
                    {
                        PageNumber = 1,
                        PageSize = 2,
                        SearchName = "luonghoanghuong"
                    },
                    404
                );
            }
        }
        [Test]
        [TestCaseSource("GetAccountListTestCaseFailSearchEmail")]
        public async Task GetAccountListTestFailSearchEmail(PaginationParameter paginationParameter, int staCode)
        {
            var rs = await accountController.GetAccountList(paginationParameter);
            var actualRes = (rs.Result as NotFoundObjectResult).Value as ResponseDTO;
            Assert.AreEqual(staCode, actualRes.Status);
        }

    }
}