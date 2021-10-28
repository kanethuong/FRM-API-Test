using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DB.Models;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.DTO.TraineeDTO;
using kroniiapi.Helper.Upload;
using kroniiapi.Helper.UploadDownloadFile;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace kroniiapiTest.Unit.TraineeControllerTest
{
    public class ViewAttendanceReportTest
    {
        private readonly Mock<IClassService> mockClass = new Mock<IClassService>();
        private readonly Mock<ITraineeService> mockTrainee = new Mock<ITraineeService>();
        private readonly Mock<IFeedbackService> mockFeedback = new Mock<IFeedbackService>();
        private readonly Mock<ITrainerService> mockTrainer = new Mock<ITrainerService>();
        private readonly Mock<IModuleService> mockModule = new Mock<IModuleService>();
        private readonly Mock<IMapper> mockMapper = new Mock<IMapper>();
        private readonly Mock<ICalendarService> mockCalendar = new Mock<ICalendarService>();
        private readonly Mock<IRoomService> mockRoom = new Mock<IRoomService>();
        private readonly Mock<IExamService> mockExam = new Mock<IExamService>();
        private readonly Mock<ICertificateService> mockCertificate = new Mock<ICertificateService>();
        private readonly Mock<IApplicationService> mockApplication = new Mock<IApplicationService>();
        private readonly Mock<IMegaHelper> mockMegaHelper = new Mock<IMegaHelper>();
        private readonly Mock<IImgHelper> mockImgHelper = new Mock<IImgHelper>();


        public static IEnumerable<TestCaseData> GetAttendanceReportTestCaseTrue
        {
            get
            {
                // True case: with Pagination
                yield return new TestCaseData(
                    1,
                    new PaginationParameter
                    {
                        PageNumber = 1,
                        PageSize = 1,
                    },
                    200
                );
                // True case: without Pagination
                yield return new TestCaseData(
                    99999,
                    new PaginationParameter
                    {

                    },
                    200
                );
            }
        }

        IEnumerable<TraineeAttendanceReport> listAttendanceReport = new List<TraineeAttendanceReport>
        {
            new TraineeAttendanceReport
            {
                ModuleName = "Math",
                NoOfSlot = 30,
                NumberSlotAbsent = 1
            },

            new TraineeAttendanceReport
            {
                ModuleName = "Chemist",
                NoOfSlot = 30,
                NumberSlotAbsent = 2

            }
        };

        Trainee trainee = new Trainee
        {
            TraineeId = 1,
            Fullname = "Nguyen Phuc Thinh",
            Email = "ThinhNPCE22783@gmail.com",
            Class = new Class
            {
                ClassId = 1,
                ClassName = "Math"
            }
        };



        [Test]
        [TestCaseSource("GetAttendanceReportTestCaseTrue")]
        public async Task GetAttendanceReportTestTrue_200(int id, PaginationParameter paginationParameter, int stacode)
        {
            //Calling Controller using 2 mock Object
            TraineeController controller = new TraineeController(mockMapper.Object,
                                                                 mockClass.Object,
                                                                 mockFeedback.Object,
                                                                 mockTrainee.Object,
                                                                 mockCalendar.Object,
                                                                 mockModule.Object,
                                                                 mockTrainer.Object,
                                                                 mockRoom.Object,
                                                                 mockExam.Object,
                                                                 mockCertificate.Object,
                                                                 mockApplication.Object,
                                                                 mockMegaHelper.Object,
                                                                 mockImgHelper.Object);

            // Setup Services return using Mock
            mockTrainee.Setup(t => t.GetTraineeById(id)).ReturnsAsync(trainee);
            mockTrainee.Setup(t => t.GetAttendanceReports(id, paginationParameter)).ReturnsAsync(Tuple.Create(2, listAttendanceReport));
            // Get Controller return result
            var actual = await controller.ViewAttendanceReport(id, paginationParameter);
            var okResult = actual.Result as ObjectResult;

            // Assert result with expected result: this time is 200
            Assert.AreEqual(stacode, okResult.StatusCode);
        }

        //-----------------------------------------------------------------------------------//

        public static IEnumerable<TestCaseData> GetAttendanceReportTestCaseFail
        {
            get
            {
                // Fail case: deactivated trainee id
                yield return new TestCaseData(
                    new Trainee
                    {
                        TraineeId = 1,
                        Fullname = "Nguyen Phuc Thinh",
                        Email = "ThinhNPCE22783@gmail.com",
                        Class = new Class
                        {
                            ClassId = 1,
                            ClassName = "Math"
                        },
                        IsDeactivated = true
                    },
                    new PaginationParameter
                    {
                        PageNumber = 1,
                        PageSize = 1,
                    },
                    400
                );
                // Fail case: Trainee not found
                yield return new TestCaseData(
                    new Trainee{

                    },
                    new PaginationParameter
                    {

                    },
                    400
                );
            }
        }

        IEnumerable<TraineeAttendanceReport> listAttendanceReportEmpty = new List<TraineeAttendanceReport>
        {

        };

        Trainee traineeDeactivated = new Trainee
        {

        };


        [Test]
        [TestCaseSource("GetAttendanceReportTestCaseFail")]
        public async Task GetAttendanceReportTestFail_400(Trainee trainee, PaginationParameter paginationParameter, int stacode)
        {
            //Calling Controller using 2 mock Object
            TraineeController controller = new TraineeController(mockMapper.Object,
                                                                 mockClass.Object,
                                                                 mockFeedback.Object,
                                                                 mockTrainee.Object,
                                                                 mockCalendar.Object,
                                                                 mockModule.Object,
                                                                 mockTrainer.Object,
                                                                 mockRoom.Object,
                                                                 mockExam.Object,
                                                                 mockCertificate.Object,
                                                                 mockApplication.Object,
                                                                 mockMegaHelper.Object,
                                                                 mockImgHelper.Object);

            // Setup Services return using Mock
            mockTrainee.Setup(t => t.GetAttendanceReports(trainee.TraineeId, paginationParameter)).ReturnsAsync(Tuple.Create(2, listAttendanceReport));
            // Get Controller return result
            var actual = await controller.ViewAttendanceReport(trainee.TraineeId, paginationParameter);
            var okResult = actual.Result as ObjectResult;
            // Assert result with expected result: this time is 400
            Assert.AreEqual(stacode, okResult.StatusCode);
        }



    }
}