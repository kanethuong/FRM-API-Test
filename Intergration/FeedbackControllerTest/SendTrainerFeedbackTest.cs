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
    public class SendTrainerFeedbackTest
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
        Admin testAdmin = new Admin()
        {
            AdminId = 4,
            Username = "khanhtoan",
            Fullname = "trankhanhtoan",
            Email = "toan@gmail.com",
            RoleId = 2,
            AvatarURL = "gg.com"
        };
        List<Trainer> trainers = new List<Trainer>()
        {
            new Trainer()
            {
                TrainerId = 3,
                Username = "duykhang",
                Fullname = "thuongduykhang",
                Email = "khang@gmail.com",
                RoleId = 3,
                AvatarURL = "gg.com"
            },
            new Trainer()
            {
                TrainerId = 10,
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
                AdminId = 4,
                TrainerId = 3
            },
            new Class()
            {
                ClassId = 5,
                ClassName = "KR1501",
                AdminId = 4,
                TrainerId = 10
            }
        };
        TrainerFeedback tf = new TrainerFeedback()
        {
            Content = "Very bad",
            Rate = 1,
            TraineeId = 6,
            TrainerId = 10
        };
        [OneTimeSetUp]
        public void Setup()
        {
            var option = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "kroniiapi").Options;

            _context = new DataContext(option);
            _context.Trainees.AddRange(trainees);
            _context.Trainers.AddRange(trainers);
            _context.Classes.AddRange(classes);
            _context.TrainerFeedbacks.AddRange(tf);
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
            _context.TrainerFeedbacks.RemoveRange(_context.TrainerFeedbacks);
            _context.Trainees.RemoveRange(_context.Trainees);
            _context.Classes.RemoveRange(_context.Classes);
            _context.Trainers.RemoveRange(_context.Trainers);
            _context.SaveChanges();
        }
        public static IEnumerable<TestCaseData> SendTrainerFeedbackTestCaseTrue
        {
            get
            {
                yield return new TestCaseData(

                    new TrainerFeedbackInput()
                    {
                        Content = "Very good",
                        Rate = 5,
                        TraineeId = 1,
                        TrainerId = 3
                    },
                    new TrainerFeedback()
                    {
                        Content = "Very good",
                        Rate = 5,
                        TraineeId = 1,
                        TrainerId = 3
                    },
                    new ResponseDTO(200, "Success")
                );
            }
        }
        [Test]
        [TestCaseSource("SendTrainerFeedbackTestCaseTrue")]
        public async Task SendTrainerFeedbackTestTrue(TrainerFeedbackInput input, TrainerFeedback expectFb, ResponseDTO expect)
        {
            var rs = await fbController.SendTrainerFeedback(input);
            var objResult = (rs as OkObjectResult).Value;
            var expectJson = JsonConvert.SerializeObject(expect);
            var actualJson = JsonConvert.SerializeObject(objResult);
            var fbRs = await _context.TrainerFeedbacks.FirstOrDefaultAsync(f => f.TrainerId == input.TrainerId
                                                           && f.TraineeId == input.TraineeId);
            Assert.True(expectJson == actualJson
                        && expectFb.Rate == fbRs.Rate
                        && expectFb.Content == fbRs.Content
                        && expectFb.TraineeId == fbRs.TraineeId
                        && expectFb.TrainerId == fbRs.TrainerId
                        );
        }

        public static IEnumerable<TestCaseData> SendTrainerFeedbackTestCaseFail
        {
            get
            {
                yield return new TestCaseData(
                    new TrainerFeedbackInput()
                    {
                        Content = "Very good",
                        Rate = 5,
                        TraineeId = 6,
                        TrainerId = 3
                    },
                    new ResponseDTO(404, "Trainer doesn't train Trainee")
                );
                yield return new TestCaseData(
                    new TrainerFeedbackInput()
                    {
                        Content = "Very good",
                        Rate = 6,
                        TraineeId = 1,
                        TrainerId = 3
                    },
                    new ResponseDTO(400, "Rate must be between 1 and 5")
                );
                yield return new TestCaseData(
                    new TrainerFeedbackInput()
                    {
                        Content = "Very good",
                        Rate = 5,
                        TraineeId = 6,
                        TrainerId = 10
                    },
                    new ResponseDTO(404, "Trainee has feedback this Trainer")
                );
                yield return new TestCaseData(
                    new TrainerFeedbackInput()
                    {
                        Content = "Very good",
                        Rate = 5,
                        TraineeId = 100,
                        TrainerId = 10
                    },
                    new ResponseDTO(404, "Don't have this Trainee")
                );
                yield return new TestCaseData(
                    new TrainerFeedbackInput()
                    {
                        Content = "Very good",
                        Rate = 5,
                        TraineeId = 6,
                        TrainerId = 100
                    },
                    new ResponseDTO(404, "Don't have this Trainer")
                );
            }
        }
        [Test]
        [TestCaseSource("SendTrainerFeedbackTestCaseFail")]
        public async Task SendTrainerFeedbackTestFail(TrainerFeedbackInput input, ResponseDTO expect)
        {
            var rs = await fbController.SendTrainerFeedback(input);
            var objResult = (rs as ObjectResult).Value;
            var expectJson = JsonConvert.SerializeObject(expect);
            var actualJson = JsonConvert.SerializeObject(objResult);
            Assert.AreEqual(expectJson, actualJson);
        }
    }
}