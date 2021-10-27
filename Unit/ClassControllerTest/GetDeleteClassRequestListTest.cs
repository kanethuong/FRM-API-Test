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
using NUnit.Framework.Internal;

namespace kroniiapiTest.Unit.ClassControllerTest
{
    public class GetDeleteClassRequestListTest
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
                // True case: with SearchName
                yield return new TestCaseData(
                    new PaginationParameter
                    {
                        SearchName = "hentaiz.net"
                    },
                    200
                );
            }
        }
        
        IEnumerable<DeleteClassRequest> deleteClassRequestsListTrue = new List<DeleteClassRequest>(){
            new DeleteClassRequest{
                AcceptedAt = null,
                AdminId = 1,
                ClassId =1,
                IsAccepted = false,
                DeleteClassRequestId=1,
                Reason = "Mot hai ba",
                CreatedAt = new DateTime(2021/10/10)

            },
            new DeleteClassRequest{
                AcceptedAt = null,
                AdminId = 1,
                ClassId =2,
                IsAccepted = false,
                DeleteClassRequestId=1,
                Reason = "Mot hai ba",
                CreatedAt = new DateTime(2021/10/10)
            }
           
        };
        [Test]
        [TestCaseSource("GetClassListTestCaseTrue")]
        public async Task GetDeleteClassRequestList_ActionResult_200(PaginationParameter paginationParameter,int stacode)
        {
            //Calling Controller using 2 mock Object
            ClassController controller = new ClassController(mockClass.Object,mockTrainee.Object,mockMark.Object,mockAdmin.Object, mockModule.Object, mockTrainer.Object, mockFeedback.Object, mockMapper.Object);

            // Setup Services return using Mock
            mockClass.Setup(x => x.GetRequestDeleteClassList(paginationParameter)).ReturnsAsync(Tuple.Create(1,deleteClassRequestsListTrue));
            // Get Controller return result
            var actual = await controller.GetDeleteClassRequestList(paginationParameter);
            var okResult = actual.Result as ObjectResult;
            
            // Assert result with expected result: this time is 404
            Assert.AreEqual(stacode, okResult.StatusCode);
        }

        public static IEnumerable<TestCaseData> GetClassListTestCaseFail
        {
            get
            {
                // True fail: with no PageNumber, PageSize and SearchName
                yield return new TestCaseData(
                    new PaginationParameter
                    {

                    },
                    404
                );
        
                // True fail: with oversizing
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

        IEnumerable<DeleteClassRequest> deleteClassRequestsListFail = new List<DeleteClassRequest>(){
            new DeleteClassRequest{
                

            },
            new DeleteClassRequest{
               
            }
           
        };

        [Test]
        [TestCaseSource("GetClassListTestCaseFail")]
        public async Task GetDeleteClassRequestList_ActionResult_404(PaginationParameter paginationParameter,int stacode)
        {
            //Calling Controller using 2 mock Object
            ClassController controller = new ClassController(mockClass.Object,mockTrainee.Object,mockMark.Object,mockAdmin.Object, mockModule.Object, mockTrainer.Object, mockFeedback.Object, mockMapper.Object);

            // Setup Services return using Mock
            mockClass.Setup(x => x.GetRequestDeleteClassList(paginationParameter)).ReturnsAsync(Tuple.Create(0,deleteClassRequestsListFail));

            // Get Controller return result
            var actual = await controller.GetDeleteClassRequestList(paginationParameter);
            var okResult = actual.Result as ObjectResult;
            
            // Assert result with expected result: this time is 404
            Assert.AreEqual(stacode, okResult.StatusCode);
        }


    }
}