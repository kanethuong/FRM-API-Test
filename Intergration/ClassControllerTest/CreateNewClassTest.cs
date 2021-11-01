using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.ClassDTO;
using kroniiapi.DTO.Profiles;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace kroniiapitest.Intergration.ClassControllerTest
{
    public class CreateNewClassTest
    {
        private IClassService classService;
        private IAdminService adminService;
        private ITrainerService trainerService;
        private IMarkService markService;
        private IModuleService moduleService;
        private IFeedbackService feedbackService;
        private IMapper mapper;
        private ITraineeService traineeService;
        private DataContext dataContext;
        private ClassController classController;

        private Class classList = new Class()
        {
            ClassName = "Math",
            Description = "Easy",
            TrainerId = 1,
            AdminId = 1,
            RoomId = 1,
            StartDay = new DateTime(2021, 10, 28),
            EndDay = new DateTime(2021, 11, 28)
        };

        private List<Admin> adminList = new List<Admin>()
        {
            new Admin{
                AdminId = 1,
                IsDeactivated = true
            },
            new Admin{
                AdminId = 2,
                IsDeactivated = false
            }
        };
        private List<Trainee> traineeList = new List<Trainee>()
        {
            new Trainee{
                TraineeId = 1,
                IsDeactivated = true
            },
            new Trainee{
                TraineeId = 2,
                IsDeactivated = false
            }
        };
        private List<Trainer> trainerList = new List<Trainer>()
        {
            new Trainer{
                TrainerId = 1,
                IsDeactivated = true
            },
            new Trainer{
                TrainerId = 2,
                IsDeactivated = false
            }
        };

        private Module moduleList = new Module
        {
            ModuleId = 1
        };

        [OneTimeSetUp]
        public void setupFirst()
        {
            var option = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase("data").Options;
            dataContext = new DataContext(option);
            dataContext.Classes.AddRange(classList);
            dataContext.Admins.AddRange(adminList);
            dataContext.Trainees.AddRange(traineeList);
            dataContext.Trainers.AddRange(trainerList);
            dataContext.SaveChanges();

            var config = new MapperConfiguration(config =>
            {
                config.AddProfile(new ClassProfile());
            });
            mapper = config.CreateMapper();

            traineeService = new TraineeService(dataContext);
            trainerService = new TrainerService(
                dataContext,
                classService
            );
            adminService = new AdminService(
                dataContext,
                classService
            );
            classService = new ClassService(
                dataContext,
                mapper,
                new TraineeService(dataContext)
            );
            classController = new ClassController(classService,
                                                  traineeService,
                                                  markService,
                                                  adminService,
                                                  moduleService,
                                                  trainerService,
                                                  feedbackService,
                                                  mapper);
        }

        [OneTimeTearDown]
        public void tearDown()
        {
            dataContext.Classes.RemoveRange(dataContext.Classes);
            dataContext.Admins.RemoveRange(dataContext.Admins);
            dataContext.Trainees.RemoveRange(dataContext.Trainees);
            dataContext.Trainers.RemoveRange(dataContext.Trainers);
            dataContext.SaveChanges();
        }

        public static IEnumerable<TestCaseData> CreateNewClassTestCases
        {
            get
            {
                // True case
                yield return new TestCaseData(
                    new NewClassInput
                    {
                        ClassName = "Physic",
                        Description = "Easy",
                        TrainerId = 2,
                        AdminId = 2,
                        RoomId = 1,
                        StartDay = DateTime.Now,
                        EndDay = DateTime.Now.AddMonths(1),
                        TraineeIdList = new List<int>() { 2 },
                        ModuleIdList = new List<int>() { 1 }
                    },
                    201
                );

                //Fail case: class name duplicate
                yield return new TestCaseData(
                    new NewClassInput
                    {
                        ClassName = "Math",
                        Description = "Easy",
                        TrainerId = 2,
                        AdminId = 2,
                        RoomId = 1,
                        StartDay = DateTime.Now,
                        EndDay = DateTime.Now.AddMonths(1),
                        TraineeIdList = new List<int>() { 2 },
                        ModuleIdList = new List<int>() { 1 }
                    },
                    409
                );

                //Fail case: trainer not exist
                yield return new TestCaseData(
                    new NewClassInput
                    {
                        ClassName = "Physic",
                        Description = "Easy",
                        TrainerId = 3,
                        AdminId = 2,
                        RoomId = 1,
                        StartDay = DateTime.Now,
                        EndDay = DateTime.Now.AddMonths(1),
                        TraineeIdList = new List<int>() { 2 },
                        ModuleIdList = new List<int>() { 1 }
                    },
                    404
                );

                //Fail case: trainer is deactivated
                yield return new TestCaseData(
                    new NewClassInput
                    {
                        ClassName = "Physic",
                        Description = "Easy",
                        TrainerId = 1,
                        AdminId = 2,
                        RoomId = 1,
                        StartDay = DateTime.Now,
                        EndDay = DateTime.Now.AddMonths(1),
                        TraineeIdList = new List<int>() { 2 },
                        ModuleIdList = new List<int>() { 1 }
                    },
                    404
                );

                //Fail case: admin is not exist
                yield return new TestCaseData(
                    new NewClassInput
                    {
                        ClassName = "Physic",
                        Description = "Easy",
                        TrainerId = 2,
                        AdminId = 3,
                        RoomId = 1,
                        StartDay = DateTime.Now,
                        EndDay = DateTime.Now.AddMonths(1),
                        TraineeIdList = new List<int>() { 2 },
                        ModuleIdList = new List<int>() { 1 }
                    },
                    404
                );

                //Fail case: admin is deactivated
                yield return new TestCaseData(
                    new NewClassInput
                    {
                        ClassName = "Physic",
                        Description = "Easy",
                        TrainerId = 2,
                        AdminId = 1,
                        RoomId = 1,
                        StartDay = DateTime.Now,
                        EndDay = DateTime.Now.AddMonths(1),
                        TraineeIdList = new List<int>() { 2 },
                        ModuleIdList = new List<int>() { 1 }
                    },
                    404
                );

                //Fail case: start day in the past
                yield return new TestCaseData(
                    new NewClassInput
                    {
                        ClassName = "Physic",
                        Description = "Easy",
                        TrainerId = 2,
                        AdminId = 2,
                        RoomId = 1,
                        StartDay = new DateTime(2021,02,02),
                        EndDay = DateTime.Now.AddMonths(1),
                        TraineeIdList = new List<int>() { 2 },
                        ModuleIdList = new List<int>() { 1 }
                    },
                    409
                );

                //Fail case: end day less than start day
                yield return new TestCaseData(
                    new NewClassInput
                    {
                        ClassName = "Physic",
                        Description = "Easy",
                        TrainerId = 2,
                        AdminId = 2,
                        RoomId = 1,
                        StartDay = new DateTime(2021,02,02),
                        EndDay = new DateTime(2021,01,01),
                        TraineeIdList = new List<int>() { 2 },
                        ModuleIdList = new List<int>() { 1 }
                    },
                    409
                );

                // Fail case: trainee not exist
                yield return new TestCaseData(
                    new NewClassInput
                    {
                        ClassName = "Physic",
                        Description = "Easy",
                        TrainerId = 2,
                        AdminId = 2,
                        RoomId = 1,
                        StartDay = DateTime.Now,
                        EndDay = DateTime.Now.AddMonths(1),
                        TraineeIdList = new List<int>() { 3 },
                        ModuleIdList = new List<int>() { 1 }
                    },
                    404
                );

                // Fail case: trainee is deactivated
                yield return new TestCaseData(
                    new NewClassInput
                    {
                        ClassName = "Physic",
                        Description = "Easy",
                        TrainerId = 2,
                        AdminId = 2,
                        RoomId = 1,
                        StartDay = DateTime.Now,
                        EndDay = DateTime.Now.AddMonths(1),
                        TraineeIdList = new List<int>() { 1 },
                        ModuleIdList = new List<int>() { 1 }
                    },
                    404
                );

                // Fail case: module is not exist
                yield return new TestCaseData(
                    new NewClassInput
                    {
                        ClassName = "Physic",
                        Description = "Easy",
                        TrainerId = 2,
                        AdminId = 2,
                        RoomId = 1,
                        StartDay = DateTime.Now,
                        EndDay = DateTime.Now.AddMonths(1),
                        TraineeIdList = new List<int>() { 2 },
                        ModuleIdList = new List<int>() { 2 }
                    },
                    409
                );
            }
        }

        [Test]
        [TestCaseSource("CreateNewClassTestCases")]
        public async Task CreateNewClass_Test(NewClassInput newClassInput, int expStatus)
        {
            // Act
            var rs = await classController.CreateNewClass(newClassInput) as ObjectResult;
            var response = rs.Value as ResponseDTO;

            // Assert
            Assert.True(
                expStatus == rs.StatusCode &&
                expStatus == response.Status
            );
        }

    }
}