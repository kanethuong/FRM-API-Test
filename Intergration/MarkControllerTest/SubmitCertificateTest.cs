using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.Profiles;
using kroniiapi.DTO.TraineeDTO;
using kroniiapi.Helper.Upload;
using kroniiapi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace kroniiapitest.Intergration.MarkControllerTest
{
    public class SubmitCertificateTest
    {
        private DataContext dataContext;
        private IMapper mapper;
        private ICertificateService certificateService;
        private IModuleService moduleService;
        private ITraineeService traineeService;
        private MarkController markController;
        private readonly Mock<IMegaHelper> mockMegaHelper = new Mock<IMegaHelper>();

        private Certificate certificateList = new Certificate
        {
            ModuleId = 1,
            TraineeId = 1
        };

        private List<Module> moduleList = new List<Module>()
        {
            new Module{
                ModuleId = 1
            },
            new Module{
                ModuleId=2
            }
        };

        private List<Trainee> traineeList = new List<Trainee>()
        {
            new Trainee{
                TraineeId = 1,
                IsDeactivated=false
            },
            new Trainee{
                TraineeId=2,
                IsDeactivated=true
            },
            new Trainee{
                TraineeId = 3,
                IsDeactivated=false
            }
        };

        private static string outputURL = "https://google.com";
        [OneTimeSetUp]
        public void setupFirst()
        {
            var option = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase("data").Options;
            dataContext = new DataContext(option);
            dataContext.Certificates.AddRange(certificateList);
            dataContext.Modules.AddRange(moduleList);
            dataContext.Trainees.AddRange(traineeList);
            dataContext.SaveChanges();

            var config = new MapperConfiguration(config =>
            {
                config.AddProfile(new TraineeProfile());
            });
            mapper = config.CreateMapper();

            mockMegaHelper.Setup(m => m.Upload(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(outputURL);

            certificateService = new CertificateService(dataContext);
            moduleService = new ModuleService(dataContext);
            traineeService = new TraineeService(dataContext);
            markController = new MarkController(traineeService,
                                              certificateService,
                                              mockMegaHelper.Object,
                                              mapper,
                                              moduleService,
                                              null,
                                              null);
        }

        [OneTimeTearDown]
        public void tearDown()
        {
            dataContext.Certificates.RemoveRange(dataContext.Certificates);
            dataContext.Modules.RemoveRange(dataContext.Modules);
            dataContext.Trainees.RemoveRange(dataContext.Trainees);
            dataContext.SaveChanges();
        }

        public static IEnumerable<TestCaseData> SubmitCertificateTestCases
        {
            get
            {
                //True case
                yield return new TestCaseData(
                    "\\FileForTest\\SubmitCertificateTestTrue.png",
                    new CertificateInput
                    {
                        ModuleId = 2,
                        TraineeId = 3
                    },
                    201
                );

                //Fail case: module not exist
                yield return new TestCaseData(
                    "\\FileForTest\\SubmitCertificateTestTrue.png",
                    new CertificateInput
                    {
                        ModuleId = 3,
                        TraineeId = 1
                    },
                    404
                );

                //Fail case: trainee not exist
                yield return new TestCaseData(
                    "\\FileForTest\\SubmitCertificateTestTrue.png",
                    new CertificateInput
                    {
                        ModuleId = 1,
                        TraineeId = 4
                    },
                    404
                );

                //Fail case: trainee is deactivated
                yield return new TestCaseData(
                    "\\FileForTest\\SubmitCertificateTestTrue.png",
                    new CertificateInput
                    {
                        ModuleId = 1,
                        TraineeId = 2
                    },
                    404
                );

                //Fail case: certificate duplicate
                yield return new TestCaseData(
                    "\\FileForTest\\SubmitCertificateTestTrue.png",
                    new CertificateInput
                    {
                        ModuleId = 1,
                        TraineeId = 1
                    },
                    409
                );

                //Fail case: file certificate wrong extension
                yield return new TestCaseData(
                    "\\FileForTest\\SubmitCertificateTestWrongExtension.docx",
                    new CertificateInput
                    {
                        ModuleId = 2,
                        TraineeId = 3
                    },
                    400
                );
            }
        }

        [Test]
        [TestCaseSource("SubmitCertificateTestCases")]
        public async Task SubmitCertificate_Test(string pathTest, CertificateInput certificateInput, int expStatus)
        {
            //Arrange
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            string pathToTest = projectDirectory + pathTest;
            var stream = File.OpenRead(pathToTest);
            string[] fileName = pathTest.Split('\\');
            IFormFile file = new FormFile(stream, 0, stream.Length, "SubmitCertificateTest", fileName[2]);

            //Act
            var result = await markController.SubmitCertificate(file, certificateInput) as ObjectResult;
            var response = result.Value as ResponseDTO;

            //Assert
            Assert.True(expStatus == result.StatusCode && expStatus == response.Status);
        }
    }
}