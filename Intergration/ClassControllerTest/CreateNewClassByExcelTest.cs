using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.Profiles;
using kroniiapi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using OfficeOpenXml;

namespace kroniiapitest.Intergration.ClassControllerTest
{
    [TestFixture]
    public class CreateNewClassByExcelTest
    {
        private  DataContext dataContext;
        private  IClassService classService;
        private  IAdminService adminService;
        private  ITrainerService trainerService;
        private  IMarkService markService;
        private  IModuleService moduleService;
        private  IFeedbackService feedbackService;
        private  IMapper mapper;
        private  ITraineeService traineeService;
        private  ClassController classController;
        private ITimetableService timetableService;

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

        Trainee traineeList = new Trainee
        {
            TraineeId = 1,
            Username = "congvinhkosaoaaaaaaaaaaaaaake",
            Password = "654321",
            Fullname = "LeCongVinh",
            AvatarURL = "none",
            Email = "mck123@gmail.com",
            Phone = "029288290",
            DOB = new DateTime(2000, 2, 1),
            Address = "Nam Long, Quan 7, HCM",
            Gender = "Male",
            Wage = 10000,
            CreatedAt = new DateTime(2021, 1, 1),
            IsDeactivated = false,
            RoleId = 4,
        };

        List<Module> moduleList = new List<Module>
        {
            new Module
            {
                ModuleId = 2,
                ModuleName = "test"
            },
            new Module
            {
                ModuleId = 3,
                ModuleName = "testaaa"
            }
        };

        Admin adminsList = new Admin
        {
            AdminId = 1,
            Username = "fptunise454264344rsity",
            Password = "fptunisersity",
            Fullname = "DaiHocFPTCanTho",
            AvatarURL = "none",
            Email = "testUserCornn@fpt.edu.vn",
            Phone = "02927303636",
            DOB = new DateTime(2000, 1, 1),
            Address = "600, An Binh, Can Tho",
            Gender = "Male",
            Wage = 10000,
            CreatedAt = new DateTime(2021, 1, 1),
            IsDeactivated = false,
            RoleId = 2,
        };

        Trainer trainerList = new Trainer
        {
            TrainerId = 1,
            Username = "congvinhkosnsajndaoke",
            Password = "654321",
            Fullname = "LeCongVinh",
            AvatarURL = "none",
            Email = "hostcode0301@gmail.com",
            Phone = "029288290",
            DOB = new DateTime(2000, 2, 1),
            Address = "Nam Long, Quan 7, HCM",
            Gender = "Male",
            Wage = 10000,
            CreatedAt = new DateTime(2021, 1, 1),
            IsDeactivated = false,
            RoleId = 3,
        };

        List<Class> classList = new List<Class>
        {
            new Class{
                ClassId = 1,
                ClassName = "fptclass",
                Description = "fptunisersity",
                CreatedAt = new DateTime(2021,10,18),
                StartDay = new DateTime(2021,10,18),
                EndDay = new DateTime(2024,10,18),
                IsDeactivated = false,
                AdminId = 1,
                TrainerId =1
            }
        };

        [OneTimeSetUp]
        public void setupFirst()
        {
            var option = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase("data").Options;
            dataContext = new DataContext(option);
            dataContext.Roles.AddRange(roleList);
            dataContext.Admins.AddRange(adminsList);
            dataContext.Trainees.AddRange(traineeList);
            dataContext.Trainers.AddRange(trainerList);
            dataContext.Modules.AddRange(moduleList);
            dataContext.Classes.AddRange(classList);
            dataContext.SaveChanges();

            var config = new MapperConfiguration(config =>
            {
                config.AddProfile(new ClassProfile());
            });
            mapper = config.CreateMapper();

            classService = new ClassService(
                dataContext,
                mapper,
                new TraineeService(dataContext)

            );

            traineeService = new TraineeService(
                dataContext
            );

            markService = new MarkService(
                dataContext
            );

            adminService = new AdminService(
                dataContext,
                classService
            );

            moduleService = new ModuleService(
                dataContext
            );

            trainerService = new TrainerService(
                dataContext,
                classService
            );

            feedbackService = new FeedbackService(
                dataContext
            );
            classController = new ClassController(classService, 
                traineeService, adminService, moduleService, trainerService, mapper,timetableService);
        }

        [OneTimeTearDown]
        public void tearDown()
        {
            dataContext.Classes.RemoveRange(dataContext.Classes);
            dataContext.Trainees.RemoveRange(traineeList);
            dataContext.Admins.RemoveRange(adminsList);
            dataContext.Trainers.RemoveRange(trainerList);
            dataContext.Modules.RemoveRange(moduleList);
            
            dataContext.SaveChanges();
        }

        public static IEnumerable<TestCaseData> CreateNewClassByExcelTestCaseTrue
        {
            get
            {
                
                yield return new TestCaseData(
                    "\\FileForTest\\CreateClassTestTrue.xlsx",
                    201
                );
                yield return new TestCaseData(
                    "\\FileForTest\\CreateClassClassNameExist.xlsx",
                    409
                );
                yield return new TestCaseData(  //File content inapproriate case
                    "\\FileForTest\\CreateAccTestTrue.xlsx",
                    400
                );
            
                yield return new TestCaseData(  
                    "\\FileForTest\\CreateClassTrainerAlreadyHaveClass.xlsx",
                    409
                );
                yield return new TestCaseData(
                    "\\FileForTest\\CreateAccWrongExtension.docx",
                    400
                );
                yield return new TestCaseData(
                    "\\FileForTest\\CreateAccTestFakeExtension.xlsx",
                    400
                );
            }
        }

        [Test]
        [TestCaseSource("CreateNewClassByExcelTestCaseTrue")]
        public async Task CreateNewAccountByExcelTestTrue(string pathTest, int expectedStatus)
        {
            // Arrange
            //string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"CreateAccTestTrue.xlsx");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            string pathToTest = projectDirectory + pathTest;

            var stream = File.OpenRead(pathToTest);
            IFormFile file = new FormFile(stream, 0, stream.Length, "CreateClassTestTrue", "CreateClassTestTrue.xls");
            // Act
            var rs = await classController.CreateNewClassByExcel(file);
            
            var obResult = rs.Result as ObjectResult;
            
            // Assert
            Assert.AreEqual(expectedStatus, obResult.StatusCode);
        }

    }
}