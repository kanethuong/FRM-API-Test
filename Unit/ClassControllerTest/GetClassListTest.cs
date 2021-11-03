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

namespace kroniiapiTest.Unit.ClassControllerTest
{
    public class GetClassListTest
    {
        private readonly Mock<IClassService> mockClass = new Mock<IClassService>();
        private readonly Mock<ITraineeService> mockTrainee= new Mock<ITraineeService>();
        private readonly Mock<IAdminService> mockAdmin= new Mock<IAdminService>();
        private readonly Mock<IFeedbackService> mockFeedback= new Mock<IFeedbackService>();
        private readonly Mock<ITrainerService> mockTrainer= new Mock<ITrainerService>();
        private readonly Mock<IMarkService> mockMark= new Mock<IMarkService>();
        private readonly Mock<IModuleService> mockModule= new Mock<IModuleService>();
        private readonly Mock<IMapper> mockMapper = new Mock<IMapper>();

        public static IEnumerable<TestCaseData> GetClassListTestCaseTrue
        {
            get
            {
                // True case: with PageNumber, PageSize and SearchName
                yield return new TestCaseData(
                    new PaginationParameter
                    {
                        PageNumber = 1,
                        PageSize = 1,
                        SearchName = "hostcode0301"
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
        IEnumerable<Class> listClassService = new List<Class>
        {
            new Class
            {
                ClassId = 1,
                ClassName = "Phuttt",
                Description = "Tran Thien Phu",
                CreatedAt = new DateTime(2021,10,18),
                StartDay = new DateTime(2021,10,18),
                EndDay = new DateTime(2024,10,18),
                IsDeactivated = false,
                DeactivatedAt = null,
                AdminId = 1,
                TrainerId = 1,
                RoomId = 1,


            },

            new Class
            {
                ClassId = 2,
                ClassName = "Thinh",
                Description = "Ngu",
                CreatedAt = new DateTime(2021,10,18),
                StartDay = new DateTime(2021,10,18),
                EndDay = new DateTime(2024,10,18),
                IsDeactivated = false,
                DeactivatedAt = null,
                AdminId = 1,
                TrainerId = 1,
                RoomId = 1,
                
            }
        };

        [Test]
        [TestCaseSource("GetClassListTestCaseTrue")]
        public async Task GetClassList_ActionResult_200(PaginationParameter paginationParameter,int stacode)
        {
            //Calling Controller using 2 mock Object
            ClassController controller = new ClassController(mockClass.Object,mockTrainee.Object,mockAdmin.Object, mockModule.Object, mockTrainer.Object, mockMapper.Object,null);

            // Setup Services return using Mock
            mockClass.Setup(x => x.GetClassList(paginationParameter)).ReturnsAsync(Tuple.Create(2,listClassService));

            // Get Controller return result
            var actual = await controller.GetClassList(paginationParameter);
            var okResult = actual.Result as ObjectResult;
            
            // Assert result with expected result: this time is 404
            Assert.AreEqual(stacode, okResult.StatusCode);
        }

        public static IEnumerable<TestCaseData> GetClassListTestCaseFail
        {
            get
            {
                // Fail case: with no PageNumber, PageSize and SearchName
                yield return new TestCaseData(
                    new PaginationParameter
                    {

                    },
                    404
                );
        
                // Fail case: with oversizing
                yield return new TestCaseData(
                    new PaginationParameter
                    {
                        PageNumber = 10,
                        PageSize = 100
                    },
                    404
                );
            }
        }

        IEnumerable<Class> listClassServicefail = new List<Class>
        {
            new Class
            {


            },

            new Class
            {
                
            }
        };

        [Test]
        [TestCaseSource("GetClassListTestCaseFail")]
        public async Task GetClassList_ActionResult_404(PaginationParameter paginationParameter,int stacode)
        {
            //Calling Controller using 2 mock Object
            ClassController controller = new ClassController(mockClass.Object,mockTrainee.Object,mockAdmin.Object, mockModule.Object, mockTrainer.Object, mockMapper.Object,null);

            // Setup Services return using Mock
            mockClass.Setup(x => x.GetClassList(paginationParameter)).ReturnsAsync(Tuple.Create(0,listClassServicefail));

            // Get Controller return result
            var actual = await controller.GetClassList(paginationParameter);
            var okResult = actual.Result as ObjectResult;
            
            // Assert result with expected result: this time is 404
            Assert.AreEqual(stacode, okResult.StatusCode);
        }


    }
}