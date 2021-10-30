using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.ApplicationDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Helper.Upload;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace kroniiapiTest.Unit.ApplicationControllerTest
{
    public class ViewApplicationListTest
    {
        private readonly Mock<IApplicationService> mockAppService = new Mock<IApplicationService>();
        private readonly Mock<ITraineeService> mockTraineeService = new Mock<ITraineeService>();
        private readonly Mock<IMapper> mockMapper = new Mock<IMapper>();
        private readonly Mock<IMegaHelper> mockMegaHelper = new Mock<IMegaHelper>();
        IEnumerable<TraineeApplicationResponse> listApp = new List<TraineeApplicationResponse>
        {
            new TraineeApplicationResponse
            {
                Description = "",
                ApplicationURL = "Phuttt",
                Type = "Tran Thien Phu",
                IsAccepted = null
            },

            new TraineeApplicationResponse
            {
                Description = "",
                ApplicationURL = "Phuttt",
                Type = "Tran Thien Phu",
                IsAccepted = null
            }
        };
        public static IEnumerable<TestCaseData> ViewApplicationListTestCaseTrue
        {
            get
            {
                // True case: with PageNumber, PageSize and SearchName
                yield return new TestCaseData(
                    new PaginationParameter
                    {
                        PageNumber = 1,
                        PageSize = 1,
                    },
                    200
                );
                // True case: with SearchName
                yield return new TestCaseData(
                    new PaginationParameter
                    {
                        SearchName = "hostcode0301"
                    },
                    200
                );
            }
        }
        [TestCaseSource("ViewApplicationListTestCaseTrue")]
        [Test]
        public async Task ViewApplicationList_ReturnActionResult_Return200(PaginationParameter paginationParameter, int stacode)
        {
            //Calling Controller using 2 mock Object
            var appController
                = new ApplicationController(mockMapper.Object, mockTraineeService.Object, mockAppService.Object, mockMegaHelper.Object);

            // Setup Services return using Mock
            mockTraineeService.Setup(x => x.CheckTraineeExist(1)).Returns(true);
            mockTraineeService.Setup(x => x.GetApplicationListByTraineeId(1, paginationParameter)).ReturnsAsync(Tuple.Create(2, listApp));
            // Get Controller return result
            var actual = await appController.ViewApplicationList(1, paginationParameter);
            var okResult = actual.Result as ObjectResult;

            // Assert result with expected result: this time is 404
            Assert.AreEqual(stacode, okResult.StatusCode);
        }

        public static IEnumerable<TestCaseData> ViewApplicationListTestCaseFail
        {
            get
            {
                // True case: with PageNumber, PageSize and SearchName
                yield return new TestCaseData(
                    new ResponseDTO(404, "id not found"),
                    404
                );

            }
        }
        [TestCaseSource("ViewApplicationListTestCaseFail")]
        [Test]
        public async Task ViewApplicationList_ReturnActionResult_Return404_TraineeNotFound(ResponseDTO rp, int stacode)
        {
            PaginationParameter p = new();
            //Calling Controller using 2 mock Object
            var appController
                = new ApplicationController(mockMapper.Object, mockTraineeService.Object, mockAppService.Object, mockMegaHelper.Object);

            // Setup Services return using Mock
            mockTraineeService.Setup(x => x.CheckTraineeExist(1)).Returns(false);
            // Get Controller return result
            var actual = await appController.ViewApplicationList(1, p);
            var okResult = actual.Result as ObjectResult;
            var actResponse = okResult.Value as ResponseDTO;
            var expJson = JsonConvert.SerializeObject(rp);
            var actJson = JsonConvert.SerializeObject(actResponse);
            // Assert result with expected result: this time is 404
            Assert.True(
                stacode == okResult.StatusCode,
                expJson = actJson
            );
        }
        public static IEnumerable<TestCaseData> ViewApplicationListTestCaseNoForm
        {
            get
            {
                yield return new TestCaseData(
                    new ResponseDTO(404, "Trainee doesn't have any application"),
                    404
                );
            }
        }
        [TestCaseSource("ViewApplicationListTestCaseNoForm")]
        [Test]
        public async Task ViewApplicationList_ReturnActionResult_Return404_NoForm(ResponseDTO rp, int stacode)
        {
            PaginationParameter p = new();
            //Calling Controller using 2 mock Object
            var appController
                = new ApplicationController(mockMapper.Object, mockTraineeService.Object, mockAppService.Object, mockMegaHelper.Object);

            // Setup Services return using Mock
            mockTraineeService.Setup(x => x.CheckTraineeExist(1)).Returns(true);
            mockTraineeService.Setup(x => x.GetApplicationListByTraineeId(1, p)).ReturnsAsync(Tuple.Create(0, listApp));
            // Get Controller return result
            var actual = await appController.ViewApplicationList(1, p);
            var okResult = actual.Result as ObjectResult;
            var actResponse = okResult.Value as ResponseDTO;
            var expJson = JsonConvert.SerializeObject(rp);
            var actJson = JsonConvert.SerializeObject(actResponse);
            // Assert result with expected result: this time is 404
            Assert.True(
                stacode == okResult.StatusCode,
                expJson = actJson
            );
        }
    }
}