using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.AccountDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace kroniiapiTest.Unit.Controller.AccountControllerTest
{
    public class GetAccountListTest
    {
        
        private DataContext _context;
        private readonly Mock<IAccountService> mockAccountService = new Mock<IAccountService>();
        public static readonly Mock<IEmailService> mockEmailService = new Mock<IEmailService>();
        private readonly Mock<IMapper> mockMapper = new Mock<IMapper>();

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
                Username = "thinh",
                Fullname = "Nguyen Phuc Thinh",
                Email = "thinhnp@gmail.com",
                Role = "trainee"
            }
        };
        public static IEnumerable<TestCaseData> GetAccountListTestCaseTrue
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
                        SearchName = "fpt.com"
                    },
                    200
                );
            }
        }
        [TestCaseSource("GetAccountListTestCaseTrue")]
        [Test]
        public async Task GetAccountList_ReturnActionResult_Return200(PaginationParameter paginationParameter,int stacode)
        {
            //Calling Controller using 2 mock Object
            AccountController accountController 
                = new AccountController(mockAccountService.Object,mockMapper.Object,mockEmailService.Object);
            
            // Setup Services return using Mock
            mockAccountService.Setup(x=>x.GetAccountList(paginationParameter)).ReturnsAsync(Tuple.Create(2,listAcc));
             // Get Controller return result
            var actual = await accountController.GetAccountList(paginationParameter);
            var okResult = actual.Result as ObjectResult;
            
            // Assert result with expected result: this time is 404
            Assert.AreEqual(stacode, okResult.StatusCode);
        }

        public static IEnumerable<TestCaseData> GetAccountListTestCaseFail
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
        [TestCaseSource("GetAccountListTestCaseFail")]
        public async Task GetAccountList_ActionResult_404(PaginationParameter paginationParameter,int stacode)
        {
            //Calling Controller using 2 mock Object
            AccountController controller = new AccountController(mockAccountService.Object, mockMapper.Object, mockEmailService.Object);

            // Setup Services return using Mock
            mockAccountService.Setup(x => x.GetAccountList(paginationParameter)).ReturnsAsync(Tuple.Create(0,listAccfail));

            // Get Controller return result
            var actual = await controller.GetAccountList(paginationParameter);
            var okResult = actual.Result as ObjectResult;
            
            // Assert result with expected result: this time is 404
            Assert.AreEqual(stacode, okResult.StatusCode);
        }
    }
}