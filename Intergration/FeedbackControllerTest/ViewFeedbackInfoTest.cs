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
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace kroniiapiTest.Intergration.FeedbackControllerTest
{
    public class ViewFeedbackInfoTest
    {
        private DataContext _context;
        private FeedbackController fbController;
        private ClassService classService;
        private TraineeService traineeService;
        private readonly Mock<ITrainerService> mockTrainerService = new Mock<ITrainerService>();
        private readonly Mock<ITraineeService> mockTraineeService = new Mock<ITraineeService>();
        private readonly Mock<IFeedbackService> mockFeedbackService = new Mock<IFeedbackService>();
        private readonly Mock<IAdminService> mockAdminService = new Mock<IAdminService>();
        private readonly Mock<IMapper> mockMapper = new Mock<IMapper>();
        Admin testAdmin = new Admin()
        {
            AdminId = 4,
            Username = "khanhtoan",
            Fullname = "trankhanhtoan",
            Email = "toan@gmail.com",
            RoleId = 2,
            AvatarURL = "gg.com"
        };
        Trainer testTrainer = new Trainer()
        {
            TrainerId = 3,
            Username = "duykhang",
            Fullname = "thuongduykhang",
            Email = "khang@gmail.com",
            RoleId = 3,
            AvatarURL = "gg.com"
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
                ClassId = null
            }
        };
        Class testClass = new Class()
        {
            ClassId = 2,
            ClassName = "SE1501",
            AdminId = 4,
            TrainerId = 3
        };
        [SetUp]
        public void Setup()
        {
            var option = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "kroniiapi").Options;

            _context = new DataContext(option);
            _context.Trainees.AddRange(trainees);
            _context.Admins.AddRange(testAdmin);
            _context.Trainers.AddRange(testTrainer);
            _context.Classes.AddRange(testClass);
            _context.SaveChanges();
            classService = new ClassService(
                _context, mockMapper.Object,
                traineeService
            );
            traineeService = new TraineeService(_context);
            fbController = new FeedbackController(classService,
                                                  mockFeedbackService.Object,
                                                  mockMapper.Object,
                                                  mockAdminService.Object,
                                                  mockTrainerService.Object,
                                                  traineeService);
        }
        [TearDown]
        public void tearDown()
        {
            _context.Trainees.RemoveRange(_context.Trainees);
            _context.Classes.RemoveRange(_context.Classes);
            _context.Admins.RemoveRange(_context.Admins);
            _context.Trainers.RemoveRange(_context.Trainers);
            _context.SaveChanges();
        }
        public static IEnumerable<TestCaseData> ViewFeedbackInfoTestCaseTrue
        {
            get
            {
                yield return new TestCaseData(
                    1,
                    new FeedbackViewForTrainee()
                    {
                        trainer = new TrainerInFeedbackResponse()
                        {
                            TrainerId = 3,
                            Fullname = "thuongduykhang",
                            Email = "khang@gmail.com",
                            AvatarURL = "gg.com"
                        },
                        admin = new AdminInFeedbackResponse()
                        {
                            AdminId = 4,
                            Fullname = "trankhanhtoan",
                            Email = "toan@gmail.com",
                            AvatarURL = "gg.com"
                        }
                    }
                );
            }
        }
        [Test]
        [TestCaseSource("ViewFeedbackInfoTestCaseTrue")]
        public async Task ViewFeedbackInfoTestTrue(int traineeId,
                                                      FeedbackViewForTrainee expect)
        {
            var rs = await fbController.ViewFeedbackInfo(traineeId);
            var objResult = ((rs.Result as OkObjectResult).Value as FeedbackViewForTrainee);
            var expectJson = JsonConvert.SerializeObject(expect);
            var actualJson = JsonConvert.SerializeObject(objResult);
            Assert.AreEqual(expectJson, actualJson);
        }

        public static IEnumerable<TestCaseData> ViewFeedbackInfoTestCaseTraineeNotFound
        {
            get
            {
                yield return new TestCaseData(
                    2,
                    new ResponseDTO(404, "Trainee not found")
                );
                yield return new TestCaseData(
                    6,
                    new ResponseDTO(404, "Trainee does not have class")
                );
            }
        }
        [Test]
        [TestCaseSource("ViewFeedbackInfoTestCaseTraineeNotFound")]
        public async Task ViewFeedbackInfoTestTraineeNotFound(int traineeId,
                                                      ResponseDTO expect)
        {
            var rs = await fbController.ViewFeedbackInfo(traineeId);
            var objResult = ((rs.Result as NotFoundObjectResult).Value as ResponseDTO);
            var expectJson = JsonConvert.SerializeObject(expect);
            var actualJson = JsonConvert.SerializeObject(objResult);
            Assert.AreEqual(expectJson, actualJson);
        }
    }
}