using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.ClassDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.DTO.Profiles;
using kroniiapi.Helper.Upload;
using kroniiapi.Helper.UploadDownloadFile;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework;
using kroniiapi.DTO.TraineeDTO;

namespace kroniiapitest.Intergration.TraineeControllerTest
{
    public class ViewAttendanceReportTest
    {
        private IMapper _mapper;
        private IClassService _classService;
        private IFeedbackService _feedbackService;
        private ITraineeService _traineeService;
        private IImgHelper _imgHelper;
        private ICalendarService _calendarService;
        private IModuleService _moduleService;
        private ITrainerService _trainerService;
        private IRoomService _roomService;
        private IExamService _examService;
        private ICertificateService _certificateService;
        private IApplicationService _applicationService;
        private IMegaHelper _megaHelper;
        private DataContext dataContext;

        private TraineeController traineeController;
        List<Trainee> trainee = new List<Trainee>
        {
            new Trainee{
                TraineeId = 1,
                Username = "CongVinh",
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
                IsDeactivated = false,
                RoleId = 4,
                ClassId = 1,
            },
            new Trainee{
                TraineeId = 2,
                Username = "ThuyTien",
                Password = "123456",
                Fullname = "CaiGiDoThuyTien",
                AvatarURL = "none",
                Email = "thuytiencungkosaoke@gmail.vn",
                Phone = "0992377",
                DOB = new DateTime(1970, 2, 1),
                Address = "Nam Long, Quan 7, HCM",
                Gender = "Female",
                Wage = 10000,
                CreatedAt = new DateTime(2021, 1, 1),
                IsDeactivated = true,
                RoleId = 4,
                ClassId = 1,
            }

        };

        Module module = new Module
        {
            ModuleId = 1,
            ModuleName = "Math",
            Description = "Day cach sao ke",
            NoOfSlot = 30,
            IconURL = "ThuyTienvlxx.com",
            SyllabusURL = "Bao lau moi an dc 1 ty tien tu thien?",
            CreatedAt = new DateTime(2021, 1, 1)
        };

        Mark mark = new Mark
        {
            ModuleId = 1,
            TraineeId = 1,
            Score = 3,
        };

        Certificate certificate = new Certificate
        {
            ModuleId = 1,
            TraineeId = 1,
            CertificateURL = "Co cai cc, tinh sai vl doi certificate",
        };

        Attendance attendance = new Attendance
        {
            CalendarId = 1,
            TraineeId = 1,
            IsAbsent = true,
            Reason = "Dem tien met qua nen nghi"
        };

        Calendar calendar = new Calendar
        {
            CalendarId = 1,
            SyllabusSlot = 10,
            SlotInDay = 3,
            ModuleId = 1,
            ClassId = 1
        };

        ClassModule classModule = new ClassModule
        {
            ClassId = 1,
            ModuleId = 1,
        };

        [OneTimeSetUp]
        public void setupFirst()
        {
            var option = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase("data").Options;
            dataContext = new DataContext(option);
            dataContext.Trainees.AddRange(trainee);
            dataContext.Modules.AddRange(module);
            dataContext.Certificates.AddRange(certificate);
            dataContext.Marks.AddRange(mark);
            dataContext.Attendances.AddRange(attendance);
            dataContext.Calendars.AddRange(calendar);
            dataContext.ClassModules.AddRange(classModule);
            dataContext.SaveChanges();

            var config = new MapperConfiguration(config =>
            {
                config.AddProfile(new ClassProfile());
            });
            _mapper = config.CreateMapper();

            _trainerService = new TrainerService(
                dataContext,
                _classService
            );
            _traineeService = new TraineeService(
                dataContext
            );
            _moduleService = new ModuleService(
                dataContext
            );
            _classService = new ClassService(
                dataContext,
                _mapper,
                _traineeService
            );
            _feedbackService = new FeedbackService(
                dataContext
            );
            _roomService = new RoomService(
                dataContext
            );
            _certificateService = new CertificateService(
                dataContext
            );
            _calendarService = new CalendarService(
                dataContext
            );
            _examService = new ExamService(
                dataContext
            );

            traineeController = new TraineeController(_mapper,
                                                      _classService,
                                                      _feedbackService,
                                                      _traineeService,
                                                      _calendarService,
                                                      _moduleService,
                                                      _trainerService,
                                                      _roomService,
                                                      _examService,
                                                      _certificateService,
                                                      _applicationService,
                                                      _megaHelper,
                                                      _imgHelper);
        }
        [OneTimeTearDown]
        public void tearDown()
        {
            dataContext.Trainees.RemoveRange(trainee);
            dataContext.Modules.RemoveRange(module);
            dataContext.Certificates.RemoveRange(certificate);
            dataContext.Marks.RemoveRange(mark);
            dataContext.Attendances.RemoveRange(attendance);
            dataContext.Calendars.RemoveRange(calendar);
            dataContext.ClassModules.RemoveRange(classModule);
            dataContext.SaveChanges();
        }

        public static IEnumerable<TestCaseData> GetAttendanceReportTestTrue
        {
            get
            {
                // True case: with PageNumber, PageSize and SearchName
                yield return new TestCaseData(
                    1,
                    new PaginationParameter
                    {
                        PageNumber = 1,
                        PageSize = 1,
                        SearchName = ""
                    },
                    200,
                    new PaginationResponse<IEnumerable<TraineeAttendanceReport>>(1,
                    new List<TraineeAttendanceReport>{
                        new TraineeAttendanceReport{
                            ModuleName = "Math",
                            NoOfSlot = 30,
                            NumberSlotAbsent = 1
                        }
                    }
                    )
                );
            }
        }

        [Test]
        [TestCaseSource("GetAttendanceReportTestTrue")]
        public async Task GetClassListTestTrue_200(int id, PaginationParameter paginationParameter, int expectedStatus, PaginationResponse<IEnumerable<TraineeAttendanceReport>> paginationResponseTrue)
        {
            var rs = await traineeController.ViewAttendanceReport(id, paginationParameter);
            var obResult = rs.Result as ObjectResult;
            var listResult = ((rs.Result as OkObjectResult).Value as PaginationResponse<IEnumerable<TraineeAttendanceReport>>);
            var actualJson = JsonConvert.SerializeObject(listResult);
            var expectedJson = JsonConvert.SerializeObject(paginationResponseTrue);
            Assert.AreEqual(expectedJson, actualJson);
            Assert.AreEqual(expectedStatus, obResult.StatusCode);
        }



        public static IEnumerable<TestCaseData> GetAttendanceReportTestFail
        {
            get
            {
                // Fail case: Trainee id is deactivated
                yield return new TestCaseData(
                    2,
                    new PaginationParameter
                    {
                        PageNumber = 1,
                        PageSize = 1,
                        SearchName = ""
                    },
                    404
                );
                // Fail case: Trainee id is not existed
                yield return new TestCaseData(
                    99999,
                    new PaginationParameter
                    {
                        PageNumber = 1,
                        PageSize = 1,
                        SearchName = ""
                    },
                    404
                );
            }
        }

        [Test]
        [TestCaseSource("GetAttendanceReportTestFail")]
        public async Task GetClassListTestFail_404(int id, PaginationParameter paginationParameter, int expectedStatus)
        {
            var rs = await traineeController.ViewAttendanceReport(id, paginationParameter);
            var obResult = rs.Result as ObjectResult;
            Assert.AreEqual(expectedStatus, obResult.StatusCode);
        }

    }
}