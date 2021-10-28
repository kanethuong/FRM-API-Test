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
using kroniiapi.Helper.UploadDownloadFile;
using kroniiapi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace kroniiapitest.Intergration.TraineeControllerTest
{
    [TestFixture]
    public class EditProfileTest
    {
        private static IFormFile testFormFile
        {
            get
            {
                string workingDirectory = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
                string pathToTest = projectDirectory + "\\FileForTest\\Avatar.png";

                var stream = File.OpenRead(pathToTest);
                var formFile = new FormFile(stream, 0, stream.Length, "Avatar", "Avatar.png");
                var contentDisposition = new ContentDispositionHeaderValue("attachment");
                contentDisposition.FileName = "Avatar.png";
                contentDisposition.FileNameStar = "Avatar.png";
                formFile.Headers = new HeaderDictionary();
                formFile.ContentType = "application/png";
                formFile.ContentDisposition = contentDisposition.ToString();
                return formFile;
            }
        }
        private readonly Mock<IImgHelper> mockImgHelper = new Mock<IImgHelper>();
        private IMapper mapper;
        private ITraineeService service;
        private TraineeController controller;
        private DataContext dataContext;
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
        private static string OutputURL = "https://google.com";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            mockImgHelper.Setup(img => img.Upload(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(OutputURL);
            var option = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase("data").Options;
            dataContext = new DataContext(option);
            dataContext.Roles.AddRange(roleList);
            dataContext.SaveChanges();

            var config = new MapperConfiguration(config =>
            {
                config.AddProfile(new TraineeProfile());
            });
            mapper = config.CreateMapper();
            service = new TraineeService(dataContext);
            controller = new TraineeController(mapper, null, null, service, null, null, null, null, null, null, null, null, mockImgHelper.Object);
        }

        private Trainee testTrainee
        {
            get
            {
                return new Trainee()
                {
                    TraineeId = 1,
                    Username = "huynhqtien",
                    Password = "test",
                    Fullname = "Huynh Tien",
                    Email = "huynhqtien@gmail.com",
                    Phone = "0999999999",
                    DOB = DateTime.Now,
                    Address = "Can Tho",
                    Gender = "Male",
                    TuitionFee = 0,
                    Wage = 0,
                    RoleId = 1
                };
            }
        }

        [SetUp]
        public void SetUp()
        {
            dataContext.Trainees.Add(testTrainee);
            dataContext.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            dataContext.Trainees.RemoveRange(dataContext.Trainees);
            dataContext.SaveChanges();
        }

        private static IEnumerable<TestCaseData> EditProfileTestSource
        {
            get
            {
                yield return new TestCaseData(1, new TraineeProfileDetailInput(), 409);
                yield return new TestCaseData(1, new TraineeProfileDetailInput()
                {
                    Fullname = "Huynh Quang Tien",
                    Phone = "0111111111",
                    DOB = DateTime.MinValue,
                    Address = "An Giang",
                    Gender = "Female"
                }, 200);
                yield return new TestCaseData(1201, new TraineeProfileDetailInput(), 404);
            }
        }

        [Test]
        [TestCaseSource(nameof(EditProfileTestSource))]
        public async Task EditProfile_Result(int id, TraineeProfileDetailInput input, int statusCode)
        {
            var actionResult = await controller.EditProfile(id, input) as ObjectResult;
            var response = actionResult.Value as ResponseDTO;
            Assert.True(
                actionResult.StatusCode == statusCode && response.Status == statusCode
            );
        }

        private static IEnumerable<TestCaseData> UpdateAvatarTestSource
        {
            get
            {
                yield return new TestCaseData(1, testFormFile, 200);
                yield return new TestCaseData(2, testFormFile, 404);
                yield return new TestCaseData(1, null, 409);
            }
        }

        [Test]
        [TestCaseSource(nameof(UpdateAvatarTestSource))]
        public async Task UpdateAvatar_Result(int id, IFormFile file, int statusCode)
        {
            var actionResult = await controller.UpdateAvatar(id, file) as ObjectResult;
            var response = actionResult.Value as ResponseDTO;
            Assert.True(
                actionResult.StatusCode == statusCode && response.Status == statusCode
            );
        }
    }
}