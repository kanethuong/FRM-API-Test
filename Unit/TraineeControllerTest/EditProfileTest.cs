using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.Profiles;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace kroniiapitest.Unit.TraineeControllerTest
{
    [TestFixture]
    public class EditProfileTest
    {
        private IMapper mapper;
        private readonly Mock<ITraineeService> mockTraineeService = new();
        private TraineeController controller;

        [OneTimeSetUp]
        public void SetUp()
        {
            var config = new MapperConfiguration(config =>
            {
                config.AddProfile(new TraineeProfile());
            });
            mapper = config.CreateMapper();
            controller = new TraineeController(mapper, null, null, mockTraineeService.Object, null, null, null, null, null, null, null, null, null);
        }

        private static IEnumerable<TestCaseData> EditProfileData
        {
            get
            {
                yield return new TestCaseData(1, 200);
                yield return new TestCaseData(0, 409);
                yield return new TestCaseData(-1, 404);
            }
        }

        [Test]
        [TestCaseSource(nameof(EditProfileData))]
        public async Task EditProfile_Result(int result, int statusCode)
        {
            mockTraineeService.Setup(tr => tr.UpdateTrainee(It.IsAny<int>(), It.IsAny<Trainee>())).ReturnsAsync(result);
            var actionResult = await controller.EditProfile(0, new()) as ObjectResult;
            var response = actionResult.Value as ResponseDTO;
            Assert.True(
                actionResult.StatusCode == statusCode && response.Status == statusCode
            );
        }
    }
}