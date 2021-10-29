using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.ApplicationDTO;
using kroniiapi.DTO.Profiles;
using kroniiapi.Helper.Upload;
using kroniiapi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace kroniiapitest.Unit.TraineeControllerTest
{
    [TestFixture]
    public class SubmitApplicationFormTest
    {
        private IMapper mapper;
        private readonly Mock<IApplicationService> mockApplicationService;
        private readonly Mock<ITraineeService> mockTraineeService;
        private readonly Mock<IMegaHelper> mockMegaHelper;
        private ApplicationController controller;

        private static IEnumerable<TestCaseData> SubmitApplication
        {
            get
            {
                yield return new TestCaseData(
                    new ApplicationInput
                    {
                        Description = "Xac nhan sv",
                        TraineeId = 1,
                        ApplicationCategoryId = 6
                    },
                    1,
                    200
                    );                
            }
        }


        [Test]
        [TestCaseSource("SubmitApplication")]
        public async Task EditProfile_Result(ApplicationInput applicationInput,int result, int statusCode)
        {
            // controller = new ApplicationController(mapper, null, mockApplicationService.Object, null);
            // mockMegaHelper.Setup(m => m.Upload()); 
            // mockApplicationService.Setup(tr => tr.InsertNewApplication(It.IsAny<Application>(), It.IsAny<IFormFile>())).ReturnsAsync(result);
            // var actual = await controller.SubmitApplicationForm(applicationInput, );
            // var okResult = actual.Result as ObjectResult;
            // Assert.AreEqual(statusCode, okResult.StatusCode);
        }
    }
}