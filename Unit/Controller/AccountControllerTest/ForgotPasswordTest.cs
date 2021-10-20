using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DTO;
using kroniiapi.DTO.AccountDTO;
using kroniiapi.Helper;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace kroniiapiTest.Unit.Controller.AccountControllerTest
{
    [TestFixture]
    public class ForgotPasswordTest
    {
        private readonly Mock<IMapper> mockMapper = new Mock<IMapper>();
        private readonly Mock<IAccountService> mockAccountService = new Mock<IAccountService>();
        private readonly Mock<IEmailService> mockEmailService = new Mock<IEmailService>();

        public static IEnumerable<TestCaseData> ForgotPasswordTestCases
        {
            get
            {
                //True case
                yield return new TestCaseData(
                    new EmailInput
                    {
                        Email = "khanhtoan@gmail.com"
                    },
                    200
                );

                //False case
                yield return new TestCaseData(
                    new EmailInput
                    {
                        Email = ""
                    },
                    404
                );

                //False case
                yield return new TestCaseData(
                    null,
                    404
                );
            }
        }

        [Test]
        [TestCaseSource("ForgotPasswordTestCases")]
        public async Task ForgotPassword_TestEmailInput(EmailInput emailInput, int expStatus)
        {
            // Arrange
            AccountController controller=new AccountController(mockAccountService.Object,mockMapper.Object,mockEmailService.Object);
            mockAccountService.Setup(acc => acc.UpdateAccountPassword((emailInput!=null)?emailInput.Email:null,It.IsAny<string>())).ReturnsAsync(1);

            // Act
            var rs=await controller.ForgotPassword(emailInput) as ObjectResult;
            var response=rs.Value as ResponseDTO;

            // Assert
            Assert.True(
                expStatus==rs.StatusCode &&
                expStatus==response.Status
            );
        }

        [Test]
        public async Task ForgotPassword_Success()
        {
            // Arrange
            EmailInput emailInput=new EmailInput();
            AccountController controller=new AccountController(mockAccountService.Object,mockMapper.Object,mockEmailService.Object);
            mockAccountService.Setup(acc => acc.UpdateAccountPassword(emailInput.Email,It.IsAny<string>())).ReturnsAsync(1);

            // Act
            var rs=await controller.ForgotPassword(emailInput) as ObjectResult;
            var response=rs.Value as ResponseDTO;

            // Assert
            Assert.True(
                200==rs.StatusCode &&
                200==response.Status
            );
        }

        [Test]
        public async Task ForgotPassword_Fail()
        {
            // Arrange
            EmailInput emailInput=new EmailInput();
            AccountController controller=new AccountController(mockAccountService.Object,mockMapper.Object,mockEmailService.Object);
            mockAccountService.Setup(acc => acc.UpdateAccountPassword(emailInput.Email,It.IsAny<string>())).ReturnsAsync(0);

            // Act
            var rs=await controller.ForgotPassword(emailInput) as ObjectResult;
            var response=rs.Value as ResponseDTO;

            // Assert
            Assert.True(
                404==rs.StatusCode &&
                404==response.Status
            );
        }
    }
}