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

namespace kroniiapitest.Unit.FeedbackControllerTest
{
    public class SendAdminFeedbackTest
    {
         private readonly Mock<ITraineeService> mockTraineeService = new Mock<ITraineeService>();
        private readonly Mock<IFeedbackService> mockFeedbackService = new Mock<IFeedbackService>();
        private readonly Mock<IMapper> mockMapper = new Mock<IMapper>();
        public static IEnumerable<TestCaseData> SendAdminFeedbackTestCaseTrue
        {
            get
            {
                yield return new TestCaseData(
                    1,
                    200
                );
                yield return new TestCaseData(
                    -1,
                    404
                );
                yield return new TestCaseData(
                    0,
                    404
                );
            }
        }
        [TestCaseSource("SendAdminFeedbackTestCaseTrue")]
        [Test]
        public async Task SendAdminFeedback_ReturnActionResult(int rs, int stacode)
        {
            //Calling Controller using 2 mock Object
            var FbController
                = new FeedbackController(null, mockFeedbackService.Object, mockMapper.Object, null, null,mockTraineeService.Object);
            var input = new AdminFeedbackInput()
            {
                AdminId=1,
                TraineeId=1,
                Content="good",
                Rate=4
            };
            // Setup Services return using Mock
            mockTraineeService.Setup(x=>x.GetClassIdByTraineeId(1)).ReturnsAsync((1,""));
            mockFeedbackService.Setup(x => x.InsertNewAdminFeedback(null)).ReturnsAsync((rs, "abc"));
            // Get Controller return result
            var actual = await FbController.SendAdminFeedback(input);
            var okResult = actual as ObjectResult;

            Assert.AreEqual(stacode, okResult.StatusCode);
        }
    }
}