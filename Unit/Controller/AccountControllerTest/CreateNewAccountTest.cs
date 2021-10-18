using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.Controllers;
using kroniiapi.DTO;
using kroniiapi.DTO.AccountDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace kroniiapiTest.Unit.Controller.AccountControllerTest
{
    [TestFixture]
    public class CreateNewAccountTest
    {
        private readonly Mock<IMapper> mockMapper = new Mock<IMapper>();
        private readonly Mock<IAccountService> mockAccountService = new Mock<IAccountService>();
        private readonly Mock<IEmailService> mockEmailService = new Mock<IEmailService>();

        [Test]
        public async Task CreateNewAccount_Success() {
            // Arrange
            AccountInput accountInput = new AccountInput();
            AccountController controller = new AccountController(mockAccountService.Object, mockMapper.Object, mockEmailService.Object);
            mockAccountService.Setup(acc => acc.InsertNewAccount(accountInput)).ReturnsAsync(1);
            mockAccountService.Setup(acc => acc.SaveChange()).ReturnsAsync(1);
            // Act
            var result = await controller.CreateNewAccount(accountInput) as ObjectResult;
            var response = result.Value as ResponseDTO;
            // Assert
            Assert.AreEqual(201, result.StatusCode);
            Assert.AreEqual(201, response.Status);
        }

        [Test]
        public async Task CreateNewAccount_Existed() {
            // Arrange
            AccountInput accountInput = new AccountInput();
            AccountController controller = new AccountController(mockAccountService.Object, mockMapper.Object, mockEmailService.Object);
            mockAccountService.Setup(acc => acc.InsertNewAccount(accountInput)).ReturnsAsync(-1);
            // Act
            var result = await controller.CreateNewAccount(accountInput) as ObjectResult;
            var response = result.Value as ResponseDTO;
            // Assert
            Assert.AreEqual(409, result.StatusCode);
            Assert.AreEqual(409, response.Status);
        }

        [Test]
        public async Task CreateNewAccount_Failed() {
            // Arrange
            AccountInput accountInput = new AccountInput();
            AccountController controller = new AccountController(mockAccountService.Object, mockMapper.Object, mockEmailService.Object);
            mockAccountService.Setup(acc => acc.InsertNewAccount(accountInput)).ReturnsAsync(1);
            mockAccountService.Setup(acc => acc.SaveChange()).ReturnsAsync(0);
            // Act
            var result = await controller.CreateNewAccount(accountInput) as ObjectResult;
            var response = result.Value as ResponseDTO;
            // Assert
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual(400, response.Status);
        }
    }
}