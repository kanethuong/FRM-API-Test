using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using kroniiapi.Controllers;
using kroniiapi.DB.Models;

namespace kroniiapitest.Unit.ClassControllerTest
{
    public class ViewClassDetailByTraineeId
    {
        private readonly Mock<IClassService> mockClass = new Mock<IClassService>();
        private readonly Mock<ITraineeService> mockTrainee= new Mock<ITraineeService>();
        private readonly Mock<IAdminService> mockAdmin= new Mock<IAdminService>();
        private readonly Mock<IFeedbackService> mockFeedback= new Mock<IFeedbackService>();
        private readonly Mock<ITrainerService> mockTrainer= new Mock<ITrainerService>();
        private readonly Mock<IMarkService> mockMark= new Mock<IMarkService>();
        private readonly Mock<IModuleService> mockModule= new Mock<IModuleService>();
        private readonly Mock<IMapper> mockMapper = new Mock<IMapper>();

        public static IEnumerable<TestCaseData> ViewClassDetailByTraineeIdTestCaseTrue
        {
            get
            {
                yield return new TestCaseData(
                    1,
                    200
                );
            }
        }

        Class classDetail = new Class
        {
            ClassId = 1,
            ClassName = "SE1501",
            Description = "Duoc vl",
            CreatedAt = new DateTime(2021, 1, 1),
            StartDay = new DateTime(2021, 1, 1),
            EndDay = new DateTime(2021, 6, 6),
            IsDeactivated = false,
        };
        [Test]
        [TestCaseSource("ViewClassDetailByTraineeIdTestCaseTrue")]
        public async Task ViewClassDetailByTraineeIdTrue_ActionResult_Status200(int traineeId, int result)
        {
            ClassController controller = new ClassController(mockClass.Object,mockTrainee.Object ,mockAdmin.Object, mockModule.Object, mockTrainer.Object, mockMapper.Object,null);

            mockTrainee.Setup(x => x.GetClassIdByTraineeId(traineeId)).ReturnsAsync((1, ""));

            mockClass.Setup(c => c.GetClassDetail(1)).ReturnsAsync(classDetail);

            var actual = await controller.ViewClassDetailByTraineeId(traineeId);
            var obResult = actual.Result as ObjectResult;

            Assert.AreEqual(result, obResult.StatusCode);
        }

        public static IEnumerable<TestCaseData> ViewClassDetailByTraineeIdTestCaseFalse
        {
            get
            {
                yield return new TestCaseData(
                    2,
                    404
                );
            }
        }

        [Test]
        [TestCaseSource("ViewClassDetailByTraineeIdTestCaseFalse")]
        public async Task ViewClassDetailByTraineeIdTrue_ActionResult_Status404(int traineeId, int result)
        {
            ClassController controller = new ClassController(mockClass.Object,mockTrainee.Object,mockAdmin.Object, mockModule.Object, mockTrainer.Object, mockMapper.Object,null);

            mockTrainee.Setup(x => x.GetClassIdByTraineeId(traineeId)).ReturnsAsync((-1, ""));

            mockClass.Setup(c => c.GetClassDetail(-1)).ReturnsAsync(classDetailfalse);

            var actual = await controller.ViewClassDetailByTraineeId(traineeId);
            var obResult = actual.Result as ObjectResult;

            Assert.AreEqual(result, obResult.StatusCode);
        }

        public static IEnumerable<TestCaseData> ViewClassDetailByTraineeIdTestCaseFalseNull
        {
            get
            {
                yield return new TestCaseData(
                    3,
                    404
                );
            }
        }
        Class classDetailfalse = new Class
        {
            
        };
        [Test]
        [TestCaseSource("ViewClassDetailByTraineeIdTestCaseFalseNull")]
        public async Task ViewClassDetailByTraineeIdTrue_ActionResult_Status404_nullObject(int traineeId, int result)
        {
            ClassController controller = new ClassController(mockClass.Object,mockTrainee.Object,mockAdmin.Object, mockModule.Object, mockTrainer.Object, mockMapper.Object,null);

            mockTrainee.Setup(x => x.GetClassIdByTraineeId(traineeId)).ReturnsAsync((1, ""));

            mockClass.Setup(c => c.GetClassDetail(-1)).ReturnsAsync(classDetailfalse);

            var actual = await controller.ViewClassDetailByTraineeId(traineeId);
            var obResult = actual.Result as ObjectResult;

            Assert.AreEqual(result, obResult.StatusCode);
        }


    }
}