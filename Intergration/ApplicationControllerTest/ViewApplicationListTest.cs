using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.ApplicationDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Helper.Upload;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace kroniiapiTest.Intergration.ApplicationControllerTest
{
    public class ViewApplicationListTest
    {
        private DataContext _context;
        private ApplicationController appController;
        private TraineeService traineeService;
        private readonly Mock<IApplicationService> mockAppService = new Mock<IApplicationService>();
        private readonly Mock<IMapper> mockMapper = new Mock<IMapper>();
        private readonly Mock<IMegaHelper> mockMegaHelper = new Mock<IMegaHelper>();
        private readonly List<ApplicationCategory> types = new List<ApplicationCategory>() {
            new ApplicationCategory() {
                ApplicationCategoryId = 3,
                CategoryName = "Loai 3"
            },
            new ApplicationCategory() {
                ApplicationCategoryId = 4,
                CategoryName = "Loai 4"
            },
        };
        private readonly List<Trainee> trainees = new List<Trainee>() {
            new Trainee() {
                TraineeId = 1,
            Username = "anhtho",
            Fullname = "tieuanhtho",
            Email = "tho@gmail.com",
            RoleId = 4
            },
            new Trainee() {
               TraineeId = 2,
            Username = "khanhtoan",
            Fullname = "trankhanhtoan",
            Email = "toan@gmail.com",
            RoleId = 4
            }
        };
        private readonly List<Application> apps = new List<Application>() {
            new Application() {
                ApplicationId=1,
                Description="Xin nghi hoc",
                ApplicationURL="gg.com",
                ApplicationCategoryId=3,
                TraineeId = 1,
                IsAccepted =false
            },
            new Application() {
                ApplicationId=2,
                Description="Xin chuyen lop",
                ApplicationURL="gg.com",
                ApplicationCategoryId=4,
                TraineeId = 1,
                IsAccepted= true
            },
            new Application() {
                ApplicationId=3,
                Description="Xac nhan sinh vien",
                ApplicationURL="gg.com",
                ApplicationCategoryId=3,
                TraineeId = 1,
                IsAccepted= null
            }
        };
        [SetUp]
        public void Setup()
        {
            var option = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "kroniiapi").Options;

            _context = new DataContext(option);
            _context.Trainees.AddRange(trainees);
            _context.ApplicationCategories.AddRange(types);
            _context.Applications.AddRange(apps);
            _context.SaveChanges();
            traineeService = new TraineeService(
                _context
            );
            appController = new ApplicationController(mockMapper.Object, traineeService, mockAppService.Object, mockMegaHelper.Object);
        }
        [TearDown]
        public void tearDown()
        {
            _context.Applications.RemoveRange(_context.Applications);
            _context.ApplicationCategories.RemoveRange(_context.ApplicationCategories);
            _context.Trainees.RemoveRange(_context.Trainees);
            _context.SaveChanges();
        }
        public static IEnumerable<TestCaseData> ViewApplicationListTestCaseTrue
        {
            get
            {
                // True case: with PageNumber, PageSize and SearchName
                yield return new TestCaseData(
                    1,
                    new PaginationParameter
                    {
                        PageNumber = 1,
                        PageSize = 3,
                    },
                    new PaginationResponse<IEnumerable<TraineeApplicationResponse>>(3,
                        new List<TraineeApplicationResponse>
                        {
                            new TraineeApplicationResponse
                            {
                                Description = "Xin nghi hoc",
                                ApplicationURL = "gg.com",
                                Type = "Loai 3",
                                IsAccepted = false,
                            },
                            new TraineeApplicationResponse
                            {
                                Description = "Xin chuyen lop",
                                ApplicationURL = "gg.com",
                                Type = "Loai 4",
                                IsAccepted = true,
                            },
                            new TraineeApplicationResponse
                            {
                                Description = "Xac nhan sinh vien",
                                ApplicationURL = "gg.com",
                                Type = "Loai 3",
                                IsAccepted = null,
                            },
                        })
                );
                // True case: with SearchName
                yield return new TestCaseData(
                    1,
                    new PaginationParameter
                    {
                        PageNumber = 1,
                        PageSize = 10,
                        SearchName ="4"
                    },
                    new PaginationResponse<IEnumerable<TraineeApplicationResponse>>(1,
                        new List<TraineeApplicationResponse>
                        {
                            new TraineeApplicationResponse
                            {
                                Description = "Xin chuyen lop",
                                ApplicationURL = "gg.com",
                                Type = "Loai 4",
                                IsAccepted = true,
                            }
                        })
                );
            }
        }

        [Test]
        [TestCaseSource("ViewApplicationListTestCaseTrue")]
        public async Task ViewApplicationListTestTrue(int traineeId,
                                                      PaginationParameter paginationParameter,
                                                      PaginationResponse<IEnumerable<TraineeApplicationResponse>> expect)
        {
            var rs = await appController.ViewApplicationList(traineeId,paginationParameter);
            var objResult = ((rs.Result as OkObjectResult).Value as PaginationResponse<IEnumerable<TraineeApplicationResponse>>);
            var expectJson = JsonConvert.SerializeObject(expect);
            var actualJson = JsonConvert.SerializeObject(objResult);
            Assert.AreEqual(expectJson, actualJson);
        }

        public static IEnumerable<TestCaseData> ViewApplicationListTestCaseFail
        {
            get
            {
                // True case: with PageNumber, PageSize and SearchName
                yield return new TestCaseData(
                    3,
                    new PaginationParameter
                    {
                        PageNumber = 1,
                        PageSize = 1,
                    },
                    new ResponseDTO(404, "id not found")
                );
                // True case: with SearchName
                yield return new TestCaseData(
                    2,
                    new PaginationParameter
                    {
                        PageNumber = 1,
                        PageSize = 10,
                    },
                    new ResponseDTO(404, "Trainee doesn't have any application")
                );
            }
        }

        [Test]
        [TestCaseSource("ViewApplicationListTestCaseFail")]
        public async Task ViewApplicationListTestFail(int traineeId,
                                                      PaginationParameter paginationParameter,
                                                      ResponseDTO expect)
        {
            var rs = await appController.ViewApplicationList(traineeId,paginationParameter);
            var objResult = ((rs.Result as NotFoundObjectResult).Value as ResponseDTO);
            var expectJson = JsonConvert.SerializeObject(expect);
            var actualJson = JsonConvert.SerializeObject(objResult);
            Assert.AreEqual(expectJson, actualJson);
        }
    }
}