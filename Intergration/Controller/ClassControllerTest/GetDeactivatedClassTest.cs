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

namespace kroniiapiTest.Intergration.Controller.ClassControllerTest
{
    public class GetDeactivatedClassTest
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
        List<Class> classList = new List<Class>
        {
            new Class{
                ClassId = 1,
                ClassName = "fptunisersadadwaedsity",
                Description = "fptunisersity",
                CreatedAt = new DateTime(2021,10,18),
                StartDay = new DateTime(2021,10,18),
                EndDay = new DateTime(2024,10,18),
                IsDeactivated = true,
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
            new Class{
                ClassId = 3,
                ClassName = "sdvaszdfvsdfv",
                Description = "fptunisersdvsdvsity",
                CreatedAt = new DateTime(2021,10,18),
                StartDay = new DateTime(2021,10,18),
                EndDay = new DateTime(2024,10,18),
                IsDeactivated = true,
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
        [OneTimeSetUp]
        public void setupFirst() {
            var option = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase("data").Options;
            dataContext = new DataContext(option);
            dataContext.Roles.AddRange(roleList);
            dataContext.Classes.AddRange(classList);
            dataContext.Admins.AddRange(adminsList);
            dataContext.Trainees.AddRange(traineeList);
            dataContext.Trainers.AddRange(trainerList);
            dataContext.SaveChanges();

            var config = new MapperConfiguration(config => {
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
                traineeService
            );
            classController = new ClassController(classService,traineeService,markService,adminService,moduleService, trainerService,feedbackService, mapper);
        }
        
        [OneTimeTearDown]
        public void tearDown() {
            dataContext.Classes.RemoveRange(dataContext.Classes);
            dataContext.Roles.RemoveRange(dataContext.Roles);
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
                        PageSize = 3,
                        SearchName = ""
                    },
                    200,
                    new PaginationResponse<IEnumerable<DeleteClassResponse>>(2,
                        new List<DeleteClassResponse>{
                            new DeleteClassResponse{
                                ClassId = 1,
                                ClassName = "fptunisersadadwaedsity",
                                Description = "fptunisersity"
                            },
                            new DeleteClassResponse{
                                ClassId = 3,
                                ClassName = "sdvaszdfvsdfv",
                                Description = "fptunisersdvsdvsity"
                            }
                        }
                    )
                );
                
            }
        }
        

        [Test]
        [TestCaseSource("GetClassListTestCaseTrue")]
        public async Task GetClassListTest_True(PaginationParameter paginationParameter, int expectedStatus,PaginationResponse<IEnumerable<DeleteClassResponse>> paginationResponseTrue)
        {          
            var rs = await classController.GetDeactivatedClass(paginationParameter);
            var obResult = rs.Result as ObjectResult;
            var listResult = ((rs.Result as OkObjectResult).Value  as PaginationResponse<IEnumerable<DeleteClassResponse>>);
            var actualJson = JsonConvert.SerializeObject(listResult);
            var expectedJson = JsonConvert.SerializeObject(paginationResponseTrue);
            Assert.AreEqual(expectedJson, actualJson);
        }

        public static IEnumerable<TestCaseData> GetClassListTestCaseFail
        {
            get
            {
                // True case: with PageNumber, PageSize and SearchName
                yield return new TestCaseData(
                    new PaginationParameter
                    {
                        PageNumber = 4,
                        PageSize = 3,
                        SearchName = ""
                    },
                    404,
                    new PaginationResponse<IEnumerable<DeleteClassResponse>>(2,
                        new List<DeleteClassResponse>{
                            
                        }
                    )
                );
            }
        }



        [Test]
        [TestCaseSource("GetClassListTestCaseFail")]
        public async Task GetClassListTest_Fail(PaginationParameter paginationParameter, int expectedStatus,PaginationResponse<IEnumerable<DeleteClassResponse>> paginationResponseFail)
        {          
            var rs = await classController.GetDeactivatedClass(paginationParameter);
            var obResult = rs.Result as ObjectResult;
            var listResult = ((rs.Result as OkObjectResult).Value  as PaginationResponse<IEnumerable<DeleteClassResponse>>);
            var actualJson = JsonConvert.SerializeObject(listResult);
            var expectedJson = JsonConvert.SerializeObject(paginationResponseFail);
            Assert.AreEqual(expectedJson, actualJson);
        }
    }
}