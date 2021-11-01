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
    public class ViewClassDetailTest
    {
        private IMapper mapper;
        private Mock<IClassService> mockClassService = new();
        private ClassController controller;

        private static IEnumerable<TestCaseData> GetClassDetailTestData
        {
            get
            {
                yield return new TestCaseData(new Class(), 200);
                yield return new TestCaseData(null, 404);
            }
        }

        [OneTimeSetUp]
        public void SetUp() {
            var config = new MapperConfiguration(config => config.AddProfile(new ClassProfile()));
            mapper = config.CreateMapper();
            controller = new ClassController(mockClassService.Object, null, null, null, null, null, null, mapper);
        }

        [Test]
        [TestCaseSource(nameof(GetClassDetailTestData))]
        public async Task GetClassDetail_Result(Class cl, int statusCode)
        {
            mockClassService.Setup(cs => cs.GetClassDetail(It.IsAny<int>())).ReturnsAsync(cl);
            var actionResult = await controller.ViewClassDetail(0);
            var value = actionResult.Result as ObjectResult;
            Assert.AreEqual(statusCode, value.StatusCode);
        }
    }
}