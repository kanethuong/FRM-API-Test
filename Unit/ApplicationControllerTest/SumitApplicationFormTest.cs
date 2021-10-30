using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.ApplicationDTO;
using kroniiapi.DTO.Profiles;
using kroniiapi.Helper.Upload;
using kroniiapi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace kroniiapitest.Unit.TraineeControllerTest
{
    [TestFixture]
    public class SubmitApplicationFormTest
    {
        private IMapper mapper;
        private readonly Mock<IMapper> mockMapper = new Mock<IMapper>();
        private readonly Mock<IApplicationService> mockApplicationService = new Mock<IApplicationService>();
        private readonly Mock<ITraineeService> mockTraineeService = new Mock<ITraineeService>();
        private readonly Mock<IMegaHelper> mockMegaHelper = new Mock<IMegaHelper>();
        
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
            var config = new MapperConfiguration(config =>
            {
                config.AddProfile(new ApplicationProfile());
            });
            mapper = config.CreateMapper();
            ApplicationController applicationController = new ApplicationController(mockMapper.Object, mockTraineeService.Object, mockApplicationService.Object, mockMegaHelper.Object);
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            string pathToTest = projectDirectory + pathTest;
            var stream = File.OpenRead(pathToTest);
            IFormFile file = new FormFile(stream, 0, stream.Length, "ApplicationTest", "ApplicationTest.docx");
            Application application = mapper.Map<Application>(applicationInput);
            mockMegaHelper.Setup(mega => mega.Upload(stream, "ApplicationTest.docx","ApplicationForm")).ReturnsAsync("url.com");
            mockApplicationService.Setup(app => app.InsertNewApplication(application, file)).ReturnsAsync(1);            
            var actual = await applicationController.SubmitApplicationForm(applicationInput, file);
            var okResult = actual as ObjectResult;
            Assert.AreEqual(statusCode, okResult.StatusCode);
        }

        private static IEnumerable<TestCaseData> SubmitApplicationNotDocFile
        {
            get
            {
                //Fail case: File is not .doc or .docx
                yield return new TestCaseData(
                    new ApplicationInput
                    {
                        Description = "Xac nhan sv",
                        TraineeId = 1,
                        ApplicationCategoryId = 6
                    },
                    "\\FileForTest\\Avatar.png",
                    400
                    );                
            }
        }


        [Test]
        [TestCaseSource("SubmitApplicationNotDocFile")]
        public async Task SubmitApplicationNotDocFile_400(ApplicationInput applicationInput,string pathTest, int statusCode)
        {
            var config = new MapperConfiguration(config =>
            {
                config.AddProfile(new ApplicationProfile());
            });
            mapper = config.CreateMapper();
            ApplicationController applicationController = new ApplicationController(mockMapper.Object, mockTraineeService.Object, mockApplicationService.Object, mockMegaHelper.Object);
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            string pathToTest = projectDirectory + pathTest;
            var stream = File.OpenRead(pathToTest);
            IFormFile file = new FormFile(stream, 0, stream.Length, "ApplicationTest", "ApplicationTest.docx");
            Application application = mapper.Map<Application>(applicationInput);
            mockMegaHelper.Setup(mega => mega.Upload(stream, "ApplicationTest.docx","ApplicationForm")).ReturnsAsync("url.com");
            mockApplicationService.Setup(app => app.InsertNewApplication(application, file)).ReturnsAsync(1);            
            var actual = await applicationController.SubmitApplicationForm(applicationInput, file);
            var okResult = actual as ObjectResult;
            Assert.AreEqual(statusCode, okResult.StatusCode);
        }

        private static IEnumerable<TestCaseData> SubmitApplicationNoFileSelected
        {
            get
            {
                //Fail case: File is not .doc or .docx
                yield return new TestCaseData(
                    new ApplicationInput
                    {
                        Description = "Xac nhan sv",
                        TraineeId = 1,
                        ApplicationCategoryId = 6
                    },
                    400
                    );                
            }
        }


        [Test]
        [TestCaseSource("SubmitApplicationNoFileSelected")]
        public async Task SubmitApplicationNoFileSelected_400(ApplicationInput applicationInput,int statusCode)
        {
            var config = new MapperConfiguration(config =>
            {
                config.AddProfile(new ApplicationProfile());
            });
            mapper = config.CreateMapper();
            ApplicationController applicationController = new ApplicationController(mockMapper.Object, mockTraineeService.Object, mockApplicationService.Object, mockMegaHelper.Object);
            // string workingDirectory = Environment.CurrentDirectory;
            // string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            // string pathToTest = projectDirectory + pathTest;
            // var stream = File.OpenRead(pathToTest);
            // IFormFile file = new FormFile(stream, 0, stream.Length, "ApplicationTest", "ApplicationTest.docx");
            Application application = mapper.Map<Application>(applicationInput);
            // mockMegaHelper.Setup(mega => mega.Upload(stream, "ApplicationTest.docx","ApplicationForm")).ReturnsAsync("url.com");
            mockApplicationService.Setup(app => app.InsertNewApplication(application, null)).ReturnsAsync(0);            
            var actual = await applicationController.SubmitApplicationForm(applicationInput, null);
            var okResult = actual as ObjectResult;
            Assert.AreEqual(statusCode, okResult.StatusCode);
        }


    }
}
