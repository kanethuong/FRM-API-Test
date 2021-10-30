using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.FeedbackDTO;
using kroniiapi.DTO.Profiles;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace kroniiapitest.Intergration.FeedbackControllerTest
{
    public class SendAdminFeedbackTest
    {
        private DataContext _context;
        private readonly Mock<ITrainerService> mockTrainerService = new Mock<ITrainerService>();
        private readonly Mock<ITraineeService> mockTraineeService = new Mock<ITraineeService>();
        private readonly Mock<IClassService> mockClassService = new Mock<IClassService>();
        private readonly Mock<IAdminService> mockAdminService = new Mock<IAdminService>();
        private FeedbackController fbController;
        private FeedbackService feedbackService;
        private TraineeService traineeService;
        private IMapper mapper;
    
        List<Admin> admins = new List<Admin>()
        {
            new Admin()
            {
                 AdminId = 3,
            Username = "khanhtoan",
            Fullname = "trankhanhtoan",
            Email = "toan@gmail.com",
            RoleId = 2,
            AvatarURL = "gg.com"
            },
            new Admin()
            {
                AdminId = 10,
                Username = "phu",
                Fullname = "thienphu",
                Email = "phu@gmail.com",
                RoleId = 3,
                AvatarURL = "gg.com"
            }
        };

        List<Trainee> trainees = new List<Trainee>()
        {
            new Trainee()
            {
                TraineeId = 1,
                Username = "anhtho",
                Fullname = "tieuanhtho",
                RoleId = 4,
                ClassId = 2
            },
            new Trainee()
            {
                TraineeId = 6,
                Username = "minh",
                Fullname = "khaminh",
                RoleId = 4,
                ClassId = 5
            }
        };
        List<Class> classes = new List<Class>(){
            new Class()
            {
                ClassId = 2,
                ClassName = "SE1501",
                AdminId = 3,
                TrainerId = 4
            },
            new Class()
            {
                ClassId = 5,
                ClassName = "KR1501",
                AdminId = 10,
                TrainerId = 4
            }
        };
        AdminFeedback af = new AdminFeedback()
        {
            Content = "Very bad",
            Rate = 1,
            TraineeId = 6,
            AdminId = 10
        };
        [OneTimeSetUp]
        public void Setup()
        {
            var option = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "kroniiapi").Options;

            _context = new DataContext(option);
            _context.Trainees.AddRange(trainees);
            _context.Admins.AddRange(admins);
            _context.Classes.AddRange(classes);
            _context.AdminFeedbacks.AddRange(af);
            _context.SaveChanges();
            var config = new MapperConfiguration(config =>
            {
                config.AddProfile(new FeedbackProfile());
            });
            mapper = config.CreateMapper();
            feedbackService = new FeedbackService(_context);
            // traineeService = new TraineeService(_context);
            mockTraineeService.Setup(x => x.GetClassIdByTraineeId(1)).ReturnsAsync((1, ""));
            fbController = new FeedbackController(mockClassService.Object,
                                                  feedbackService,
                                                  mapper,
                                                  mockAdminService.Object,
                                                  mockTrainerService.Object,
                                                  mockTraineeService.Object);
        }
        [OneTimeTearDown]
        public void tearDownAll()
        {
            _context.AdminFeedbacks.RemoveRange(_context.AdminFeedbacks);
            _context.Trainees.RemoveRange(_context.Trainees);
            _context.Classes.RemoveRange(_context.Classes);
            _context.Admins.RemoveRange(_context.Admins);
            _context.SaveChanges();
        }
        public static IEnumerable<TestCaseData> SendAdminFeedbackTestCaseTrue
        {
            get
            {
                yield return new TestCaseData(

                    new AdminFeedbackInput()
                    {
                        Content = "Very good",
                        Rate = 5,
                        TraineeId = 1,
                        AdminId = 3
                    },
                    new AdminFeedback()
                    {
                        Content = "Very good",
                        Rate = 5,
                        TraineeId = 1,
                        AdminId = 3
                    },
                    new ResponseDTO(200, "Success")
                );
            }
        }
        [Test]
        [TestCaseSource("SendAdminFeedbackTestCaseTrue")]
        public async Task SendAdminFeedbackTestTrue(AdminFeedbackInput input, AdminFeedback expectFb, ResponseDTO expect)
        {
            var rs = await fbController.SendAdminFeedback(input);
            var objResult = (rs as ObjectResult).Value;
            var expectJson = JsonConvert.SerializeObject(expect);
            var actualJson = JsonConvert.SerializeObject(objResult);
            var fbRs = await _context.AdminFeedbacks.FirstOrDefaultAsync(f => f.AdminId == input.AdminId
                                                           && f.TraineeId == input.TraineeId);
            Assert.True(expectJson == actualJson
                        && expectFb.Rate == fbRs.Rate
                        && expectFb.Content == fbRs.Content
                        && expectFb.TraineeId == fbRs.TraineeId
                        && expectFb.AdminId == fbRs.AdminId
                        );
        }

        public static IEnumerable<TestCaseData> SendAdminFeedbackTestCaseFail
        {
            get
            {
                yield return new TestCaseData(
                    new AdminFeedbackInput()
                    {
                        Content = "Very good",
                        Rate = 5,
                        TraineeId = 6,
                        AdminId = 3
                    },
                    new ResponseDTO(404, "Admin doesn't train Trainee")
                );
                yield return new TestCaseData(
                    new AdminFeedbackInput()
                    {
                        Content = "Very good",
                        Rate = 6,
                        TraineeId = 1,
                        AdminId = 3
                    },
                    new ResponseDTO(400, "Rate must be between 1 and 5")
                );
                yield return new TestCaseData(
                    new AdminFeedbackInput()
                    {
                        Content = "Very good",
                        Rate = 5,
                        TraineeId = 6,
                        AdminId = 10
                    },
                    new ResponseDTO(404, "Trainee has feedback this Admin")
                );
                yield return new TestCaseData(
                    new AdminFeedbackInput()
                    {
                        Content = "Very good",
                        Rate = 5,
                        TraineeId = 100,
                        AdminId = 10
                    },
                    new ResponseDTO(404, "Don't have this Trainee")
                );
                yield return new TestCaseData(
                    new AdminFeedbackInput()
                    {
                        Content = "Very good",
                        Rate = 5,
                        TraineeId = 6,
                        AdminId = 100
                    },
                    new ResponseDTO(404, "Don't have this Admin")
                );
            }
        }
        [Test]
        [TestCaseSource("SendAdminFeedbackTestCaseFail")]
        public async Task SendAdminFeedbackTestFail(AdminFeedbackInput input, ResponseDTO expect)
        {
            var rs = await fbController.SendAdminFeedback(input);
            var objResult = (rs as ObjectResult).Value;
            var expectJson = JsonConvert.SerializeObject(expect);
            var actualJson = JsonConvert.SerializeObject(objResult);
            Assert.AreEqual(expectJson, actualJson);
        }
    }
}