using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.ApplicationDTO;
using kroniiapi.DTO.Email;
using kroniiapi.DTO.Profiles;
using kroniiapi.Helper.Upload;
using kroniiapi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace kroniiapiTest.Intergration.ApplicationControllerTest
{
    [TestFixture]
    public class SubmitApplicationFormTest
    {
        private DataContext dataContext;
        private ITraineeService traineeService;
        private IMapper mapper;
        private ApplicationService applicationService;
        private ApplicationController applicationController;
        private IMegaHelper megaHelper;
        private Mock<IMegaHelper> mockMegaHelper = new Mock<IMegaHelper>();
        private readonly List<ApplicationCategory> applicationCategories = new List<ApplicationCategory>() {
            new ApplicationCategory() {
                ApplicationCategoryId = 1,
                CategoryName = "Đơn đề nghị thôi học"
            },
            new ApplicationCategory() {
                ApplicationCategoryId = 2,
                CategoryName = "Đơn chuyển cơ sở"
            },
            new ApplicationCategory() {
                ApplicationCategoryId = 3,
                CategoryName = "Đơn chuyển ngành học"
            },
            new ApplicationCategory() {
                ApplicationCategoryId = 4,
                CategoryName = "Đơn bảo lưu học phần"
            },
            new ApplicationCategory() {
                ApplicationCategoryId = 5,
                CategoryName = "Đơn đăng ký thi cải thiện điểm"
            },
            new ApplicationCategory() {
                ApplicationCategoryId = 6,
                CategoryName = "Đơn xác nhận sinh viên"
            },

        };


        List<Trainee> trainee = new List<Trainee>
        {
            new Trainee{
                TraineeId = 1,
                Username = "CongVinh",
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
                IsDeactivated = false,
                RoleId = 4,
                ClassId = 1,
            },
            new Trainee{
                TraineeId = 2,
                Username = "ThuyTien",
                Password = "123456",
                Fullname = "CaiGiDoThuyTien",
                AvatarURL = "none",
                Email = "thuytiencungkosaoke@gmail.vn",
                Phone = "0992377",
                DOB = new DateTime(1970, 2, 1),
                Address = "Nam Long, Quan 7, HCM",
                Gender = "Female",
                Wage = 10000,
                CreatedAt = new DateTime(2021, 1, 1),
                IsDeactivated = true,
                RoleId = 4,
                ClassId = 1,
            }

        };


        [OneTimeSetUp]
        public void setupFirst()
        {
            var option = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase("data").Options;
            dataContext = new DataContext(option);
            dataContext.ApplicationCategories.AddRange(applicationCategories);
            dataContext.Trainees.AddRange(trainee);
            dataContext.SaveChanges();

            var config = new MapperConfiguration(config =>
            {
                config.AddProfile(new ApplicationProfile());
            });
            mapper = config.CreateMapper();


            applicationService = new ApplicationService(
                dataContext,
                mockMegaHelper.Object
            );
            applicationController = new ApplicationController(mapper, traineeService, applicationService, mockMegaHelper.Object);

        }

        [OneTimeTearDown]
        public void tearDown()
        {
            dataContext.Applications.RemoveRange(dataContext.Applications);
            dataContext.ApplicationCategories.RemoveRange(dataContext.ApplicationCategories);
            dataContext.Trainees.RemoveRange(trainee);
            dataContext.SaveChanges();
        }

        private static IEnumerable<TestCaseData> SubmitApplicationTrue
        {
            get
            {
                yield return new TestCaseData(
                    new ApplicationInput
                    {
                        Description = "Xac nhan sv",
                        TraineeId = 1,
                        ApplicationCategoryId = 6
                    },
                    "\\FileForTest\\ApplicationTest.docx",
                    201
                    );                
            }
        }


        [Test]
        [TestCaseSource("SubmitApplicationTrue")]
        public async Task SubmitApplicationTrue_201(ApplicationInput applicationInput,string pathTest, int statusCode)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            string pathToTest = projectDirectory + pathTest;
            var stream = File.OpenRead(pathToTest);
            IFormFile file = new FormFile(stream, 0, stream.Length, "ApplicationTest", "ApplicationTest.docx");
            mockMegaHelper.Setup(mega => mega.Upload(stream, "ApplicationTest.docx", "ApplicationForm")).ReturnsAsync("url.com");
            var actual = await applicationController.SubmitApplicationForm(applicationInput, file);
            var okResult = actual as ObjectResult;
            Assert.AreEqual(statusCode, okResult.StatusCode);
        }


        private static IEnumerable<TestCaseData> SubmitApplicationFail
        {
            get
            {
                //Fail case: Trainee deactivated
                yield return new TestCaseData(
                    new ApplicationInput
                    {
                        Description = "Bao luu",
                        TraineeId = 2,
                        ApplicationCategoryId = 64
                    },
                    "\\FileForTest\\ApplicationTest.docx",
                    400
                    );        
                
                //Fail case: Not .doc or .docx file
                yield return new TestCaseData(
                    new ApplicationInput
                    {
                        Description = "Xac nhan sv",
                        TraineeId = 2,
                        ApplicationCategoryId = 6
                    },
                    "\\FileForTest\\Avatar.png",
                    400
                    );      

            }
        }


        [Test]
        [TestCaseSource("SubmitApplicationFail")]
        public async Task SubmitApplicationFail_400(ApplicationInput applicationInput,string pathTest, int statusCode)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            string pathToTest = projectDirectory + pathTest;
            var stream = File.OpenRead(pathToTest);
            IFormFile file = new FormFile(stream, 0, stream.Length, "ApplicationTest", "ApplicationTest.docx");
            mockMegaHelper.Setup(mega => mega.Upload(stream, "ApplicationTest.docx", "ApplicationForm")).ReturnsAsync("url.com");
            var actual = await applicationController.SubmitApplicationForm(applicationInput, file);
            var okResult = actual as ObjectResult;
            Assert.AreEqual(statusCode, okResult.StatusCode);
        }


        private static IEnumerable<TestCaseData> SubmitApplicationFailNoFileSelected
        {
            get
            {
                
                //Fail case: No file selected
                yield return new TestCaseData(
                    new ApplicationInput
                    {
                        Description = "Xac nhan sv",
                        TraineeId = 2,
                        ApplicationCategoryId = 6
                    },
                    400
                    );      

            }
        }


        [Test]
        [TestCaseSource("SubmitApplicationFailNoFileSelected")]
        public async Task SubmitApplicationFailNoFileSelected_400(ApplicationInput applicationInput, int statusCode)
        {
            // string workingDirectory = Environment.CurrentDirectory;
            // string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            // string pathToTest = projectDirectory + pathTest;
            // var stream = File.OpenRead(pathToTest);
            // IFormFile file = new FormFile(stream, 0, stream.Length, "ApplicationTest", "ApplicationTest.docx");
            //mockMegaHelper.Setup(mega => mega.Upload(stream, "ApplicationTest.docx", "ApplicationForm")).ReturnsAsync("url.com");
            var actual = await applicationController.SubmitApplicationForm(applicationInput, null);
            var okResult = actual as ObjectResult;
            Assert.AreEqual(statusCode, okResult.StatusCode);
        }


    }
}