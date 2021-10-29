using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DTO.FeedbackDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace kroniiapiTest.Unit.FeedbackControllerTest
{
    public class ViewFeedbackInfoTest
    {
        private readonly Mock<ITrainerService> mockTrainerService = new Mock<ITrainerService>();
        private readonly Mock<IClassService> mockClassService = new Mock<IClassService>();
        private readonly Mock<ITraineeService> mockTraineeService = new Mock<ITraineeService>();

        private readonly Mock<IFeedbackService> mockFeedbackService = new Mock<IFeedbackService>();
        private readonly Mock<IAdminService> mockAdminService = new Mock<IAdminService>();
        private readonly Mock<IMapper> mockMapper = new Mock<IMapper>();
        FeedbackViewForTrainee view = new FeedbackViewForTrainee()
        {
            trainer = new TrainerInFeedbackResponse()
            {
                TrainerId = 1,
                AvatarURL = "",
                Email = "",
                Fullname = ""
            },
            admin = new AdminInFeedbackResponse()
            {
                AdminId = 1,
                AvatarURL = "",
                Email = "",
                Fullname = ""
            }
        };
        public static IEnumerable<TestCaseData> ViewFeedbackInfoTestCaseTrue
        {
            get
            {
                yield return new TestCaseData(
                    1,
                    200
                );
            }
        }
        [TestCaseSource("ViewFeedbackInfoTestCaseTrue")]
        [Test]
        public async Task ViewFeedbackInfo_ReturnActionResult_Return200(int traineeId, int stacode)
        {
            //Calling Controller using 2 mock Object
            var FbController
                = new FeedbackController(mockClassService.Object, mockFeedbackService.Object, mockMapper.Object, mockAdminService.Object, mockTrainerService.Object,mockTraineeService.Object);

            // Setup Services return using Mock
            mockClassService.Setup(x => x.GetFeedbackViewForTrainee(traineeId)).ReturnsAsync(view);
            // Get Controller return result
            var actual = await FbController.ViewFeedbackInfo(1);
            var okResult = actual.Result as ObjectResult;

            // Assert result with expected result: this time is 404
            Assert.AreEqual(stacode, okResult.StatusCode);
        }

        public static IEnumerable<TestCaseData> ViewFeedbackInfoTestCase404
        {
            get
            {
                yield return new TestCaseData(
                    1,
                    404
                );
            }
        }

        [TestCaseSource("ViewFeedbackInfoTestCase404")]
        [Test]
        public async Task ViewFeedbackInfo_ReturnActionResult_Return404(int traineeId, int stacode)
        {
            //Calling Controller using 2 mock Object
            var FbController
                = new FeedbackController(mockClassService.Object, mockFeedbackService.Object, mockMapper.Object, mockAdminService.Object, mockTrainerService.Object,mockTraineeService.Object);

            // Setup Services return using Mock
            mockClassService.Setup(x => x.GetFeedbackViewForTrainee(traineeId)).ReturnsAsync(value: null);
            // Get Controller return result
            var actual = await FbController.ViewFeedbackInfo(1);
            var okResult = actual.Result as ObjectResult;

            // Assert result with expected result: this time is 404
            Assert.AreEqual(stacode, okResult.StatusCode);
        }
    }
}