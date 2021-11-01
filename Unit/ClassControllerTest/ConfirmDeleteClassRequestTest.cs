using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DTO;
using kroniiapi.DTO.ClassDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace kroniiapiTest.Unit.ClassControllerTest
{
    [TestFixture]
    public class ConfirmDeleteClassRequestTest
    {
        ClassController classController;
        Mock<IClassService> mockClass = new Mock<IClassService>();
        Mock<IAdminService> mockAdmin = new Mock<IAdminService>();
        Mock<ITrainerService> mockTrainer = new Mock<ITrainerService>();
        Mock<ITraineeService> mockTrainee = new Mock<ITraineeService>();
        Mock<IMarkService> mockMark = new Mock<IMarkService>();
        Mock<IModuleService> mockModule = new Mock<IModuleService>();
        Mock<IFeedbackService> mockFeedback = new Mock<IFeedbackService>();
        Mock<IMapper> mockMapper = new Mock<IMapper>();

        ConfirmDeleteClassInput cfDelClassInput = new ConfirmDeleteClassInput();
        [Test]
        public async Task ConfirmDelClassReq_Test_Success()
        {
            classController = new ClassController(mockClass.Object, mockTrainee.Object, mockAdmin.Object, mockModule.Object, mockTrainer.Object, mockMapper.Object,null);
            mockClass.Setup(cl => cl.UpdateDeletedClass(cfDelClassInput)).ReturnsAsync(1);
            var result = await classController.ConfirmDeleteClassRequest(cfDelClassInput) as ObjectResult;
            var rs = result.Value as ResponseDTO;
            var expectedStatus = 200;
            Assert.True(
                result.StatusCode == expectedStatus && rs.Status == expectedStatus, "Wrong status code"
            );
        }

        [Test]
        public async Task ConfirmDelClassReq_Test_NotFound()
        {
            classController = new ClassController(mockClass.Object, mockTrainee.Object, mockAdmin.Object, mockModule.Object, mockTrainer.Object, mockMapper.Object,null);
            mockClass.Setup(cl => cl.UpdateDeletedClass(cfDelClassInput)).ReturnsAsync(-1);
            var result = await classController.ConfirmDeleteClassRequest(cfDelClassInput) as ObjectResult;
            var rs = result.Value as ResponseDTO;
            var expectedStatus = 404;
            Assert.True(
                result.StatusCode == expectedStatus && rs.Status == expectedStatus, "Wrong status code"
            );
        }

        [Test]
        public async Task ConfirmDelClassReq_Test_Conflict()
        {
            classController = new ClassController(mockClass.Object, mockTrainee.Object,mockAdmin.Object, mockModule.Object, mockTrainer.Object, mockMapper.Object,null);
            mockClass.Setup(cl => cl.UpdateDeletedClass(cfDelClassInput)).ReturnsAsync(0);
            var result = await classController.ConfirmDeleteClassRequest(cfDelClassInput) as ObjectResult;
            var rs = result.Value as ResponseDTO;
            var expectedStatus = 409;
            Assert.True(
                result.StatusCode == expectedStatus && rs.Status == expectedStatus, "Wrong status code"
            );
        }

    }
}