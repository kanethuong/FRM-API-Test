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

namespace kroniiapiTest.Unit.AccountControllerTest
{
    public class GetDeactivatedAccountListTest 
    {
        private readonly Mock<IAccountService> mockAccount = new Mock<IAccountService>();
        private readonly Mock<IMapper> mockMapper = new Mock<IMapper>();
        private readonly Mock<IEmailService> mockEmail = new Mock<IEmailService>();


        public static IEnumerable<TestCaseData> DeactivatedAccListTestCase
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
        IEnumerable<AccountResponse> listAcc = new List<AccountResponse>
        {
            new AccountResponse
            {
                AccountId = 1,
                Username = "Phuttt",
                Fullname = "Tran Thien Phu",
                Email = "hostcode0301@gmail.com",
                Role = "admin"
            },

            new AccountResponse
            {
                AccountId = 2,
                Username = "Fuck",
                Fullname = "Thinh Cho Dien",
                Email = "thinhnp@gmail.com",
                Role = "trainee"
            }
        };

        [Test]
        [TestCaseSource("DeactivatedAccListTestCase")]
        public async Task GetDeactivatedAccountList_ActionResult_200(PaginationParameter paginationParameter,int stacode)
        {
            //Calling Controller using 2 mock Object
            AccountController controller = new AccountController(mockAccount.Object, mockMapper.Object, mockEmail.Object);

            // Setup Services return using Mock
            mockAccount.Setup(x => x.GetDeactivatedAccountList(paginationParameter)).ReturnsAsync(Tuple.Create(2,listAcc));

            // Get Controller return result
            var actual = await controller.GetDeactivatedAccountList(paginationParameter);
            var okResult = actual as ObjectResult;
            
            // Assert result with expected result: this time is 404
            Assert.AreEqual(stacode, okResult.StatusCode);
        }

        public static IEnumerable<TestCaseData> DeactivatedAccListTestCaseFail
        {
            get
            {
                // True case: with PageNumber, PageSize and SearchName
                yield return new TestCaseData(
                    new PaginationParameter
                    {
                        
                    },
                    404

                );
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

        IEnumerable<AccountResponse> listAccfail = new List<AccountResponse>
        {
            new AccountResponse
            {

            }
        };

        [Test]
        [TestCaseSource("DeactivatedAccListTestCaseFail")]
        public async Task GetDeactivatedAccountList_ActionResult_404(PaginationParameter paginationParameter,int stacode)
        {
            //Calling Controller using 2 mock Object
            AccountController controller = new AccountController(mockAccount.Object, mockMapper.Object, mockEmail.Object);

            // Setup Services return using Mock
            mockAccount.Setup(x => x.GetDeactivatedAccountList(paginationParameter)).ReturnsAsync(Tuple.Create(0,listAccfail));

            // Get Controller return result
            var actual = await controller.GetDeactivatedAccountList(paginationParameter);
            var okResult = actual as ObjectResult;
            
            // Assert result with expected result: this time is 404
            Assert.AreEqual(stacode, okResult.StatusCode);
        }
    }
}