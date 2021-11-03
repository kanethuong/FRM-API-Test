using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DB.Models;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.DTO.Profiles;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace kroniiapitest.Unit.ClassControllerTest
{
    [TestFixture]
    public class GetTraineeListByClassIdTest
    {
        private IMapper mapper;
        private Mock<IClassService> mockClassService = new();
        private ClassController controller;

        private static IEnumerable<TestCaseData> GetTraineeListByClassIdTestData
        {
            get
            {
                yield return new TestCaseData(0, 404);
                yield return new TestCaseData(1, 200);
                yield return new TestCaseData(2, 200);
            }
        }

        [OneTimeSetUp]
        public void SetUp() {
            var config = new MapperConfiguration(config => config.AddProfile(new TraineeProfile()));
            mapper = config.CreateMapper();
            controller = new ClassController(mockClassService.Object, null, null, null, null, mapper,null);
        }

        [Test]
        [TestCaseSource(nameof(GetTraineeListByClassIdTestData))]
        public async Task GetTraineeListByClassId_Result(int result, int statusCode)
        {
            mockClassService.Setup(cs => cs.GetTraineesByClassId(It.IsAny<int>(), It.IsAny<PaginationParameter>())).ReturnsAsync(new Tuple<int, IEnumerable<Trainee>>(result, new List<Trainee>()));
            var actionResult = await controller.GetTraineeListByClassId(0, new());
            var value = actionResult.Result as ObjectResult;
            Assert.AreEqual(statusCode, value.StatusCode);
        }
    }
}