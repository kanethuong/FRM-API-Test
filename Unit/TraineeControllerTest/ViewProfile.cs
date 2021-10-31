using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using kroniiapi.Controllers;
using kroniiapi.DTO.AccountDTO;
using kroniiapi.DB.Models;
using kroniiapi.DTO.ClassDTO;
using kroniiapi.Helper.UploadDownloadFile;
using kroniiapi.Helper.Upload;

namespace kroniiapitest.Unit.TraineeControllerTest
{
    public class ViewProfile
    {
        private readonly Mock<IMapper> mockMapper;
        private readonly Mock<IClassService> mockClassService;
        private readonly Mock<IFeedbackService> mockFeedbackService;
        private readonly Mock<ITraineeService> mockTraineeService;
        private readonly Mock<IImgHelper> mockImg;
        private readonly Mock<ICalendarService> mockCalendar;
        private readonly Mock<IModuleService> mockModule;
        private readonly Mock<ITrainerService> _trainerService;
        private readonly Mock<IRoomService> _roomService;
        private readonly Mock<IExamService> _examService;
        private readonly Mock<ICertificateService> _certificateService;
        private readonly Mock<IApplicationService> _applicationService;
        private readonly Mock<IMegaHelper> _megaHelper;

        Trainee trainee = new Trainee
        {
            TraineeId = 1,
            Username = "congvinhkosaoaaaaaaaaaaaaaake",
            Password = "654321",
            Fullname = "LeCongVinh",
            AvatarURL = "none",
            Email = "congvinhkhongsaoke@gmail.vn",
            Phone = "029288290",
            DOB = new DateTime(2000, 2, 1),
            Address = "Nam Long, Quan 7, HCM",
            Gender = "Male",
            Wage = 10000,
            CreatedAt = new DateTime(2021, 1, 1),
            IsDeactivated = true,
            RoleId = 4,
        };
        [Test]
        public async Task ViewProfileTestTrue()
        {
            TraineeController controller = new TraineeController(mockMapper.Object, mockClassService.Object, mockFeedbackService.Object, mockTraineeService.Object, mockCalendar.Object, mockModule.Object, _trainerService.Object, _roomService.Object, _examService.Object, _certificateService.Object, _applicationService.Object, _megaHelper.Object, mockImg.Object);
            mockTraineeService.Setup(s => s.GetTraineeById(1)).ReturnsAsync(trainee);

            var actual = await controller.ViewProfile(1);
            var obrs = actual.Result as ObjectResult;

            Assert.AreEqual(200, obrs.StatusCode);

        }

    }
}