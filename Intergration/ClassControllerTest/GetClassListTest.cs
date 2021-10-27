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
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.DTO.Profiles;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework;

namespace kroniiapiTest.Intergration.ClassControllerTest
{
    public class GetClassListTest
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


        List<Class> classList = new List<Class>
        {
            new Class{
                ClassId = 1,
                ClassName = "fptunisersadadwaedsity",
                Description = "fptunisersity",
                CreatedAt = new DateTime(2021,10,18),
                StartDay = new DateTime(2021,10,18),
                EndDay = new DateTime(2024,10,18),
                IsDeactivated = false,
                AdminId = 1,
                TrainerId =1
            },
            new Class{
                ClassId = 2,
                ClassName = "abcxyz",
                Description = "fptunisersity",
                CreatedAt = new DateTime(2021,10,18),
                StartDay = new DateTime(2021,10,18),
                EndDay = new DateTime(2024,10,18),
                IsDeactivated = false,
                AdminId = 1,
                TrainerId = 1
            },

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
            DOB = new DateTime(2000, 1, 1),
            Address = "600, An Binh, Can Tho",
            Gender = "Male",
            Wage = 10000,
            CreatedAt = new DateTime(2021, 1, 1),
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
            DOB = new DateTime(2000, 2, 1),
            Address = "Nam Long, Quan 7, HCM",
            Gender = "Male",
            Wage = 10000,
            CreatedAt = new DateTime(2021, 1, 1),
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
            DOB = new DateTime(2000, 2, 1),
            Address = "Nam Long, Quan 7, HCM",
            Gender = "Male",
            Wage = 10000,
            CreatedAt = new DateTime(2021, 1, 1),
            IsDeactivated = true,
            RoleId = 3,
        };
        [OneTimeSetUp]
        public void setupFirst()
        {
            var option = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase("data").Options;
            dataContext = new DataContext(option);
            dataContext.Classes.AddRange(classList);
            dataContext.Admins.AddRange(adminsList);
            dataContext.Trainees.AddRange(traineeList);
            dataContext.Trainers.AddRange(trainerList);
            dataContext.SaveChanges();

            var config = new MapperConfiguration(config =>
            {
                config.AddProfile(new ClassProfile());
            });
            mapper = config.CreateMapper();

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
            classController = new ClassController(classService, traineeService, markService, adminService, moduleService, trainerService, feedbackService, mapper);
        }

        [OneTimeTearDown]
        public void tearDown()
        {
            dataContext.Classes.AddRange(classList);
            dataContext.Admins.RemoveRange(dataContext.Admins);
            dataContext.Trainees.RemoveRange(dataContext.Trainees);
            dataContext.Trainers.RemoveRange(dataContext.Trainers);
            dataContext.SaveChanges();
        }


        public static IEnumerable<TestCaseData> GetClassListTestCaseTrue
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
                //True case: With SearchName
                yield return new TestCaseData(
                   new PaginationParameter
                   {
                       SearchName = "abcd"
                   },
                   200
               );
            }
        }


        [Test]
        [TestCaseSource("GetClassListTestCaseTrue")]
        public async Task GetClassListTest_True(PaginationParameter paginationParameter, int expectedStatus)
        {
            var rs = await classController.GetClassList(paginationParameter);
            var obResult = rs.Result as ObjectResult;
            Assert.AreEqual(expectedStatus, obResult.StatusCode);
        }

    }
}